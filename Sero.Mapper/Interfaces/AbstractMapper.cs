using System;
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

        public TDestination Map<TDestination>(object obj)
        {
            if (obj == null)
                return default(TDestination);

            Type sourceType = obj.GetType();
            Type destinationType = typeof(TDestination);

            var mapping = MappingHandlers.FirstOrDefault(x => x.SourceType == sourceType 
                                                            && x.DestinationType == destinationType);

            if (mapping == null)
                throw new Exception("There is no mapping defined for this SOURCE-DESTINATION pair.");

            var dto = (TDestination)mapping.Transformation.Invoke(obj);
            return dto;
        }
        
        public IList<TDestination> Map<TDestination>(IList<object> objList)
        {
            if (objList == null)
                return null;

            var dstList = new List<TDestination>();

            foreach (var obj in objList)
            {
                var dst = Map<TDestination>(obj);

                if (dst != null)
                    dstList.Add(dst);
            }

            return dstList;
        }

        public IList<TDestination> Map<TDestination>(ICollection<object> objList)
        {
            if (objList == null)
                return null;

            var dstList = new List<TDestination>();

            foreach (var obj in objList)
            {
                var dst = Map<TDestination>(obj);

                if (dst != null)
                    dstList.Add(dst);
            }

            return dstList;
        }
    }
}
