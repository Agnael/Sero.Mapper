using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

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
   ///   Convenience method to avoid repeated code.
   /// </summary>
   private TDest MapInternal<TDest>(object src, object destBase)
   {
      Type sourceType = src.GetType();
      Type destinationType = typeof(TDest);

      var mapping = 
         MappingHandlers
         .FirstOrDefault(
            mapping => mapping.SourceType == sourceType && 
            mapping.DestinationType == destinationType
         );

      if (mapping == null)
         throw new MissingMappingException(sourceType, destinationType);

      // Final destination instance, with the user's transformation applied
      bool isMutableConverterFound = 
         mapping.Converter.TryPickT0(
            out ConvertMutable convertMutable,
            out ConvertImmutable convertImmutable
         );

      if (!isMutableConverterFound)
         throw new MutableConverterNotFoundException(sourceType, destinationType);

      convertMutable.Invoke(src, destBase, this);

      return (TDest)destBase;
   }

   /// <summary>
   ///   Convenience method to avoid repeated code.
   /// </summary>
   private TDestination MapInternal<TDestination>(object src)
   {
      Type sourceType = src.GetType();
      Type destinationType = typeof(TDestination);

      var mapping =
         MappingHandlers
         .FirstOrDefault(
            mapping => mapping.SourceType == sourceType &&
            mapping.DestinationType == destinationType
         );

      if (mapping == null)
         throw new MissingMappingException(sourceType, destinationType);

      // Final destination instance, with the user's transformation applied
      bool isImmutableConverterFound =
         mapping.Converter.TryPickT1(
            out ConvertImmutable convertImmutable,
            out ConvertMutable convertMutable
         );

      if (convertMutable == null && convertImmutable == null)
         throw new MissingMappingException(sourceType, destinationType);

      if (!isImmutableConverterFound)
      {
         // TODO: Right now, if the user didn't provide a mutable base object to map over, we assume we MUST
         // then need an immutable converter, which is not necessarily the case. We'd need to refactor this
         // converter selection process avoiding double checks.
         TDestination dest = Activator.CreateInstance<TDestination>();
         convertMutable.Invoke(src, dest, this);

         return dest;
      }

      return (TDestination)convertImmutable.Invoke(src, this);
   }

   public TDestination Map<TDestination>(object sourceObj)
   {
      if (sourceObj == null)
         throw new ArgumentNullException(nameof(sourceObj));

      TDestination destination = this.MapInternal<TDestination>(sourceObj);
      return destination;
   }

   public TDestination Map<TDestination>(object sourceObj, TDestination existingDestinationObj)
   {
      if (sourceObj == null)
         throw new ArgumentNullException(nameof(sourceObj));

      if (EqualityComparer<TDestination>.Default.Equals(existingDestinationObj, default(TDestination)))
         throw new ArgumentNullException(nameof(existingDestinationObj));

      TDestination dto = this.MapInternal<TDestination>(sourceObj, existingDestinationObj);
      return dto;
   }

   public ICollection<TDestination> MapList<TDestination>(IEnumerable<object> sourceObjList)
   {
      if (sourceObjList == null)
         throw new ArgumentNullException(nameof(sourceObjList));

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
