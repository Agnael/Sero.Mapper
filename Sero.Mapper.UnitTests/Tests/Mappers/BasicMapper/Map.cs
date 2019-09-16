using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sero.Mapper.UnitTests.Tests.Mappers.BasicMapper
{
    internal class SrcTest
    {
        public string Name { get; set; }

        public SrcTest() { }
        public SrcTest(string name)
        {
            Name = name;
        }
    }
    
    internal class DestTest
    {
        public string NameSynonim { get; set; }

        public DestTest() { }
        public DestTest(string nameSynonim)
        {
            NameSynonim = nameSynonim;
        }
    }

    public class Map : BasicMapperFixture
    {
        [Theory]
        [InlineData("alberrrt")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Valor")]
        [InlineData("3424234")]
        [InlineData("[]A{d}asd}12{3--..")]
        public void Single__Success(string propValue)
        {
            var src = new SrcTest(propValue);

            var sut = _sutBuilder.WithMapping<SrcTest, DestTest>((orig, dest) =>
            {
                dest.NameSynonim = orig.Name;
            })
            .Build();

            var actual = sut.Map<DestTest>(src);
            Assert.Equal(src.Name, actual.NameSynonim);
        }
        [Theory]
        [InlineData("alberrrt")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Valor")]
        [InlineData("3424234")]
        [InlineData("[]A{d}asd}12{3--..")]
        public void Single__MutationAttempt__Ignored(string propValue)
        {
            var src = new SrcTest(propValue);

            var sut = _sutBuilder.WithMapping<SrcTest, DestTest>((orig, dest) =>
            {
                dest.NameSynonim = orig.Name;

                // Modifies the ORIGINAL value
                orig.Name += "_MODIFIED";
            })
            .Build();

            var actual = sut.Map<DestTest>(src);
            Assert.Equal(src.Name, actual.NameSynonim);
        }

        [Theory]
        [InlineData("", "" )]
        public void Collection_Success(params string[] propValues)
        {

        }
    }
}
