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
        public void Single__WithDestination__Success(string propValue)
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

            IMapper sut = _sutBuilder
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

        //[Theory]
        //[InlineData("")]
        //[InlineData(null)]
        //[InlineData("al43errrt")]
        //[InlineData("[]A{d}asd}12{3--..")]
        //public void Single__MutationAttempt__Ignored(string propValue)
        //{
        //    SrcTest src = _srcBuilder.WithName(propValue);
        //    DestTest expected = _destBuilder.WithName(propValue);

        //    IMapper sut = _sutBuilder.WithMapping<SrcTest, DestTest>((orig, dest) =>
        //    {
        //        dest.NameSrc = orig.Name;

        //        // Modifies the ORIGINAL value ***********************************
        //        // This change should be ignored outside of the scope of the block,
        //        // ideally it should be not accepted by the compiler
        //        orig.Name += "_MODIFIED";
        //    })
        //    .Build();

        //    DestTest actual = sut.Map<DestTest>(src);

        //    Assert.Equal(expected, actual, _destComparer);
        //    Assert.Equal(propValue, src.Name);
        //}
    }
}
