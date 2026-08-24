using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sero.Mapper;

/// <summary>
/// A thread-safe <see cref="ICollection{T}"/>, replacing
/// <c>System.ServiceModel.Primitives</c>' <c>SynchronizedCollection&lt;T&gt;</c>.
///
/// That type sits in the System.Collections.Generic namespace but ships in the WCF
/// client package, so depending on it dragged in a chain ending at
/// System.Drawing.Common 5.0.0 (critical advisory) and
/// System.Security.Cryptography.Xml 5.0.0 (moderate) — for a mapping registry that
/// has nothing to do with WCF, and where nothing in the source even spells
/// "ServiceModel".
///
/// Collection&lt;T&gt; is the BCL equivalent and lives in System.Runtime. The locking
/// below restores the one thing it lacks. Reads that go through LINQ (Any,
/// FirstOrDefault) enumerate without taking the lock, which matches
/// SynchronizedCollection's own behaviour: this type is written during
/// registration and read afterwards, so concurrent add-while-enumerating is not a
/// pattern it ever supported.
/// </summary>
public class SynchronizedCollectionBase<T> : Collection<T>
{
	readonly object _syncRoot = new object();

	/// <summary>
	/// Exposed so derived types can widen the lock across a check-then-act pair.
	/// Both derived collections reject duplicates before inserting, and doing that
	/// without holding the lock would let two threads pass the check and both insert.
	/// </summary>
	protected object SyncRoot => _syncRoot;

	protected override void InsertItem(int index, T item)
	{
		lock (_syncRoot)
			base.InsertItem(index, item);
	}

	protected override void SetItem(int index, T item)
	{
		lock (_syncRoot)
			base.SetItem(index, item);
	}

	protected override void RemoveItem(int index)
	{
		lock (_syncRoot)
			base.RemoveItem(index);
	}

	protected override void ClearItems()
	{
		lock (_syncRoot)
			base.ClearItems();
	}
}
