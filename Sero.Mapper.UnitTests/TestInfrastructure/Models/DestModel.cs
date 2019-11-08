using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests
{
    internal class DestModel
    {
        public int IdSrc { get; set; }
        public string NameSrc { get; set; }
        public string DescriptionSrc { get; set; }

        public DestModel() { }
        public DestModel(int id, string name, string description)
        {
            IdSrc = id;
            NameSrc = name;
            DescriptionSrc = description;
        }
    }
}
