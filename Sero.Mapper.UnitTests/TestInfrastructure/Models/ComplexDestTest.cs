using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests
{
    internal class ComplexDestTest
    {
        public string ComplexResultName { get; set; }
        public DestTest ComplexResultInternal { get; set; }

        public ComplexDestTest() { }
        public ComplexDestTest(string name, DestTest interior)
        {
            ComplexResultName = name;
            ComplexResultInternal = interior;
        }
    }
}
