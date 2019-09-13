using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public delegate object Transformation(object entity);

    public class MappingHandler
    {
        public Type SourceType { get; set; }
        public Type DestinationType { get; set; }
        public Func<object, object> Transformation { get; set; }

        public MappingHandler(Type sourceType, Type destinationType, Func<object, object> func)
        {
            this.SourceType = sourceType;
            this.DestinationType = destinationType;
            this.Transformation = func;
        }
    }
}
