using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests
{
    internal class ComplexSrcModelBuilder
    {
        private string _name;
        private SrcModelBuilder _internalBuilder;

        public ComplexSrcModelBuilder()
        {
            _name = "test complex name";
            _internalBuilder = new SrcModelBuilder();
        }

        internal ComplexSrcModelBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal ComplexSrcModelBuilder WithInternal(Action<SrcModelBuilder> config)
        {
            config(_internalBuilder);
            return this;
        }

        internal ComplexSrcModel Build()
        {
            return new ComplexSrcModel(_name, _internalBuilder);
        }

        public static implicit operator ComplexSrcModel(ComplexSrcModelBuilder builder) => builder.Build();
    }
}
