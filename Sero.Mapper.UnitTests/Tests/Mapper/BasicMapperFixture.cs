using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests.MapperTests
{
    public abstract class BasicMapperFixture : IDisposable
    {
        internal SrcModelBuilder _srcBuilder { get; private set; }
        internal DestModelBuilder _destBuilder { get; private set; }

        internal ComplexSrcModelBuilder _complexSrcBuilder { get; private set; }
        internal ComplexDestModelBuilder _complexDestBuilder { get; private set; }

        internal DestModelComparer _destComparer { get; private set; }
        internal ComplexDestModelComparer _complexDestComparer { get; private set; }

        internal BasicMapperBuilder _sutBuilder { get; private set; }

        public BasicMapperFixture()
        {
            _sutBuilder = new BasicMapperBuilder();

            _srcBuilder = new SrcModelBuilder();
            _destBuilder = new DestModelBuilder();

            _complexSrcBuilder = new ComplexSrcModelBuilder();
            _complexDestBuilder = new ComplexDestModelBuilder();

            _destComparer = new DestModelComparer();
            _complexDestComparer = new ComplexDestModelComparer(_destComparer);
        }

        public void Dispose()
        {

        }
    }
}
