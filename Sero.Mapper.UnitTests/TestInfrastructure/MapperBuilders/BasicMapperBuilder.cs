using System;
using System.Collections.Generic;
using System.Text;
using Sero.Mapper;

namespace Sero.Mapper.Tests
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
            _mapperbuilder.AddSheet<DefaultSheet>();
            return this;
        }

        internal BasicMapperBuilder WithMapping<TSrc, TDest>(ConvertMutable<TSrc, TDest> transformation)
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
