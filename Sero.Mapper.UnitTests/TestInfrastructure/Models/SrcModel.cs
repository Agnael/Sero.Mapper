using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests
{
    internal class SrcModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public SrcModel() { }
        public SrcModel(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
