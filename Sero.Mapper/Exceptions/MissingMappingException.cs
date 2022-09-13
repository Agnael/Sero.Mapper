using System;

namespace Sero.Mapper;

public class MissingMappingException : Exception
{
   public MissingMappingException(Type srcType, Type destType) :
      base($"No mapping was found for [{srcType} to {destType}].")
   {

   }
}
