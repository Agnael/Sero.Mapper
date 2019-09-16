using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests.ModelBuilders
{
    internal class DestTestBuilder
    {
        private int _id;
        private string _name;
        private string _description;

        public DestTestBuilder()
        {
            _id = 100;
            _name = "Test name";
            _description = "Test description";
        }

        internal DestTestBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        internal DestTestBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal DestTestBuilder WithDescription(string desc)
        {
            _description = desc;
            return this;
        }

        internal DestTest Build()
        {
            return new DestTest(_id, _name, _description);
        }

        public static implicit operator DestTest(DestTestBuilder builder) => builder.Build();
    }
}
