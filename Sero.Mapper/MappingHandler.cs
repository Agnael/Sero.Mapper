using OneOf;
using System;

namespace Sero.Mapper;

/// <summary>
///   Delegate necessary to be able to store the transformation as a non generic type, since the 
///   MappingHandler class is also non-generic and needs to stay that way.
/// </summary>
public delegate void ConvertMutable(object src, object dest, Mapper mapper);

public delegate object ConvertImmutable(object src, Mapper mapper);

/// <summary>
///   Delegate used to show to the user the lamda he has to write, with the concrete types relevant to the 
///   current transformation.
///   This one is used to allow the user to write a lambda that receives a Mapper instance to use it 
///   internally if he needs to.
/// </summary>
public delegate void ConvertMutable<TSrc, TDest>(TSrc src, TDest dest, Mapper mapper);

public delegate TDest ConvertImmutable<TSrc, TDest>(TSrc src, Mapper mapper);

/// <summary>
///   Container class for mapping transformations. It's necessary because we need a non-generic way to store 
///   generic transformation lambdas provided by the user.
/// </summary>
public class MappingHandler
{
   public Type SourceType { get; protected set; }
   public Type DestinationType { get; protected set; }
   public OneOf<ConvertMutable, ConvertImmutable> Converter { get; protected set; }

   public MappingHandler(Type sourceType, Type destinationType, ConvertMutable mutableConverter)
   {
      this.SourceType = sourceType;
      this.DestinationType = destinationType;
      this.Converter = mutableConverter;
   }

   public MappingHandler(Type sourceType, Type destinationType, ConvertImmutable immutableConverter)
   {
      this.SourceType = sourceType;
      this.DestinationType = destinationType;
      this.Converter = immutableConverter;
   }
}
