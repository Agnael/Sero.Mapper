using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public delegate object Transformation(object obj, object destObj = null);
    public delegate void TransformationMask<TSrc, TDest>(TSrc src, TDest dest);

    public class MappingHandler
    {
        public Type SourceType { get; protected set; }
        public Type DestinationType { get; protected set; }
        public Transformation Transformation { get; protected set; }

        public MappingHandler(Type sourceType, Type destinationType, Transformation transf)
        {
            this.SourceType = sourceType;
            this.DestinationType = destinationType;
            this.Transformation = transf;
        }
    }
}
