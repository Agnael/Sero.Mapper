using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests.ModelBuilders
{
    internal class ComplexDestTestBuilder
    {
        private string _name;
        private DestTestBuilder _internalBuilder;

        public ComplexDestTestBuilder()
        {
            _name = "test complex name";
            _internalBuilder = new DestTestBuilder();
        }

        internal ComplexDestTestBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal ComplexDestTestBuilder WithInternal(Action<DestTestBuilder> config)
        {
            config(_internalBuilder);
            return this;
        }

        internal ComplexDestTest Build()
        {
            return new ComplexDestTest(_name, _internalBuilder);
        }

        public static implicit operator ComplexDestTest(ComplexDestTestBuilder builder) => builder.Build();
    }
}
