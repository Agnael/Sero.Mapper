using System;

namespace Sero.Mapper;

public class MutableConverterNotFoundException : Exception
{
   public MutableConverterNotFoundException(Type srcType, Type destType)
      : base(
         $"A mutable converter (mapping with void return type) is needed but wasn't found for " +
         $"transformation ['{srcType}' to '{destType}'].")
   {

   }
}
