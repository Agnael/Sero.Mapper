using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Sero.Mapper.UnitTests.Mappers.BasicMapper
{
    public class MapList : BasicMapperFixture
    {
        [Theory]
        [InlineData("elem1", "elem2")]
        [InlineData("elem1", null, "socotroco")]
        public void Collection_Success(params string[] propValues)
        {
            // Arrange
            IList<SrcTest> srcList = new List<SrcTest>();
            IList<DestTest> expectedList = new List<DestTest>();

            foreach (var value in propValues)
                srcList.Add(new SrcTest(5, value, value));

            foreach (var value in propValues)
                expectedList.Add(new DestTest(5, value, value));

            IMapper sut = _sutBuilder.WithDefaultMapping().Build();

            // Execute
            ICollection<DestTest> actualList = sut.MapList<DestTest>(srcList);

            // Assert
            Assert.Equal<DestTest>(expectedList, actualList, _destComparer);
        }

        [Fact]
        public void Collection__WithNullElements__Success()
        {
            IList<SrcTest> srcList = new List<SrcTest>();
            IList<DestTest> expectedList = new List<DestTest>();

            srcList.Add(_srcBuilder.WithName("elem1"));
            srcList.Add(null);
            srcList.Add(_srcBuilder.WithName("elem3"));

            expectedList.Add(_destBuilder.WithName("elem1"));
            expectedList.Add(null);
            expectedList.Add(_destBuilder.WithName("elem3"));

            IMapper sut = _sutBuilder.WithDefaultMapping().Build();
            ICollection<DestTest> actualList = sut.MapList<DestTest>(srcList);
            
            Assert.Equal<DestTest>(expectedList, actualList, _destComparer);
        }
    }
}
