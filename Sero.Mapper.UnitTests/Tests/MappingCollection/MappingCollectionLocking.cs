using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sero.Mapper.Tests.MappingCollectionTests;

/// <summary>
/// Deterministic tests that the synchronization actually exists, as opposed to
/// MappingCollectionBehaviour's concurrency tests, which hammer the collection and
/// hope to land on a bad interleaving.
///
/// A hammer test can only ever prove the UNLOCKED version reliably breaks; a green
/// run never proves the locked version is correct, because the scheduler may simply
/// not have produced the failing order. The tests here avoid that by not racing at
/// all: they hold the lock from the test thread and assert that writers block, which
/// either happens or does not.
/// </summary>
public class MappingCollectionLocking
{
	/// <summary>
	/// SyncRoot is protected so derived types can widen the lock. Exposing it here
	/// lets the test take the same lock the collection takes.
	/// </summary>
	class ProbeCollection : MappingCollection
	{
		public object ExposedSyncRoot => SyncRoot;
	}

	static MappingHandler Handler(Type sourceType, Type destinationType) =>
		new MappingHandler(
			sourceType,
			destinationType,
			(ConvertMutable)((src, dest, mapper, sp) => { }),
			Enumerable.Empty<Tuple<Type, Type>>());

	[Fact]
	public void Add__WhileSyncRootHeld__Blocks()
	{
		// Deterministic: if Add takes the lock, it cannot proceed while this thread
		// holds it. No interleaving required, no retries, no flake.
		ProbeCollection collection = new ProbeCollection();

		using ManualResetEventSlim addAttempted = new ManualResetEventSlim(false);

		Task adder;

		lock (collection.ExposedSyncRoot)
		{
			adder = Task.Run(() =>
			{
				addAttempted.Set();
				collection.Add(Handler(typeof(SrcModel), typeof(DestModel)));
			});

			// The task has definitely entered Add's critical section attempt.
			Assert.True(addAttempted.Wait(TimeSpan.FromSeconds(5)));

			// It must not complete while we hold the lock. A generous window: the
			// assertion is "still blocked", so a slow machine makes this MORE
			// reliable, not less.
			Assert.False(adder.Wait(TimeSpan.FromMilliseconds(300)));
			Assert.Empty(collection);
		}

		// Released — it should now finish promptly.
		Assert.True(adder.Wait(TimeSpan.FromSeconds(5)));
		Assert.Single(collection);
	}

	[Fact]
	public void Clear__WhileSyncRootHeld__Blocks()
	{
		// Same proof for the inherited mutators, which lock in the base class
		// rather than in the overridden Add.
		ProbeCollection collection = new ProbeCollection();
		collection.Add(Handler(typeof(SrcModel), typeof(DestModel)));

		using ManualResetEventSlim clearAttempted = new ManualResetEventSlim(false);

		Task clearer;

		lock (collection.ExposedSyncRoot)
		{
			clearer = Task.Run(() =>
			{
				clearAttempted.Set();
				collection.Clear();
			});

			Assert.True(clearAttempted.Wait(TimeSpan.FromSeconds(5)));
			Assert.False(clearer.Wait(TimeSpan.FromMilliseconds(300)));
			Assert.Single(collection);
		}

		Assert.True(clearer.Wait(TimeSpan.FromSeconds(5)));
		Assert.Empty(collection);
	}

	[Fact]
	public async Task Add__BarrierSynchronisedRace__ExactlyOneWins()
	{
		// Still a race, but a much sharper one than Task.Run gives: every thread is
		// created and warmed first, then released from a Barrier at the same instant,
		// so they contend on the duplicate check rather than running one after another.
		const int THREAD_COUNT = 8;
		const int ATTEMPTS = 25;

		for (int attempt = 0; attempt < ATTEMPTS; attempt++)
		{
			MappingCollection collection = new MappingCollection();

			int duplicateExceptions = 0;
			using Barrier barrier = new Barrier(THREAD_COUNT);

			Task[] racers = Enumerable.Range(0, THREAD_COUNT)
				.Select(_ => Task.Factory.StartNew(() =>
				{
					// Everyone waits here; the last arrival releases them together.
					barrier.SignalAndWait();

					try
					{
						collection.Add(Handler(typeof(SrcModel), typeof(DestModel)));
					}
					catch (MappingCollectionDuplicateException)
					{
						Interlocked.Increment(ref duplicateExceptions);
					}
				},
				// LongRunning gives each racer its own thread instead of a pool slot,
				// so the Barrier cannot deadlock against a saturated thread pool.
				TaskCreationOptions.LongRunning))
				.ToArray();

			await Task.WhenAll(racers);

			Assert.Equal(1, collection.Count);
			Assert.Equal(THREAD_COUNT - 1, duplicateExceptions);
		}
	}

	[Fact]
	public async Task Add__BarrierSynchronisedDistinct__NothingIsLost()
	{
		const int THREAD_COUNT = 16;
		const int PER_THREAD = 25;

		MappingCollection collection = new MappingCollection();
		using Barrier barrier = new Barrier(THREAD_COUNT);

		Task[] writers = Enumerable.Range(0, THREAD_COUNT)
			.Select(threadIndex => Task.Factory.StartNew(() =>
			{
				barrier.SignalAndWait();

				for (int i = 0; i < PER_THREAD; i++)
					collection.Add(Handler(typeof(SrcModel), MakeDistinctDestType(threadIndex * PER_THREAD + i)));
			},
			TaskCreationOptions.LongRunning))
			.ToArray();

		await Task.WhenAll(writers);

		Assert.Equal(THREAD_COUNT * PER_THREAD, collection.Count);

		// Every mapping must be individually retrievable, not merely counted — a
		// torn write could leave the right Count with a corrupted element.
		for (int i = 0; i < THREAD_COUNT * PER_THREAD; i++)
			Assert.NotNull(collection.GetMappingHandler(typeof(SrcModel), MakeDistinctDestType(i)));
	}

	static Type MakeDistinctDestType(int index)
	{
		Type current = typeof(DestModel);

		for (int i = 0; i <= index; i++)
			current = current.MakeArrayType();

		return current;
	}
}
