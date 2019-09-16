using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests
{
    internal class SrcTest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public SrcTest() { }
        public SrcTest(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
