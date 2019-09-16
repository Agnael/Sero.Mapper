using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests.ModelBuilders
{
    internal class SrcTestBuilder
    {
        private int _id;
        private string _name;
        private string _description;

        public SrcTestBuilder()
        {
            _id = 100;
            _name = "Test name";
            _description = "Test description";
        }

        internal SrcTestBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        internal SrcTestBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal SrcTestBuilder WithDescription(string desc)
        {
            _description = desc;
            return this;
        }

        internal SrcTest Build()
        {
            return new SrcTest(_id, _name, _description);
        }

        public static implicit operator SrcTest(SrcTestBuilder builder) => builder.Build();
    }
}
