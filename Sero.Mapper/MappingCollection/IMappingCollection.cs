using System;
using System.Collections.Generic;

namespace Sero.Mapper;

public interface IMappingCollection : ICollection<MappingHandler>
{
   MappingHandler GetMappingHandler(Type srcType, Type destType);
}
