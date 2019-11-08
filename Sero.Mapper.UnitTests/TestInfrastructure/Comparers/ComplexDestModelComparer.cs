
using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests
{
    internal class ComplexDestModelComparer : IEqualityComparer<ComplexDestModel>
    {
        private DestModelComparer _destComparer;

        public ComplexDestModelComparer(DestModelComparer destTestComparer)
        {
            _destComparer = destTestComparer;
        }

        public bool Equals(ComplexDestModel expected, ComplexDestModel actual)
        {
            if (expected == null ^ actual == null)
                return false;

            if (expected == null && actual == null)
                return true;

            return expected.ComplexResultName == actual.ComplexResultName
                && _destComparer.Equals(expected.ComplexResultInternal, actual.ComplexResultInternal);
        }

        public int GetHashCode(ComplexDestModel obj)
        {
            return obj.ComplexResultName.GetHashCode()
                ^ _destComparer.GetHashCode(obj.ComplexResultInternal);
        }
    }
}
