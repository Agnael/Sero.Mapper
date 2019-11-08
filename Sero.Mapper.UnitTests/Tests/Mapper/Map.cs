using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sero.Mapper.Tests.MapperTests
{
    public class Map : BasicMapperFixture
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("al43errrt")]
        [InlineData("[]A{d}asd}12{3--..")]
        public void Single__Success(string propValue)
        {
            SrcModel src = _srcBuilder.WithName(propValue);
            DestModel expected = _destBuilder.WithName(propValue);

            var sut = _sutBuilder.WithDefaultMapping().Build();
            var actual = sut.Map<DestModel>(src);

            Assert.Equal(expected, actual, _destComparer);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("al43errrt")]
        [InlineData("[]A{d}asd}12{3--..")]
        public void Single__ProvidingDestinationInstanceParameter__Success(string propValue)
        {
            SrcModel src = _srcBuilder
                                .WithId(26)
                                .WithName("originalName")
                                .WithDescription("originalDescription").Build();

            SrcNameModel src2 = new SrcNameModel(propValue);

            DestModel expected = _destBuilder
                                .WithId(26)
                                .WithName(propValue)
                                .WithDescription("originalDescription");

            Mapper sut = _sutBuilder
                            .WithDefaultMapping()
                            .WithMapping<SrcNameModel, DestModel>((source, dest) => 
                            {
                                dest.NameSrc = source.Name;
                            })
                            .Build();

            DestModel actual = sut.Map<DestModel>(src);
            actual = sut.Map<DestModel>(src2, actual);

            Assert.Equal(expected, actual, _destComparer);
        }

        [Fact]
        public void Single__ProvidingDestinationInstanceParameterAsNull__ArgumentNullException()
        {
            SrcModel src = _srcBuilder;
            Mapper sut = _sutBuilder.WithDefaultMapping().Build();
            Assert.Throws<ArgumentNullException>(() => sut.Map<DestModel>(src, null));
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("al43errrt", "asdjj3")]
        [InlineData("[]A{d}asd}12{3--..", "km342}}{}][--..")]
        public void Single__WithInternalMapperUsage__Success(string wrapperName, string internalName)
        {
            ComplexSrcModel src = _complexSrcBuilder
                                        .WithName(wrapperName)
                                        .WithInternal(interno => interno.WithName(internalName));

            ComplexDestModel expected = _complexDestBuilder
                                        .WithName(wrapperName)
                                        .WithInternal(interno => interno.WithName(internalName));

            Mapper sut = _sutBuilder.WithDefaultMapping().Build();

            ComplexDestModel actual = sut.Map<ComplexDestModel>(src);

            Assert.Equal(expected, actual, _complexDestComparer);
        }

        [Fact]
        public void Single__Null__ArgumentNullException()
        {
            Mapper sut = _sutBuilder.WithDefaultMapping().Build();
            Assert.Throws<ArgumentNullException>(() => sut.Map<DestModel>(null));
        }

        [Fact]
        public void Single__UnmappedTransformation__MissingMappingException()
        {
            SrcModel src = _srcBuilder;
            Mapper sut = _sutBuilder.Build();
            Assert.Throws<MissingMappingException>(() => sut.Map<DestModel>(src));
        }
    }
}
