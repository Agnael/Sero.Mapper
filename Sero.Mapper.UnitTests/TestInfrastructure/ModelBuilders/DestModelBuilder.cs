using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests
{
    internal class DestModelBuilder
    {
        private int _id;
        private string _name;
        private string _description;

        public DestModelBuilder()
        {
            _id = 100;
            _name = "Test name";
            _description = "Test description";
        }

        internal DestModelBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        internal DestModelBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal DestModelBuilder WithDescription(string desc)
        {
            _description = desc;
            return this;
        }

        internal DestModel Build()
        {
            return new DestModel(_id, _name, _description);
        }

        public static implicit operator DestModel(DestModelBuilder builder) => builder.Build();
    }
}
