using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests
{
    internal class ComplexDestModel
    {
        public string ComplexResultName { get; set; }
        public DestModel ComplexResultInternal { get; set; }

        public ComplexDestModel() { }
        public ComplexDestModel(string name, DestModel interior)
        {
            ComplexResultName = name;
            ComplexResultInternal = interior;
        }
    }
}
