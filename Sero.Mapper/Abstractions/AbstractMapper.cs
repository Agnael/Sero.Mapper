using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sero.Mapper
{
    public abstract class AbstractMapper : IMapper
    {
        public IEnumerable<MappingHandler> MappingHandlers { get; protected set; }

        public AbstractMapper(IEnumerable<MappingHandler> mappingHandlers)
        {
            this.MappingHandlers = mappingHandlers;
        }

        public TDestination Map<TDestination>(object obj, 
                                            TDestination existingDestination = default(TDestination))
        {
            if (obj == null)
                return default(TDestination);

            Type sourceType = obj.GetType();
            Type destinationType = typeof(TDestination);

            var mapping = MappingHandlers.FirstOrDefault(x => x.SourceType == sourceType
                                                            && x.DestinationType == destinationType);

            if (mapping == null)
                throw new Exception("There is no mapping defined for this SOURCE-DESTINATION pair.");

            // If the existingDestination parameter was not passed, instantiate a new one
            if(EqualityComparer<TDestination>.Default.Equals(existingDestination, default(TDestination)))
            {
                existingDestination = Activator.CreateInstance<TDestination>();
            }

            TDestination dto = (TDestination)mapping.Transformation.Invoke(obj, existingDestination);
            return dto;
        }

        public ICollection<TDestination> MapList<TDestination>(IEnumerable<object> objList)
        {
            if (objList == null)
                return null;

            var dstList = new List<TDestination>();

            foreach (var obj in objList)
            {
                var dst = Map<TDestination>(obj);
                dstList.Add(dst);
            }

            return dstList;
        }
    }
}
