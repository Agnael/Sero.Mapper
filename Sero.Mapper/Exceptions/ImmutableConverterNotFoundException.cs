using System;

namespace Sero.Mapper;

public class ImmutableConverterNotFoundException : Exception
{
   public ImmutableConverterNotFoundException(Type srcType, Type destType)
      : base(
         $"An immutable converter (mapping with '{destType}' return type) is needed but wasn't found " +
         $"for transformation ['{srcType}' to '{destType}'].")
   {

   }
}