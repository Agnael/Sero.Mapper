using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    internal delegate object Transformation(object entity);

    internal class MappingHandler
    {
        public Type SourceType { get; set; }
        public Type DestinationType { get; set; }
        public Transformation Transform { get; set; }
        public Func<object, object> Func { get; set; }

        public MappingHandler(Type sourceType, Type destinationType, Func<object, object> func)
        {
            this.SourceType = sourceType;
            this.DestinationType = destinationType;
            this.Func = func;
        }
    }
}
