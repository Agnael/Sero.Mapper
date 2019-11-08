using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sero.Mapper
{
    /// <summary>
    /// Performs type transformations, it's Sero.Mapper's main class. 
    /// </summary>
    public class Mapper
    {
        public IEnumerable<MappingHandler> MappingHandlers { get; protected set; }

        public Mapper(IEnumerable<MappingHandler> mappingHandlers)
        {
            this.MappingHandlers = mappingHandlers;
        }

        /// <summary>
        ///     Convenience method to avoid repeated code
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
        public TDestination Map<TDestination>(object sourceObj)
        {
            if (sourceObj == null)
                throw new ArgumentNullException("sourceObj");
            
            // Since the user is not trying to override an existing destination instance, we have to provide it.
            // Having this instance allows to inject it into user's transformation lambda, helping to reduce boilerplate code.
            TDestination destinationBase = Activator.CreateInstance<TDestination>();
            TDestination destination = this.MapInternal<TDestination>(sourceObj, destinationBase);

            return destination;
        }

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
        public TDestination Map<TDestination>(object sourceObj, TDestination existingDestinationObj)
        {
            if (sourceObj == null)
                throw new ArgumentNullException("sourceObj");

            if (EqualityComparer<TDestination>.Default.Equals(existingDestinationObj, default(TDestination)))
                throw new ArgumentNullException("existingDestinationObj");

            TDestination dto = this.MapInternal<TDestination>(sourceObj, existingDestinationObj);
            return dto;
        }

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
        public ICollection<TDestination> MapList<TDestination>(IEnumerable<object> sourceObjList)
        {
            if (sourceObjList == null)
                throw new ArgumentNullException("sourceObj");

            var destinationList = new List<TDestination>();

            foreach (var sourceObj in sourceObjList)
            {                
                // If any of the collection elements is null, Sero.Mapper won't take any responsibility for it and
                // throw right then and there
                try
                {
                    var destination = Map<TDestination>(sourceObj);
                    destinationList.Add(destination);
                }
                catch(ArgumentNullException ex)
                {
                    throw new NullItemsInCollectionException();
                }
            }

            return destinationList;
        }
    }
}
