using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public class MissingMappingException : Exception
    {
        public MissingMappingException(Type srcType, Type destType) 
            : base(string.Format("No transformation was registered for the [{0} to {1}] mapping.",
                    srcType.GetType().ToString(),
                    destType.GetType().ToString()))
        {
        }
    }
}
