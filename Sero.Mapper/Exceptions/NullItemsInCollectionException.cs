using System;

namespace Sero.Mapper;

 public class NullItemsInCollectionException : Exception
 {
     public NullItemsInCollectionException() : base("The provided collection has null elements.")
     {

     }
 }
