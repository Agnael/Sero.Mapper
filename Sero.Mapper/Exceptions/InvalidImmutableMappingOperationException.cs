using System;

namespace Sero.Mapper;

public class InvalidImmutableMappingOperationException : Exception
{
   public InvalidImmutableMappingOperationException(Type srcType, Type destType)
      : base (
         $"Error when executing the ['{srcType}' to '{destType}'] transformation." +
         $"This specific operation is not valid for this transformation because it's result " +
         $"was defined as immutable."
      )
   {

   }
}