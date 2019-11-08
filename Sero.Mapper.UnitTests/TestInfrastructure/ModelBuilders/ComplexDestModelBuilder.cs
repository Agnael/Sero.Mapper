using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests
{
    internal class ComplexDestModelBuilder
    {
        private string _name;
        private DestModelBuilder _internalBuilder;

        public ComplexDestModelBuilder()
        {
            _name = "test complex name";
            _internalBuilder = new DestModelBuilder();
        }

        internal ComplexDestModelBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal ComplexDestModelBuilder WithInternal(Action<DestModelBuilder> config)
        {
            config(_internalBuilder);
            return this;
        }

        internal ComplexDestModel Build()
        {
            return new ComplexDestModel(_name, _internalBuilder);
        }

        public static implicit operator ComplexDestModel(ComplexDestModelBuilder builder) => builder.Build();
    }
}
