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

            Assert.Throws<MappingCollectionDuplicateException>(
                () => builder.CreateMap<SrcModel, DestModel>((src, dest, mapper) => { })
            );
        }

        [Fact]
        public void TransformationWithMapper__CreatesDuplicateMapping__DuplicatedMappingException()
        {
            MapperBuilder builder = new MapperBuilder();
            builder.CreateMap<SrcModel, DestModel>((src, dest, mapper) => { });

            Assert.Throws<MappingCollectionDuplicateException>(
                () => builder.CreateMap<SrcModel, DestModel>((src, dest, mapper) => { })
            );
        }
    }
}
