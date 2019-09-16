using System;
using System.Collections.Generic;
using System.Text;
using Sero.Mapper;

namespace Sero.Mapper.UnitTests.Tests.Mappers.BasicMapper
{
    internal class BasicMapperBuilder
    {
        private IMapperBuilder _mapperbuilder;

        public BasicMapperBuilder()
        {
            _mapperbuilder = new MapperBuilder();
        }

        internal BasicMapperBuilder WithMapping<TSrc, TDest>(TransformationMask<TSrc, TDest> transformation)
        {
            _mapperbuilder.CreateMap<TSrc, TDest>(transformation);
            return this;
        }

        internal IMapper Build()
        {
            return _mapperbuilder.Build();
        }
    }
}
