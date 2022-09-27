using System;
using System.Collections.Generic;
using System.Linq;

namespace Sero.Mapper;

public class MappingCollection : SynchronizedCollection<MappingHandler>, IMappingCollection
{
   public MappingHandler GetMappingHandler(Type srcType, Type destType)
   {
      MappingHandler handler =
         this.FirstOrDefault(
            mapping => mapping.SourceType == srcType &&
            mapping.DestinationType == destType
         );

      if (handler == null)
         throw new MissingMappingException(srcType, destType);

      return handler;
   }

   public new void Add(MappingHandler mapping)
   {
      bool isDuplicate =
         this.Any(
            existingMapping =>
               existingMapping.SourceType == mapping.SourceType &&
               existingMapping.DestinationType == mapping.DestinationType
         );

      if (isDuplicate)
         throw new MappingCollectionDuplicateException(mapping.SourceType, mapping.DestinationType);

      (this as SynchronizedCollection<MappingHandler>).Add(mapping);
   }
}
