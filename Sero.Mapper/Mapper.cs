using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sero.Mapper
{
    public static class Mapper
    {
        private static List<MappingHandler> _mappingList;

        static Mapper()
        {
            _mappingList = new List<MappingHandler>();
        }

        public static void CreateMap<TSource, TDestination>(Func<TSource, TDestination> func)
        {
            var mapping = new MappingHandler(typeof(TSource),
                                      typeof(TDestination),
                                      f => func((TSource)f));

            bool isExisting = _mappingList.Any(x => x.SourceType == mapping.SourceType
                                                    && x.DestinationType == mapping.DestinationType);

            if (isExisting)
                throw new Exception("Can't register this mapping. Another one was already registered for this SOURCE-DESTINATION pair.");

            _mappingList.Add(mapping);
        }

        public static TDestination Map<TDestination>(object obj)
        {
            if (obj == null)
                return default(TDestination);

            Type sourceType = obj.GetType();
            Type destinationType = typeof(TDestination);

            var mapping = _mappingList.FirstOrDefault(x => x.SourceType == sourceType && x.DestinationType == destinationType);

            if (mapping == null)
                throw new Exception("There is no mapping defined for this SOURCE-DESTINATION pair.");

            var dto = (TDestination)mapping.Transform.Invoke(obj);
            return dto;
        }

        public static IList<TDestination> Map<TDestination>(IList<object> objList)
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

        public static IList<TDestination> Map<TDestination>(ICollection<object> objList)
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
