using Sero.Mapper.UnitTests.Comparers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sero.Mapper.UnitTests.Mappers.BasicMapper
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
            SrcTest src = _srcBuilder;
            DestTest expected = _destBuilder;

            var sut = _sutBuilder.WithDefaultMapping().Build();
            var actual = sut.Map<DestTest>(src);

            Assert.Equal(expected, actual, _destComparer);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("al43errrt")]
        [InlineData("[]A{d}asd}12{3--..")]
        public void Single__ProvidingDestinationInstanceParameter__Success(string propValue)
        {
            SrcTest src = _srcBuilder
                                .WithId(26)
                                .WithName("originalName")
                                .WithDescription("originalDescription").Build();

            SrcNameTest src2 = new SrcNameTest(propValue);

            DestTest expected = _destBuilder
                                .WithId(26)
                                .WithName(propValue)
                                .WithDescription("originalDescription");

            Mapper sut = _sutBuilder
                            .WithDefaultMapping()
                            .WithMapping<SrcNameTest, DestTest>((source, dest) => 
                            {
                                dest.NameSrc = source.Name;
                            })
                            .Build();

            DestTest actual = sut.Map<DestTest>(src);
            actual = sut.Map<DestTest>(src2, actual);

            Assert.Equal(expected, actual, _destComparer);
        }

        [Fact]
        public void Single__Null__ArgumentNullException()
        {
            Mapper sut = _sutBuilder.WithDefaultMapping().Build();
            Assert.Throws<ArgumentNullException>(() => sut.Map<DestTest>(null));
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("al43errrt", "asdjj3")]
        [InlineData("[]A{d}asd}12{3--..", "km342}}{}][--..")]
        public void Single__WithInternalMapperUsage__Success(string wrapperName, string internalName)
        {
            ComplexSrcTest src = _complexSrcBuilder
                                        .WithName(wrapperName)
                                        .WithInternal(interno => interno.WithName(internalName));

            ComplexDestTest expected = _complexDestBuilder
                                        .WithName(wrapperName)
                                        .WithInternal(interno => interno.WithName(internalName));

            Mapper sut = _sutBuilder.WithDefaultMapping().Build();

            ComplexDestTest actual = sut.Map<ComplexDestTest>(src);

            Assert.Equal(expected, actual, _complexDestComparer);
        }
    }
}
