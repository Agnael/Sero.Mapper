using System;
using System.Collections.Generic;
using System.Text;
using Sero.Mapper;
using Sero.Mapper.UnitTests.TestInfrastructure.Sheets;

namespace Sero.Mapper.UnitTests
{
    internal class BasicMapperBuilder
    {
        private MapperBuilder _mapperbuilder;

        public BasicMapperBuilder()
        {
            _mapperbuilder = new MapperBuilder();
        }

        internal BasicMapperBuilder WithDefaultMapping()
        {
            _mapperbuilder.AddSheet<DefaultTestMappings>();
            return this;
        }

        internal BasicMapperBuilder WithMapping<TSrc, TDest>(TransformationMask<TSrc, TDest> transformation)
        {
            _mapperbuilder.CreateMap<TSrc, TDest>(transformation);
            return this;
        }

        internal Mapper Build()
        {
            return _mapperbuilder.Build();
        }
    }
}
