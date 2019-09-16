using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests.Comparers
{
    internal class DestTestComparer : IEqualityComparer<DestTest>
    {
        public bool Equals(DestTest expected, DestTest actual)
        {
            if (expected == null ^ actual == null)
                return false;

            if (expected == null && actual == null)
                return true;

            return expected.IdSrc == actual.IdSrc
                && expected.NameSrc == actual.NameSrc
                && expected.DescriptionSrc == actual.DescriptionSrc;
        }

        public int GetHashCode(DestTest obj)
        {
            return obj.IdSrc.GetHashCode()
                ^ obj.NameSrc.GetHashCode()
                ^ obj.DescriptionSrc.GetHashCode();
        }
    }
}
