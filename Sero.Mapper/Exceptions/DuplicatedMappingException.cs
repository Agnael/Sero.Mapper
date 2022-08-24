using System;

namespace Sero.Mapper;

public class DuplicatedMappingException<TSrc, TDest> : Exception
{
   public DuplicatedMappingException() : 
      base($"The [{typeof(TSrc)} to {typeof(TDest)}] transformation was already registered.")
   {

   }
}
