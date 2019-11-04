using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests.ModelBuilders
{
    internal class ComplexSrcTestBuilder
    {
        private string _name;
        private SrcTestBuilder _internalBuilder;

        public ComplexSrcTestBuilder()
        {
            _name = "test complex name";
            _internalBuilder = new SrcTestBuilder();
        }

        internal ComplexSrcTestBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal ComplexSrcTestBuilder WithInternal(Action<SrcTestBuilder> config)
        {
            config(_internalBuilder);
            return this;
        }

        internal ComplexSrcTest Build()
        {
            return new ComplexSrcTest(_name, _internalBuilder);
        }

        public static implicit operator ComplexSrcTest(ComplexSrcTestBuilder builder) => builder.Build();
    }
}
