using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public class BasicMapper : AbstractMapper
    {
        public BasicMapper(IEnumerable<MappingHandler> mappingHandlers)
            : base(mappingHandlers)
        {

        }
    }
}
