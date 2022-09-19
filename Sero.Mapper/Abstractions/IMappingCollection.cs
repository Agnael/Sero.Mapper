using Functional.Maybe;
using System;
using System.Collections.Generic;

namespace Sero.Mapper;

public interface IMappingCollection<TMapping> : ICollection<TMapping>
   where TMapping : class, IMapping
{
   TMapping GetMappingHandler(Type srcType, Type destType);
}
