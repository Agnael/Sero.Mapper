using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests
{
    internal class SrcModelBuilder
    {
        private int _id;
        private string _name;
        private string _description;

        public SrcModelBuilder()
        {
            _id = 100;
            _name = "Test name";
            _description = "Test description";
        }

        internal SrcModelBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        internal SrcModelBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal SrcModelBuilder WithDescription(string desc)
        {
            _description = desc;
            return this;
        }

        internal SrcModel Build()
        {
            return new SrcModel(_id, _name, _description);
        }

        public static implicit operator SrcModel(SrcModelBuilder builder) => builder.Build();
    }
}
