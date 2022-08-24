using System;

namespace Sero.Mapper;

public class MissingMappingException : Exception
{
   public MissingMappingException(Type srcType, Type destType) :
      base(
         $"No transformation was registered for the [{srcType.GetType()} to {destType.GetType()}] mapping."
      )
   {

   }
}
