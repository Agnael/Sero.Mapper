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

   private TDest MapInternal<TDest>(ConvertMutable convertMutable, object srcObj)
   {
      TDest destObj = Activator.CreateInstance<TDest>();
      return MapInternal(convertMutable, srcObj, destObj);
   }

   private TDest MapInternal<TDest>(ConvertMutable convertMutable, object srcObj, TDest preexistingDest)
   {
      convertMutable.Invoke(srcObj, preexistingDest, this, _serviceProvider);
      return preexistingDest;
   }

   private TDest MapInternal<TDest>(
      ConvertImmutableWithBase convertImmutableWithBase, 
      object srcObj, 
      TDest preexistingDest)
   {
      convertImmutableWithBase.Invoke(srcObj, this, _serviceProvider, preexistingDest);
      return preexistingDest;
   }

   private TDest MapInternal<TDest>(
      ConvertImmutableWithBaseAsync convertImmutableWithBaseAsync,
      object srcObj,
      TDest preexistingDest)
   {
      Task<TDest> mappingTask =
         Task.Run(
            async () =>
            {
               await convertImmutableWithBaseAsync.Invoke(srcObj, this, _serviceProvider, preexistingDest);
               return preexistingDest;
            }
         );

      return mappingTask.WaitAndUnwrapException();
   }

   private TDest MapInternal<TDest>(ConvertImmutable convertImmutable, object srcObj)
   {
      return (TDest)convertImmutable.Invoke(srcObj, this, _serviceProvider);
   }

   private TDest MapInternal<TDest>(ConvertMutableAsync convertMutableAsync, object srcObj)
   {
      TDest destObj = Activator.CreateInstance<TDest>();
      return MapInternal<TDest>(convertMutableAsync, srcObj, destObj);
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
               await convertMutableAsync.Invoke(srcObj, preexistingDest, this, _serviceProvider);
               return preexistingDest;
            }
         );

      return mappingTask.WaitAndUnwrapException();
   }

   /// <summary>
   /// Runs an async handler in a blocking, synchronized, way.
   /// </summary>
   private TDest MapInternal<TDest>(ConvertImmutableAsync convertImmutableAsync, object srcObj)
   {
      Task<TDest> mappingTask =
         Task.Run(
            async () =>
            {
               object mapped = await convertImmutableAsync.Invoke(srcObj, this, _serviceProvider);
               return (TDest)mapped;
            }
         );

      return mappingTask.WaitAndUnwrapException();
   }

   private async Task<TDest> MapInternalAsync<TDest>(ConvertMutableAsync convertMutableAsync, object srcObj)
   {
      TDest destObj = Activator.CreateInstance<TDest>();
      return await MapInternalAsync(convertMutableAsync, srcObj, destObj);
   }

   private async Task<TDest> MapInternalAsync<TDest>(
      ConvertMutableAsync convertMutableAsync,
      object srcObj,
      TDest existingDestObj)
   {
      await convertMutableAsync.Invoke(srcObj, existingDestObj, this, _serviceProvider);
      return existingDestObj;
   }

   private async Task<TDest> MapInternalAsync<TDest>(
      ConvertImmutableAsync convertImmutableAsync, 
      object srcObj)
   {
      object mapped = await convertImmutableAsync.Invoke(srcObj, this, _serviceProvider);
      return (TDest)mapped;
   }

   private async Task<TDest> MapInternalAsync<TDest>(
      ConvertImmutableWithBaseAsync convertImmutableWithBaseAsync,
      object srcObj,
      TDest existingDestObj)
   {
      object outputObj =
         await convertImmutableWithBaseAsync.Invoke(srcObj, this, _serviceProvider, existingDestObj);

      return (TDest)outputObj;
   }

   private string GetMessage_AsyncMappingExecutedSynchronously(MappingHandler mapping)
   {
      return
         $"The {mapping} was defined as ASYNC when created, but then executed with the thread " +
         $"blocking {nameof(Map)} method which . You should use {nameof(MapAsync)} instead.";
   }

   public TDestination Map<TDestination>(object sourceObj) 
      where TDestination : class
   {
      if (sourceObj == null)
         return (TDestination)null;

      Type sourceType = sourceObj.GetType();
      Type destinationType = typeof(TDestination);

      MappingHandler mapping = _mappingHandlers.GetMappingHandler(sourceType, destinationType);

      return mapping.Converter.Match(
         convertMutable => 
            MapInternal<TDestination>(convertMutable, sourceObj),

         convertImmutable => 
            MapInternal<TDestination>(convertImmutable, sourceObj),

         convertMutableAsync =>
         {
            _logger.LogWarning(GetMessage_AsyncMappingExecutedSynchronously(mapping));
            return MapInternal<TDestination>(convertMutableAsync, sourceObj);
         },

         convertImmutableAsync => 
         {
            _logger.LogWarning(GetMessage_AsyncMappingExecutedSynchronously(mapping));
            return MapInternal<TDestination>(convertImmutableAsync, sourceObj);
         },

         convertImmutableWithBase =>
            throw new InvalidImmutableMappingOperationException(sourceType, destinationType),

         convertImmutableWithBaseAsync =>
            throw new InvalidImmutableMappingOperationException(sourceType, destinationType)
      );
   }

   public TDestination Map<TDestination>(object sourceObj, TDestination existingDestinationObj)
      where TDestination : class
   {
      if (sourceObj == null)
         return null;

      if (existingDestinationObj == null)
         return Map<TDestination>(sourceObj);

      Type srcType = sourceObj.GetType();
      Type destType = typeof(TDestination);

      MappingHandler mapping = _mappingHandlers.GetMappingHandler(srcType, destType);

      return mapping.Converter.Match(
         convertMutable                => 
            MapInternal(convertMutable, sourceObj, existingDestinationObj),

         convertImmutable              => 
            throw new InvalidImmutableMappingOperationException(srcType, destType),

         convertMutableAsync           => 
            MapInternal(convertMutableAsync, sourceObj, existingDestinationObj),

         convertImmutableAsync         => 
            throw new InvalidImmutableMappingOperationException(srcType, destType),

         convertImmutableWithBase      =>
            MapInternal(convertImmutableWithBase, sourceObj, existingDestinationObj),

         convertImmutableWithBaseAsync =>
            MapInternal(convertImmutableWithBaseAsync, sourceObj, existingDestinationObj)
      );
   }

   public async Task<TDest> MapAsync<TDest>(object sourceObj)
      where TDest : class
   {
      if (sourceObj == null) 
         return null;

      Type sourceType = sourceObj.GetType();
      Type destinationType = typeof(TDest);

      MappingHandler mapping = _mappingHandlers.GetMappingHandler(sourceType, destinationType);

      return await mapping.Converter.Match(
         async convertMutable                => 
            MapInternal<TDest>(convertMutable, sourceObj),

         async convertImmutable              => 
            MapInternal<TDest>(convertImmutable, sourceObj),

         async convertMutableAsync           => 
            await MapInternalAsync<TDest>(convertMutableAsync, sourceObj),

         async convertImmutableAsync         => 
            await MapInternalAsync<TDest>(convertImmutableAsync, sourceObj),

         async convertImmutableWithBase      =>
            throw new InvalidImmutableMappingOperationException(sourceType, destinationType),

         async convertImmutableWithBaseAsync => 
            throw new InvalidImmutableMappingOperationException(sourceType, destinationType)
      );
   }

   public async Task<TDest> MapAsync<TDest>(object sourceObj, TDest existingDestObj)
      where TDest : class
   {
      if (sourceObj == null) 
         return null;

      if (existingDestObj == null)
         return await MapAsync<TDest>(sourceObj);

      Type srcType = sourceObj.GetType();
      Type destType = typeof(TDest);

      MappingHandler mapping = _mappingHandlers.GetMappingHandler(srcType, destType);

      return await mapping.Converter.Match(
         async convertMutable => 
            MapInternal<TDest>(convertMutable, sourceObj, existingDestObj),

         async convertImmutable => 
            throw new InvalidImmutableMappingOperationException(srcType, destType),

         async convertMutableAsync => 
            await MapInternalAsync(convertMutableAsync, sourceObj, existingDestObj),

         async convertImmutableAsync => 
            throw new InvalidImmutableMappingOperationException(srcType, destType),

         async convertImmutableWithBase =>
            MapInternal<TDest>(convertImmutableWithBase, sourceObj, existingDestObj),

         async convertImmutableWithBaseAsync =>
            await MapInternalAsync<TDest>(convertImmutableWithBaseAsync, sourceObj, existingDestObj)
      );
   }

   public ICollection<TDest> MapList<TDest>(IEnumerable<object> sourceObjList)
      where TDest : class
   {
      ICollection<TDest> destinationList =
         sourceObjList
         .Select(x => Map<TDest>(x))
         .ToList();

      return destinationList;
   }

   public async Task<ICollection<TDestination>> MapListAsync<TDestination>(IEnumerable<object> sourceObjList)
      where TDestination : class
   {
      List<TDestination> results = new List<TDestination>();

      // Awaits tasks one by one in case the mapping lambda contains some thread unsafe stuff (e.g.: EfCore).
      foreach (object srcObj in sourceObjList)
      {
         results.Add(await MapAsync<TDestination>(srcObj));
      }

      return results;
   }
}
