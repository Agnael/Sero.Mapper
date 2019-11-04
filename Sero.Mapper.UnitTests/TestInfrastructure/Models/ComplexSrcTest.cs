using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests
{
    internal class ComplexSrcTest
    {
        public string Name { get; set; }
        public SrcTest Internal { get; set; }

        public ComplexSrcTest() { }
        public ComplexSrcTest(string name, SrcTest interior)
        {
            Name = name;
            Internal = interior;
        }
    }
}
