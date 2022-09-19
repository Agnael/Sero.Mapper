using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sero.Mapper;

public interface IMapper
{
   /// <summary>
   ///     Transforms an object instance into a new instance of a different type.
   /// </summary>
   /// <typeparam name="TDestination">
   ///     The desired destination type.
   /// </typeparam>
   /// <param name="sourceObj">
   ///     The source instance that will be used to create a destination type instance.
   /// </param>
   /// <returns>
   ///     An instance of the desired destination type, created according to the defined transformation
   /// </returns>
   /// <exception cref="System.ArgumentNullException">
   ///     Thrown when any of the parameters is null.
   /// </exception>
   /// <exception cref="Sero.Mapper.MissingMappingException">
   ///     Thrown when trying to map a SOURCE-DESTINATION pair that has no transformation registered.
   /// </exception>
   TDestination Map<TDestination>(object sourceObj);
   Task<TDestination> MapAsync<TDestination>(object sourceObj);

   /// <summary>
   ///     Executes a transformation from a source instance into a an existing destination instance, 
   /// overwritting only the affected fields.
   /// </summary>
   /// <typeparam name="TDestination">
   ///     The desired destination type
   /// </typeparam>
   /// <param name="sourceObj">
   ///     The source instance that will be used to overwrite the provided destination type instance.
   /// </param>
   /// <param name="existingDestinationObj">
   ///     The already existing destination type instance to be overwritten.
   /// </param>
   /// <exception cref="System.ArgumentNullException">
   ///     Thrown when any of the parameters is null.
   /// </exception>
   /// <exception cref="Sero.Mapper.MissingMappingException">
   ///     Thrown when trying to map a SOURCE-DESTINATION pair that has no transformation registered.
   /// </exception>
   /// <returns>
   ///     The provided destination instance with the transformation applied.
   /// </returns>
   TDestination Map<TDestination>(object sourceObj, TDestination existingDestinationObj);
   Task<TDestination> MapAsync<TDestination>(object sourceObj, TDestination existingDestinationObj);

   /// <summary>
   ///     Returns an ICollection with one destination type instance per element in the received source instance IEnumerable.
   /// </summary>
   /// <typeparam name="TDestination">
   ///     The destination type desired for each element of the returned ICollection.
   /// </typeparam>
   /// <param name="sourceObjList">
   ///     An IEnumerable of source objects.
   /// </param>
   /// <returns>
   ///     An ICollection of the desired type instances, created according to the defined transformation.
   /// </returns>
   /// <exception cref="System.ArgumentNullException">
   ///     Thrown when any of the parameters is null.
   /// </exception>
   /// <exception cref="System.NullItemsInCollectionException">
   ///     Thrown when any of the "sourceObjList" collection elements is null.
   /// </exception>
   /// <exception cref="Sero.Mapper.MissingMappingException">
   ///     Thrown when trying to map a SOURCE-DESTINATION pair that has no transformation registered.
   /// </exception>
   ICollection<TDestination> MapList<TDestination>(IEnumerable<object> sourceObjList);
   Task<ICollection<TDestination>> MapListAsync<TDestination>(IEnumerable<object> sourceObjList);
}
