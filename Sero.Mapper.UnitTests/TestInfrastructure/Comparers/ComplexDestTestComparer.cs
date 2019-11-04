
using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests.Comparers
{
    internal class ComplexDestTestComparer : IEqualityComparer<ComplexDestTest>
    {
        private DestTestComparer _destComparer;

        public ComplexDestTestComparer(DestTestComparer destTestComparer)
        {
            _destComparer = destTestComparer;
        }

        public bool Equals(ComplexDestTest expected, ComplexDestTest actual)
        {
            if (expected == null ^ actual == null)
                return false;

            if (expected == null && actual == null)
                return true;

            return expected.ComplexResultName == actual.ComplexResultName
                && _destComparer.Equals(expected.ComplexResultInternal, actual.ComplexResultInternal);
        }

        public int GetHashCode(ComplexDestTest obj)
        {
            return obj.ComplexResultName.GetHashCode()
                ^ _destComparer.GetHashCode(obj.ComplexResultInternal);
        }
    }
}
