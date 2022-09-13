using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sero.Mapper;

/// <summary>
///     Registers and holds mapping transformations to finally create a Mapper instance with them.
/// </summary>
public class MapperBuilder
{
   private List<MappingHandler> _mappingList;

   public MapperBuilder()
   {
      _mappingList = new List<MappingHandler>();
   }

   /// <summary>
   ///     Takes an IMappingSheet implementing class and registers it's mapping definitions.
   /// </summary>
   /// <param name="sheet">
   ///     IMappingSheet implementing class.
   /// </param>
   public MapperBuilder AddSheet(IMappingSheet sheet)
   {
      sheet.MappingRegistration(this);
      return this;
   }

   /// <summary>
   ///     Convenience method that only needs the IMappingSheet implementation class name to register it's definitions.
   /// </summary>
   /// <typeparam name="T">
   ///     A class that implements IMappingSheet.
   /// </typeparam>
   public MapperBuilder AddSheet<T>() where T : IMappingSheet
   {
      IMappingSheet instance = Activator.CreateInstance<T>();
      return AddSheet(instance);
   }

   /// <summary>
   ///   Creates a single mapping definition for the SOURCE-DESTINATION types provided.
   ///   This method overload allows you to register a transformation lambda that receives the current 
   ///   Mapper instance, to execute mappings in the lambda.
   /// </summary>
   /// <typeparam name="TSource">
   ///   Source type that the transformation should receive.
   /// </typeparam>
   /// <typeparam name="TDestination">
   ///   Destination type that the transformation must return.
   /// </typeparam>
   /// <param name="funcMask">
   ///   Lambda that takes 3 parameters: (TSource src, TDestination dest, Mapper mapper) and defines the
   ///   transformation you need. It must not return the resulting transformed "dest" parameter, the 
   ///   library takes care of that.
   /// </param>
   public MapperBuilder CreateMap<TSource, TDestination>(
      ConvertMutable<TSource, TDestination> funcMask)
   {
      MappingHandler newMapping = MakeMappingHandler<TSource, TDestination>(funcMask);

      bool isDuplicate =
         _mappingList
         .Any(
            existingMapping =>
               existingMapping.SourceType == newMapping.SourceType &&
               existingMapping.DestinationType == newMapping.DestinationType
         );

      if (isDuplicate)
      {
         throw new DuplicatedMappingException<TSource, TDestination>();
      }

      _mappingList.Add(newMapping);

      return this;
   }

   /// <summary>
   ///   Creates a single mapping definition for the SOURCE-DESTINATION types provided.
   /// </summary>
   /// <typeparam name="TSource">
   ///   Source type that the transformation should receive.
   /// </typeparam>
   /// <typeparam name="TDestination">
   ///   Destination type that the transformation must return.
   /// </typeparam>
   /// <param name="funcMask">
   ///   Lambda that takes 3 parameters: (TSource src, TDestination dest) and defines the transformation 
   ///   you need. It must not return the resulting transformed "dest" parameter, the library takes care 
   ///   of that.
   /// </param>
   public MapperBuilder CreateMap<TSource, TDestination>(ConvertImmutable<TSource, TDestination> funcMask)
   {
      MappingHandler newMapping = MakeMappingHandler<TSource, TDestination>(funcMask);

      bool isDuplicate =
         _mappingList
         .Any(
            existingMapping =>
               existingMapping.SourceType == newMapping.SourceType &&
               existingMapping.DestinationType == newMapping.DestinationType
         );

      if (isDuplicate)
      {
         throw new DuplicatedMappingException<TSource, TDestination>();
      }

      _mappingList.Add(newMapping);

      return this;
   }

   /// <summary>
   ///   Builds and returns a MappingHandler instance. Convenience method to avoid repeating code.
   /// </summary>
   private MappingHandler MakeMappingHandler<TSrc, TDest>(
      OneOf<ConvertMutable<TSrc, TDest>, ConvertImmutable<TSrc, TDest>> converter)
   {
      MappingHandler mappingHandler =
         converter
         .Match(
            convertMutable =>
            {
               return new MappingHandler(
                  typeof(TSrc),
                  typeof(TDest),
                  (src, dest, mapper) => convertMutable.Invoke((TSrc)src, (TDest)dest, mapper)
               );
            },
            convertImmutable =>
            {
               return new MappingHandler(
                  typeof(TSrc),
                  typeof(TDest),
                  (src, mapper) => convertImmutable.Invoke((TSrc)src, mapper)
               );
            }
         );

      return mappingHandler;
   }

   /// <summary>
   ///   Uses the MapperBuilder configurations to build a Mapper instance.
   /// </summary>
   public Mapper Build()
   {
      return new Mapper(_mappingList);
   }
}
