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

            var sut = _sutBuilder.WithMapping<SrcTest, DestTest>(source => 
                                    {
                                        var dest = new DestTest();
                                        dest.NameSynonim = source.Name;
                                        return dest;
                                    })
                                    .Build();

            var actual = sut.Map<DestTest>(src);
            Assert.Equal(src.Name, actual.NameSynonim);
        }
    }
}
