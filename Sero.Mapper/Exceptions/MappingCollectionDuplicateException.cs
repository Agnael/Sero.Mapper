using System;

namespace Sero.Mapper;

public class MappingCollectionDuplicateException : Exception
{
   public MappingCollectionDuplicateException(Type srcType, Type destType) :
      base($"A [{srcType} to {destType}] transformation was already registered.")
   {

   }
}
