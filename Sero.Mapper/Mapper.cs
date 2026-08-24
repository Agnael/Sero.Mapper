using Microsoft.Extensions.Logging;
using Nito.AsyncEx.Synchronous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sero.Mapper;

/// <summary>
///   Performs type transformations, it's Sero.Mapper's main class. 
/// </summary>
public class Mapper : IMapper
{
	private readonly ILogger _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly IMappingCollection _mappingHandlers;

	public Mapper(
	   ILogger logger,
	   IServiceProvider serviceProvider,
	   IMappingCollection mappingHandlers)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
		_mappingHandlers = mappingHandlers;
	}

	private void CheckNotNull(string argName, object argValue)
	{
		if (argValue == null)
			throw new ArgumentNullException(argName);
	}

	private void CheckNotNull<TValue>(string argName, TValue argValue)
	{
		if (EqualityComparer<TValue>.Default.Equals(argValue, default(TValue)))
			throw new ArgumentNullException(argName);
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private object MapInternal(Type destType, ConvertMutable convertMutable, object srcObj)
	{
		object destObj = Activator.CreateInstance(destType);
		return MapInternal(convertMutable, srcObj, destObj);
	}

	private TDest MapInternal<TDest>(ConvertMutable convertMutable, object srcObj)
	{
		return (TDest)MapInternal(typeof(TDest), convertMutable, srcObj);
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private object MapInternal(ConvertMutable convertMutable, object srcObj, object preexistingDest)
	{
		convertMutable.Invoke(srcObj, preexistingDest, this, _serviceProvider);
		return preexistingDest;
	}

	private TDest MapInternal<TDest>(ConvertMutable convertMutable, object srcObj, TDest preexistingDest)
	{
		return (TDest)MapInternal(convertMutable, srcObj, preexistingDest as object);
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private object MapInternal(
	   ConvertImmutableWithBase convertImmutableWithBase,
	   object srcObj,
	   object preexistingDest)
	{
		convertImmutableWithBase.Invoke(srcObj, this, _serviceProvider, preexistingDest);
		return preexistingDest;
	}

	private TDest MapInternal<TDest>(
	   ConvertImmutableWithBase convertImmutableWithBase,
	   object srcObj,
	   TDest preexistingDest)
	{
		return (TDest)MapInternal(convertImmutableWithBase, srcObj, preexistingDest);
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private object MapInternal(
	   ConvertImmutableWithBaseAsync convertImmutableWithBaseAsync,
	   object srcObj,
	   object preexistingDest)
	{
		Task<object> mappingTask =
		   Task.Run(
			  async () =>
			  {
				  await convertImmutableWithBaseAsync.Invoke(srcObj, this, _serviceProvider, preexistingDest);
				  return preexistingDest;
			  }
		   );

		return mappingTask.WaitAndUnwrapException();
	}

	private TDest MapInternal<TDest>(
	   ConvertImmutableWithBaseAsync convertImmutableWithBaseAsync,
	   object srcObj,
	   TDest preexistingDest)
	{
		return (TDest)MapInternal(convertImmutableWithBaseAsync, srcObj, (object)preexistingDest);
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private object MapInternal(ConvertImmutable convertImmutable, object srcObj)
	{
		return convertImmutable.Invoke(srcObj, this, _serviceProvider);
	}

	private TDest MapInternal<TDest>(ConvertImmutable convertImmutable, object srcObj)
	{
		return (TDest)MapInternal(convertImmutable, srcObj);
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private object MapInternal(Type destType, ConvertMutableAsync convertMutableAsync, object srcObj)
	{
		object destObj = Activator.CreateInstance(destType);
		return MapInternal(convertMutableAsync, srcObj, destObj as object);
	}

	private TDest MapInternal<TDest>(ConvertMutableAsync convertMutableAsync, object srcObj)
	{
		return (TDest)MapInternal(typeof(TDest), convertMutableAsync, srcObj);
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private object MapInternal(
		Type destType,
		ConvertMutableAsync convertMutableAsync,
		object srcObj,
		object preexistingDest)
	{
		Task<object> mappingTask =
		   Task.Run(
			  async () =>
			  {
				  await convertMutableAsync.Invoke(srcObj, preexistingDest, this, _serviceProvider);
				  return preexistingDest;
			  }
		   );

		return mappingTask.WaitAndUnwrapException();
	}

	/// <summary>
	/// Runs an async handler in a blocking, synchronized, way.
	/// </summary>
	private TDest MapInternal<TDest>(
	   ConvertMutableAsync convertMutableAsync,
	   object srcObj,
	   TDest preexistingDest)
	{
		Task<TDest> mappingTask =
		   Task.Run(
			  async () =>
			  {
				  await convertMutableAsync.Invoke(srcObj, preexistingDest as object, this, _serviceProvider);
				  return preexistingDest;
			  }
		   );

		return mappingTask.WaitAndUnwrapException();
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private object MapInternal(ConvertImmutableAsync convertImmutableAsync, object srcObj)
	{
		Task<object> mappingTask =
		   Task.Run(
			  async () =>
			  {
				  object mapped = await convertImmutableAsync.Invoke(srcObj, this, _serviceProvider);
				  return (object)mapped;
			  }
		   );

		return mappingTask.WaitAndUnwrapException();
	}

	/// <summary>
	/// Runs an async handler in a blocking, synchronized, way.
	/// </summary>
	private TDest MapInternal<TDest>(ConvertImmutableAsync convertImmutableAsync, object srcObj)
	{
		return (TDest)MapInternal(convertImmutableAsync, srcObj);
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private async Task<object> MapInternalAsync(Type destType, ConvertMutableAsync convertMutableAsync, object srcObj)
	{
		object destObj = Activator.CreateInstance(destType);
		return await MapInternalAsync(convertMutableAsync, srcObj, destObj);
	}

	private async Task<TDest> MapInternalAsync<TDest>(ConvertMutableAsync convertMutableAsync, object srcObj)
	{
		return (TDest)(await MapInternalAsync(typeof(TDest), convertMutableAsync, srcObj as object));
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private async Task<object> MapInternalAsync(
	   ConvertMutableAsync convertMutableAsync,
	   object srcObj,
	   object existingDestObj)
	{
		await convertMutableAsync.Invoke(srcObj, existingDestObj, this, _serviceProvider);
		return existingDestObj;
	}

	private async Task<TDest> MapInternalAsync<TDest>(
	   ConvertMutableAsync convertMutableAsync,
	   object srcObj,
	   TDest existingDestObj)
	{
		return (TDest)(await MapInternalAsync(convertMutableAsync, srcObj, existingDestObj as object));
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private async Task<object> MapInternalAsync(
	   ConvertImmutableAsync convertImmutableAsync,
	   object srcObj)
	{
		object mapped = await convertImmutableAsync.Invoke(srcObj, this, _serviceProvider);
		return mapped;
	}

	private async Task<TDest> MapInternalAsync<TDest>(
	   ConvertImmutableAsync convertImmutableAsync,
	   object srcObj)
	{
		return (TDest)(await MapInternalAsync(convertImmutableAsync, srcObj));
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private async Task<object> MapInternalAsync(
	   ConvertImmutableWithBaseAsync convertImmutableWithBaseAsync,
	   object srcObj,
	   object existingDestObj)
	{
		object outputObj =
		   await convertImmutableWithBaseAsync.Invoke(srcObj, this, _serviceProvider, existingDestObj);

		return outputObj;
	}

	private async Task<TDest> MapInternalAsync<TDest>(
	   ConvertImmutableWithBaseAsync convertImmutableWithBaseAsync,
	   object srcObj,
	   TDest existingDestObj)
	{
		return (TDest)(await MapInternalAsync(convertImmutableWithBaseAsync, srcObj, existingDestObj as object));
	}

	// --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 
	private string GetMessage_AsyncMappingExecutedSynchronously(MappingHandler mapping)
	{
		return
		   $"The {mapping} was defined as ASYNC when created, but then executed with the thread " +
		   $"blocking {nameof(Map)} method which . You should use {nameof(MapAsync)} instead.";
	}

	public object Map(Type destType, object sourceObj)
	{
		CheckNotNull(nameof(sourceObj), sourceObj);

		Type sourceType = sourceObj.GetType();

		MappingHandler mapping = _mappingHandlers.GetMappingHandler(sourceType, destType);

		if (mapping.PreprocessingMaps.Any())
		{
			object prebuildDest = Activator.CreateInstance(destType);
			foreach (Tuple<Type, Type> preprocessingMap in mapping.PreprocessingMaps)
			{
				prebuildDest =
					Map(
						preprocessingMap.Item1,
						preprocessingMap.Item2,
						prebuildDest
					);
			}

			return Map(destType, sourceObj, prebuildDest);
		}

		return mapping.Converter.Match(
			convertMutable =>
				MapInternal(destType, convertMutable, sourceObj),

			convertImmutable =>
				MapInternal(convertImmutable, sourceObj),

			convertMutableAsync =>
			{
				_logger.LogWarning(GetMessage_AsyncMappingExecutedSynchronously(mapping));
				return MapInternal(destType, convertMutableAsync, sourceObj);
			},

			convertImmutableAsync =>
			{
				_logger.LogWarning(GetMessage_AsyncMappingExecutedSynchronously(mapping));
				return MapInternal(convertImmutableAsync, sourceObj);
			},

			convertImmutableWithBase =>
				throw new InvalidImmutableMappingOperationException(sourceType, destType),

			convertImmutableWithBaseAsync =>
				throw new InvalidImmutableMappingOperationException(sourceType, destType)
		);
	}

	public TDestination Map<TDestination>(object sourceObj)
	{
		return (TDestination)Map(typeof(TDestination), sourceObj);
	}

	public object Map(Type destType, object sourceObj, object existingDestinationObj)
	{
		CheckNotNull(nameof(sourceObj), sourceObj);

		CheckNotNull(nameof(existingDestinationObj), existingDestinationObj);

		Type srcType = sourceObj.GetType();

		MappingHandler mapping = _mappingHandlers.GetMappingHandler(srcType, destType);

		return mapping.Converter.Match(
		   convertMutable =>
			  MapInternal(convertMutable, sourceObj, existingDestinationObj),

		   convertImmutable =>
			  throw new InvalidImmutableMappingOperationException(srcType, destType),

		   convertMutableAsync =>
			  MapInternal(convertMutableAsync, sourceObj, existingDestinationObj),

		   convertImmutableAsync =>
			  throw new InvalidImmutableMappingOperationException(srcType, destType),

		   convertImmutableWithBase =>
			  MapInternal(convertImmutableWithBase, sourceObj, existingDestinationObj),

		   convertImmutableWithBaseAsync =>
			  MapInternal(convertImmutableWithBaseAsync, sourceObj, existingDestinationObj)
		);
	}

	public TDestination Map<TDestination>(object sourceObj, TDestination existingDestinationObj)
	{
		return (TDestination)Map(typeof(TDestination), sourceObj, existingDestinationObj as object);
	}

	public async Task<object> MapAsync(Type destType, object sourceObj)
	{
		CheckNotNull(nameof(sourceObj), sourceObj);

		Type sourceType = sourceObj.GetType();

		MappingHandler mapping = _mappingHandlers.GetMappingHandler(sourceType, destType);

		return await mapping.Converter.Match(
		   async convertMutable =>
			  MapInternal(destType, convertMutable, sourceObj),

		   async convertImmutable =>
			  MapInternal(convertImmutable, sourceObj),

		   async convertMutableAsync =>
			  await MapInternalAsync(destType, convertMutableAsync, sourceObj),

		   async convertImmutableAsync =>
			  await MapInternalAsync(convertImmutableAsync, sourceObj),

		   async convertImmutableWithBase =>
			  throw new InvalidImmutableMappingOperationException(sourceType, destType),

		   async convertImmutableWithBaseAsync =>
			  throw new InvalidImmutableMappingOperationException(sourceType, destType)
		);
	}

	public async Task<TDest> MapAsync<TDest>(object sourceObj)
	{
		return (TDest)(await MapAsync(typeof(TDest), sourceObj));
	}

	public async Task<object> MapAsync(Type destType, object sourceObj, object existingDestObj)
	{
		CheckNotNull(nameof(sourceObj), sourceObj);

		CheckNotNull(nameof(existingDestObj), existingDestObj);

		Type srcType = sourceObj.GetType();

		MappingHandler mapping = _mappingHandlers.GetMappingHandler(srcType, destType);

		return await mapping.Converter.Match(
		   async convertMutable =>
			  MapInternal(convertMutable, sourceObj, existingDestObj),

		   async convertImmutable =>
			  throw new InvalidImmutableMappingOperationException(srcType, destType),

		   async convertMutableAsync =>
			  await MapInternalAsync(convertMutableAsync, sourceObj, existingDestObj),

		   async convertImmutableAsync =>
			  throw new InvalidImmutableMappingOperationException(srcType, destType),

		   async convertImmutableWithBase =>
			  MapInternal(convertImmutableWithBase, sourceObj, existingDestObj),

		   async convertImmutableWithBaseAsync =>
			  await MapInternalAsync(convertImmutableWithBaseAsync, sourceObj, existingDestObj)
		);
	}

	public async Task<TDest> MapAsync<TDest>(object sourceObj, TDest existingDestObj)
	{
		return (TDest)(await MapAsync(typeof(TDest), sourceObj, existingDestObj as object));
	}

	public ICollection<TDest> MapList<TDest>(IEnumerable<object> sourceObjList)
	{
		CheckNotNull(nameof(sourceObjList), sourceObjList);

		if (sourceObjList.Any(x => x == null))
			throw new NullItemsInCollectionException();

		ICollection<TDest> destinationList =
		   sourceObjList
		   .Select(x => Map<TDest>(x))
		   .ToList();

		return destinationList;
	}

	public async Task<ICollection<TDestination>> MapListAsync<TDestination>(IEnumerable<object> sourceObjList)
	{
		CheckNotNull(nameof(sourceObjList), sourceObjList);

		if (sourceObjList.Any(x => x == null))
			throw new NullItemsInCollectionException();

		List<TDestination> results = new List<TDestination>();

		// Awaits tasks one by one in case the mapping lambda contains some thread unsafe stuff (e.g.: EfCore).
		foreach (object srcObj in sourceObjList)
		{
			results.Add(await MapAsync<TDestination>(srcObj));
		}

		return results;
	}
}
