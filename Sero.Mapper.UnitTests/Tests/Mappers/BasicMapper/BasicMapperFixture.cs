using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests.Tests.Mappers.BasicMapper
{
    public abstract class BasicMapperFixture : IDisposable
    {
        internal BasicMapperBuilder _sutBuilder { get; private set; }

        public BasicMapperFixture()
        {
            _sutBuilder = new BasicMapperBuilder();
        }

        public void Dispose()
        {

        }
    }
}
