using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sero.Mapper
{
    public delegate void TransformationMask<TSrc, TDest>(TSrc src, TDest dest);

    public class MapperBuilder : IMapperBuilder
    {
        private List<MappingHandler> _mappingList;

        public MapperBuilder()
        {
            _mappingList = new List<MappingHandler>();
        }

        public void AddSheet(AbstractMappingSheet sheet)
        {
            sheet.MappingRegistration(this);
        }

        public void CreateMap<TSource, TDestination>(TransformationMask<TSource, TDestination> funcMask)
        {
            var mapping = new MappingHandler(typeof(TSource),
                                      typeof(TDestination),
                                      obj =>
                                      {
                                          TDestination dest = Activator.CreateInstance<TDestination>();
                                          funcMask((TSource)obj, dest);
                                          return dest;
                                      });

            bool isExisting = _mappingList.Any(x => x.SourceType == mapping.SourceType
                                                    && x.DestinationType == mapping.DestinationType);

            if (isExisting)
                throw new Exception("Can't register this mapping. Another one was already registered for this SOURCE-DESTINATION pair.");

            _mappingList.Add(mapping);
        }

        public IMapper Build()
        {
            return new BasicMapper(_mappingList);
        }
    }
}
