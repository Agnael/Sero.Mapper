using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public class DuplicatedMappingException<TSrc, TDest> : Exception
    {
        public DuplicatedMappingException() 
            : base(string.Format("The [{0} to {1}] transformation was already registered.",
                typeof(TSrc).ToString(),
                typeof(TDest).ToString()))
        {
        }
    }
}
