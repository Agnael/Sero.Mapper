using System;
using System.Collections.Generic;
using System.Linq;

namespace Sero.Mapper;

/// <summary>
///   Performs type transformations, it's Sero.Mapper's main class. 
/// </summary>
public class Mapper : IMapper
{
   public IEnumerable<MappingHandler> MappingHandlers { get; protected set; }

   public Mapper(IEnumerable<MappingHandler> mappingHandlers)
   {
      this.MappingHandlers = mappingHandlers;
   }

   /// <summary>
   ///   Convenience method to avoid repeated code
   /// </summary>
   private TDestination MapInternal<TDestination>(object src, object destBase)
   {
      Type sourceType = src.GetType();
      Type destinationType = typeof(TDestination);

      var mapping = MappingHandlers.FirstOrDefault(x => x.SourceType == sourceType
                                                      && x.DestinationType == destinationType);

      if (mapping == null)
         throw new MissingMappingException(sourceType, destinationType);

      // Final destination instance, with the user's transformation applied
      TDestination destination = (TDestination)mapping.Transformation.Invoke(this, src, destBase);

      return destination;
   }

   public TDestination Map<TDestination>(object sourceObj)
   {
      if (sourceObj == null)
         throw new ArgumentNullException("sourceObj");

      // Since the user is not trying to override an existing destination instance, we have to provide it.
      // Having this instance allows to inject it into user's transformation lambda, helping to
      // reduce boilerplate code.
      TDestination destinationBase = Activator.CreateInstance<TDestination>();
      TDestination destination = this.MapInternal<TDestination>(sourceObj, destinationBase);

      return destination;
   }

   public TDestination Map<TDestination>(object sourceObj, TDestination existingDestinationObj)
   {
      if (sourceObj == null)
         throw new ArgumentNullException("sourceObj");

      if (EqualityComparer<TDestination>.Default.Equals(existingDestinationObj, default(TDestination)))
         throw new ArgumentNullException("existingDestinationObj");

      TDestination dto = this.MapInternal<TDestination>(sourceObj, existingDestinationObj);
      return dto;
   }

   public ICollection<TDestination> MapList<TDestination>(IEnumerable<object> sourceObjList)
   {
      if (sourceObjList == null)
         throw new ArgumentNullException("sourceObj");

      var destinationList = new List<TDestination>();

      foreach (var sourceObj in sourceObjList)
      {
         // If any of the collection elements is null, Sero.Mapper won't take any responsibility for
         // it and throw right then and there.
         try
         {
            var destination = Map<TDestination>(sourceObj);
            destinationList.Add(destination);
         }
         catch (ArgumentNullException ex)
         {
            throw new NullItemsInCollectionException();
         }
      }

      return destinationList;
   }
}
