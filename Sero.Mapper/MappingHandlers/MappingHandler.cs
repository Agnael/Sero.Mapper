using OneOf;
using System;
using System.Threading.Tasks;

namespace Sero.Mapper;

/// <summary>
///   Delegate necessary to be able to store the transformation as a non generic type, since the 
///   MappingHandler class is also non-generic and needs to stay that way.
/// </summary>
public delegate void ConvertMutable(object src, object dest, Mapper mapper);
public delegate object ConvertImmutable(object src, Mapper mapper);

public delegate Task ConvertMutableAsync(object src, object dest, Mapper mapper);
public delegate Task<object> ConvertImmutableAsync(object src, Mapper mapper);

/// <summary>
///   Delegate used to show to the user the lamda he has to write, with the concrete types relevant to the 
///   current transformation.
///   This one is used to allow the user to write a lambda that receives a Mapper instance to use it 
///   internally if he needs to.
/// </summary>
public delegate void ConvertMutable<TSrc, TDest>(TSrc src, TDest dest, Mapper mapper);
public delegate TDest ConvertImmutable<TSrc, TDest>(TSrc src, Mapper mapper);

public delegate Task ConvertMutableAsync<TSrc, TDest>(TSrc src, TDest dest, Mapper mapper);
public delegate Task<TDest> ConvertImmutableAsync<TSrc, TDest>(TSrc src, Mapper mapper);

/// <summary>
///   Container class for mapping transformations. It's necessary because we need a non-generic way to store 
///   generic transformation lambdas provided by the user.
/// </summary>
public record MappingHandler(
   Type SourceType,
   Type DestinationType,
   OneOf<ConvertMutable, ConvertImmutable, ConvertMutableAsync, ConvertImmutableAsync> Converter
) : IMapping
{
   public static MappingHandler Make<TSrc, TDest>(
      OneOf<
         ConvertMutable<TSrc, TDest>, 
         ConvertImmutable<TSrc, TDest>,
         ConvertMutableAsync<TSrc, TDest>,
         ConvertImmutableAsync<TSrc, TDest>
      > converter)
   {
      MappingHandler mappingHandler =
         converter
         .Match(
            convertMutable =>
            {
               return new MappingHandler(
                  typeof(TSrc),
                  typeof(TDest),
                  // Weird syntax because the lambda needs to get casted for OneOf to work this time.
                  (ConvertMutable)
                  (
                     (src, dest, mapper) => convertMutable.Invoke((TSrc)src, (TDest)dest, mapper)
                  )
               );
            },
            convertImmutable =>
            {
               return new MappingHandler(
                  typeof(TSrc),
                  typeof(TDest),
                  // Weird syntax because the lambda needs to get casted for OneOf to work this time.
                  (ConvertImmutable)
                  (
                     (src, mapper) => convertImmutable.Invoke((TSrc)src, mapper)
                  )
               );
            },
            convertMutableAsync =>
            {
               return new MappingHandler(
                  typeof(TSrc),
                  typeof(TDest),
                  // Weird syntax because the lambda needs to get casted for OneOf to work this time.
                  (ConvertMutableAsync)
                  (
                     async (src, dest, mapper) => 
                        await convertMutableAsync.Invoke((TSrc)src, (TDest)dest, mapper)
                  )
               );
            },
            convertImmutableAsync =>
            {
               return new MappingHandler(
                  typeof(TSrc),
                  typeof(TDest),
                  // Weird syntax because the lambda needs to get casted for OneOf to work this time.
                  (ConvertImmutableAsync)
                  (
                     async (src, mapper) => 
                        await convertImmutableAsync.Invoke((TSrc)src, mapper)
                  )
               );
            }
         );

      return mappingHandler;
   }
}