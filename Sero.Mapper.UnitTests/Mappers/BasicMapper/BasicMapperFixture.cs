using Sero.Mapper.UnitTests.Comparers;
using Sero.Mapper.UnitTests.ModelBuilders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests.Mappers.BasicMapper
{
    public abstract class BasicMapperFixture : IDisposable
    {
        internal SrcTestBuilder _srcBuilder { get; private set; }
        internal DestTestBuilder _destBuilder { get; private set; }
        internal DestTestComparer _destComparer { get; private set; }

        internal BasicMapperBuilder _sutBuilder { get; private set; }

        public BasicMapperFixture()
        {
            _sutBuilder = new BasicMapperBuilder();

            _srcBuilder = new SrcTestBuilder();
            _destBuilder = new DestTestBuilder();
            _destComparer = new DestTestComparer();
        }

        public void Dispose()
        {

        }
    }
}
