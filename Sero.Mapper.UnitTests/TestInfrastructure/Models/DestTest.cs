using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests
{
    internal class DestTest
    {
        public int IdSrc { get; set; }
        public string NameSrc { get; set; }
        public string DescriptionSrc { get; set; }

        public DestTest() { }
        public DestTest(int id, string name, string description)
        {
            IdSrc = id;
            NameSrc = name;
            DescriptionSrc = description;
        }
    }
}
