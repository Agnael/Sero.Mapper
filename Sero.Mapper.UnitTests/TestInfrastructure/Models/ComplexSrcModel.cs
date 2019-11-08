using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests
{
    internal class ComplexSrcModel
    {
        public string Name { get; set; }
        public SrcModel Internal { get; set; }

        public ComplexSrcModel() { }
        public ComplexSrcModel(string name, SrcModel interior)
        {
            Name = name;
            Internal = interior;
        }
    }
}
