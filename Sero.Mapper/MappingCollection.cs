using System;
using System.Collections.Generic;
using System.Linq;

namespace Sero.Mapper;

public class MappingCollection<TMapping> : SynchronizedCollection<TMapping>, IMappingCollection<TMapping>
   where TMapping : class, IMapping
{
   public TMapping GetMappingHandler(Type srcType, Type destType)
   {
      TMapping handler =
         this.FirstOrDefault(
            mapping => mapping.SourceType == srcType &&
            mapping.DestinationType == destType
         );

      if (handler == null)
         throw new MissingMappingException(srcType, destType);

      return handler;
   }

   public new void Add(TMapping mapping)
   {
      bool isDuplicate =
         this.Any(
            existingMapping =>
               existingMapping.SourceType == mapping.SourceType &&
               existingMapping.DestinationType == mapping.DestinationType
         );

      if (isDuplicate)
         throw new MappingCollectionDuplicateException(mapping.SourceType, mapping.DestinationType);

      (this as SynchronizedCollection<TMapping>).Add(mapping);
   }
}
