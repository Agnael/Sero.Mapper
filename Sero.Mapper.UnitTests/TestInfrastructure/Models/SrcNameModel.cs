using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests
{
    internal class SrcNameModel
    {
        public string Name { get; set; }

        public SrcNameModel() { }
        public SrcNameModel(string name)
        {
            Name = name;
        }
    }
}
