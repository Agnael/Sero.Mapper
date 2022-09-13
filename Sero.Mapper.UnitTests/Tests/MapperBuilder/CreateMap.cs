using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sero.Mapper.Tests.MapperBuilderTests
{
    public class CreateMap
    {
        [Fact]
        public void SimpleTransformation__CreatesDuplicateMapping__DuplicatedMappingException()
        {
            MapperBuilder builder = new MapperBuilder();
            builder.CreateMap<SrcModel, DestModel>((src, dest, mapper) => { });

            Assert.Throws<DuplicatedMappingException<SrcModel, DestModel>>(
                () => builder.CreateMap<SrcModel, DestModel>((src, dest, mapper) => { })
            );
        }

        [Fact]
        public void TransformationWithMapper__CreatesDuplicateMapping__DuplicatedMappingException()
        {
            MapperBuilder builder = new MapperBuilder();
            builder.CreateMap<SrcModel, DestModel>((src, dest, mapper) => { });

            Assert.Throws<DuplicatedMappingException<SrcModel, DestModel>>(
                () => builder.CreateMap<SrcModel, DestModel>((src, dest, mapper) => { })
            );
        }
    }
}
