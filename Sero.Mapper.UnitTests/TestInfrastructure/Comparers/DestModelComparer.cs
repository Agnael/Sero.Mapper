using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests
{
    internal class DestModelComparer : IEqualityComparer<DestModel>
    {
        public bool Equals(DestModel expected, DestModel actual)
        {
            if (expected == null ^ actual == null)
                return false;

            if (expected == null && actual == null)
                return true;

            return expected.IdSrc == actual.IdSrc
                && expected.NameSrc == actual.NameSrc
                && expected.DescriptionSrc == actual.DescriptionSrc;
        }

        public int GetHashCode(DestModel obj)
        {
            return obj.IdSrc.GetHashCode()
                ^ obj.NameSrc.GetHashCode()
                ^ obj.DescriptionSrc.GetHashCode();
        }
    }
}
