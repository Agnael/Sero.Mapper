using Functional.Maybe;
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
   public readonly IMappingCollection<MappingHandler> MappingHandlers;

   public Mapper(IMappingCollection<MappingHandler> mappingHandlers)
   {
      this.MappingHandlers = mappingHandlers;
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

   private TDest MapInternal<TDest>(ConvertMutable convertMutable, object srcObj)
   {
      TDest destObj = Activator.CreateInstance<TDest>();
      return MapInternal(convertMutable, srcObj, destObj);
   }

   private TDest MapInternal<TDest>(ConvertMutable convertMutable, object srcObj, TDest preexistingDest)
   {
      convertMutable.Invoke(srcObj, preexistingDest, this);
      return preexistingDest;
   }

   private TDest MapInternal<TDest>(ConvertImmutable convertImmutable, object srcObj)
   {
      return (TDest)convertImmutable.Invoke(srcObj, this);
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
               await convertMutableAsync.Invoke(srcObj, preexistingDest, this);
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
               object mapped = await convertImmutableAsync.Invoke(srcObj, this);
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
      await convertMutableAsync.Invoke(srcObj, existingDestObj, this);
      return existingDestObj;
   }

   private async Task<TDest> MapInternalAsync<TDest>(
      ConvertImmutableAsync convertImmutableAsync, 
      object srcObj)
   {
      object mapped = await convertImmutableAsync.Invoke(srcObj, this);
      return (TDest)mapped;
   }

   public TDestination Map<TDestination>(object sourceObj)
   {
      CheckNotNull(nameof(sourceObj), sourceObj);

      Type sourceType = sourceObj.GetType();
      Type destinationType = typeof(TDestination);

      MappingHandler mapping = MappingHandlers.GetMappingHandler(sourceType, destinationType);

      return mapping.Converter.Match(
         convertMutable          => MapInternal<TDestination>(convertMutable, sourceObj),
         convertImmutable        => MapInternal<TDestination>(convertImmutable, sourceObj),
         convertMutableAsync     => MapInternal<TDestination>(convertMutableAsync, sourceObj),
         convertImmutableAsync   => MapInternal<TDestination>(convertImmutableAsync, sourceObj)
      );
   }

   public TDestination Map<TDestination>(object sourceObj, TDestination existingDestinationObj)
   {
      CheckNotNull(nameof(sourceObj), sourceObj);
      CheckNotNull(nameof(existingDestinationObj), existingDestinationObj);

      Type srcType = sourceObj.GetType();
      Type destType = typeof(TDestination);

      MappingHandler mapping = MappingHandlers.GetMappingHandler(srcType, destType);

      return mapping.Converter.Match(
         convertMutable          => MapInternal(convertMutable, sourceObj, existingDestinationObj),
         convertImmutable        => throw new InvalidImmutableMappingOperationException(srcType, destType),
         convertMutableAsync     => MapInternal(convertMutableAsync, sourceObj, existingDestinationObj),
         convertImmutableAsync   => throw new InvalidImmutableMappingOperationException(srcType, destType)
      );
   }

   public async Task<TDest> MapAsync<TDest>(object sourceObj)
   {
      CheckNotNull(nameof(sourceObj), sourceObj);

      Type sourceType = sourceObj.GetType();
      Type destinationType = typeof(TDest);

      MappingHandler mapping = MappingHandlers.GetMappingHandler(sourceType, destinationType);

      return await mapping.Converter.Match(
         async convertMutable          => MapInternal<TDest>(convertMutable, sourceObj),
         async convertImmutable        => MapInternal<TDest>(convertImmutable, sourceObj),
         async convertMutableAsync     => await MapInternalAsync<TDest>(convertMutableAsync, sourceObj),
         async convertImmutableAsync   => await MapInternalAsync<TDest>(convertImmutableAsync, sourceObj)
      );
   }

   public async Task<TDest> MapAsync<TDest>(
      object sourceObj, 
      TDest existingDestObj)
   {
      CheckNotNull(nameof(sourceObj), sourceObj);
      CheckNotNull(nameof(existingDestObj), existingDestObj);

      Type srcType = sourceObj.GetType();
      Type destType = typeof(TDest);

      MappingHandler mapping = MappingHandlers.GetMappingHandler(srcType, destType);

      return await mapping.Converter.Match(
         async convertMutable => 
            MapInternal<TDest>(convertMutable, sourceObj, existingDestObj),

         async convertImmutable => 
            throw new InvalidImmutableMappingOperationException(srcType, destType),

         async convertMutableAsync => 
            await MapInternalAsync(convertMutableAsync, sourceObj, existingDestObj),

         async convertImmutableAsync => 
            throw new InvalidImmutableMappingOperationException(srcType, destType)
      );
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

      List<Task<TDestination>> resultTasks =
         sourceObjList
         .Select(x => MapAsync<TDestination>(x))
         .ToList();

      await Task.WhenAll(resultTasks);

      return resultTasks.Select(x => x.Result).ToList();
   }
}
