using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Sero.Mapper.Tests.MapperTests
{
    public class MapList : BasicMapperFixture
    {
        [Theory]
        [InlineData("elem1", "elem2")]
        [InlineData("elem1", null, "socotroco")]
        public void Collection_Success(params string[] propValues)
        {
            // Arrange
            IList<SrcModel> srcList = new List<SrcModel>();
            IList<DestModel> expectedList = new List<DestModel>();

            foreach (var value in propValues)
                srcList.Add(new SrcModel(5, value, value));

            foreach (var value in propValues)
                expectedList.Add(new DestModel(5, value, value));

            Mapper sut = _sutBuilder.WithDefaultMapping().Build();

            // Execute
            ICollection<DestModel> actualList = sut.MapList<DestModel>(srcList);

            // Assert
            Assert.Equal<DestModel>(expectedList, actualList, _destComparer);
        }

        // Null elements are mapped to nulls rather than rejecting the whole list,
        // matching the permissive single-item behaviour. Deliberate since 48b91ee.
        [Fact]
        public void Collection__WithNullElements__NullElementsMapToNull()
        {
            IList<SrcModel> srcList = new List<SrcModel>();
            srcList.Add(_srcBuilder.WithName("elem1"));
            srcList.Add(null);
            srcList.Add(_srcBuilder.WithName("elem3"));

            Mapper sut = _sutBuilder.WithDefaultMapping().Build();

            ICollection<DestModel> actual = sut.MapList<DestModel>(srcList);

            Assert.Equal(3, actual.Count);
            Assert.NotNull(actual.ElementAt(0));
            Assert.Null(actual.ElementAt(1));
            Assert.NotNull(actual.ElementAt(2));
        }
    }
}
