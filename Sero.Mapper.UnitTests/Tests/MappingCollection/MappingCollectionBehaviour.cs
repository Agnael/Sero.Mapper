using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sero.Mapper.Tests.MappingCollectionTests;

/// <summary>
/// Characterization tests for MappingCollection / MappingCollectionBuilder.
///
/// These exist to pin the behaviour that was previously supplied by
/// SynchronizedCollection&lt;T&gt; (from System.ServiceModel.Primitives) before that
/// dependency was replaced, so any replacement can be judged against them rather
/// than by inspection.
///
/// Two properties matter:
///   1. It behaves as an ICollection&lt;T&gt; — that is the public contract, since
///      IMappingCollection derives from it.
///   2. Concurrent Add is safe, and duplicates are rejected exactly once even when
///      two threads race on the same mapping. This is the property a plain
///      Collection&lt;T&gt; does NOT have.
/// </summary>
public class MappingCollectionBehaviour
{
	static MappingHandler Handler(Type sourceType, Type destinationType) =>
		new MappingHandler(
			sourceType,
			destinationType,
			(ConvertMutable)((src, dest, mapper, sp) => { }),
			Enumerable.Empty<Tuple<Type, Type>>());

	static MappingHandler Handler<TSrc, TDest>() => Handler(typeof(TSrc), typeof(TDest));

	// ---------------------------------------------------------------- contract

	[Fact]
	public void Add__SingleMapping__IsRetrievable()
	{
		MappingCollection collection = new MappingCollection();

		collection.Add(Handler<SrcModel, DestModel>());

		Assert.Single(collection);
		Assert.NotNull(collection.GetMappingHandler(typeof(SrcModel), typeof(DestModel)));
	}

	[Fact]
	public void Add__DuplicateMapping__Throws()
	{
		MappingCollection collection = new MappingCollection();
		collection.Add(Handler<SrcModel, DestModel>());

		Assert.Throws<MappingCollectionDuplicateException>(
			() => collection.Add(Handler<SrcModel, DestModel>()));
	}

	[Fact]
	public void Add__DifferentTypePairs__BothStored()
	{
		MappingCollection collection = new MappingCollection();

		collection.Add(Handler<SrcModel, DestModel>());
		collection.Add(Handler<ComplexSrcModel, ComplexDestModel>());

		Assert.Equal(2, collection.Count);
	}

	[Fact]
	public void GetMappingHandler__Unregistered__Throws()
	{
		MappingCollection collection = new MappingCollection();

		Assert.Throws<MissingMappingException>(
			() => collection.GetMappingHandler(typeof(SrcModel), typeof(DestModel)));
	}

	[Fact]
	public void ICollectionMembers__AreUsable()
	{
		// IMappingCollection : ICollection<MappingHandler>, so a replacement base
		// class has to keep every one of these working.
		ICollection<MappingHandler> collection = new MappingCollection();
		MappingHandler handler = Handler<SrcModel, DestModel>();

		collection.Add(handler);

		Assert.Equal(1, collection.Count);
		Assert.True(collection.Contains(handler));
		Assert.False(collection.IsReadOnly);

		MappingHandler[] copy = new MappingHandler[1];
		collection.CopyTo(copy, 0);
		Assert.Same(handler, copy[0]);

		Assert.True(collection.Remove(handler));
		Assert.Empty(collection);

		collection.Add(handler);
		collection.Clear();
		Assert.Empty(collection);
	}

	// ------------------------------------------------------------- concurrency

	[Fact]
	public async Task Add__ManyThreadsDistinctMappings__NothingIsLost()
	{
		// A plain Collection<T> is not thread-safe: concurrent InsertItem calls
		// corrupt the backing List<T> and items go missing or the list throws.
		const int MAPPING_COUNT = 200;

		MappingCollection collection = new MappingCollection();

		// Distinct type pairs so the duplicate check never fires; this isolates
		// raw insertion safety from the check-then-act race below.
		List<MappingHandler> handlers = Enumerable.Range(0, MAPPING_COUNT)
			.Select(i => Handler(typeof(SrcModel), MakeDistinctDestType(i)))
			.ToList();

		await Task.WhenAll(handlers.Select(h => Task.Run(() => collection.Add(h))));

		Assert.Equal(MAPPING_COUNT, collection.Count);
	}

	[Fact]
	public async Task Add__TwoThreadsRaceOnSameMapping__ExactlyOneWins()
	{
		// The check-then-act case. Without a lock spanning both the duplicate
		// scan and the insert, two threads can both observe "not present" and
		// both insert, leaving two identical mappings and zero exceptions.
		const int ATTEMPTS = 60;

		for (int attempt = 0; attempt < ATTEMPTS; attempt++)
		{
			MappingCollection collection = new MappingCollection();

			int duplicateExceptions = 0;

			Task[] racers = Enumerable.Range(0, 2)
				.Select(_ => Task.Run(() =>
				{
					try
					{
						collection.Add(Handler<SrcModel, DestModel>());
					}
					catch (MappingCollectionDuplicateException)
					{
						System.Threading.Interlocked.Increment(ref duplicateExceptions);
					}
				}))
				.ToArray();

			await Task.WhenAll(racers);

			Assert.Equal(1, collection.Count);
			Assert.Equal(1, duplicateExceptions);
		}
	}

	[Fact]
	public async Task Builder__ManyThreadsAddDistinct__NothingIsLost()
	{
		const int MAPPING_COUNT = 200;

		MappingCollectionBuilder builder = new MappingCollectionBuilder();

		List<IMappingHandlerBuilder> builders = Enumerable.Range(0, MAPPING_COUNT)
			.Select(i => (IMappingHandlerBuilder)new StubHandlerBuilder(typeof(SrcModel), MakeDistinctDestType(i)))
			.ToList();

		await Task.WhenAll(builders.Select(b => Task.Run(() => builder.Add(b))));

		Assert.Equal(MAPPING_COUNT, builder.Count);
	}

	/// <summary>
	/// Produces a distinct Type per index so each mapping is a unique (src, dest)
	/// pair. Uses runtime-generated array types, which are cheap and guaranteed
	/// distinct without needing 200 declared classes.
	/// </summary>
	static Type MakeDistinctDestType(int index)
	{
		Type current = typeof(DestModel);

		for (int i = 0; i <= index; i++)
			current = current.MakeArrayType();

		return current;
	}

	class StubHandlerBuilder : IMappingHandlerBuilder
	{
		public StubHandlerBuilder(Type sourceType, Type destinationType)
		{
			SourceType = sourceType;
			DestinationType = destinationType;
		}

		public Type SourceType { get; }
		public Type DestinationType { get; }

		public MappingHandler Build() => Handler(SourceType, DestinationType);
	}
}
