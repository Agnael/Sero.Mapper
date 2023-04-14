using OneOf;
using System;
using System.Threading.Tasks;

namespace Sero.Mapper;

/// <summary>
///   Delegate necessary to be able to store the transformation as a non generic type, since the 
///   MappingHandler class is also non-generic and needs to stay that way.
/// </summary>
public delegate void ConvertMutable(object src, object dest, IMapper mapper, IServiceProvider sp);
public delegate object ConvertImmutable(object src, IMapper mapper, IServiceProvider sp);
public delegate object ConvertImmutableWithBase(
   object src, 
   IMapper mapper, 
   IServiceProvider sp,
   object destBase
);

public delegate Task ConvertMutableAsync(object src, object dest, IMapper mapper, IServiceProvider sp);
public delegate Task<object> ConvertImmutableAsync(object src, IMapper mapper, IServiceProvider sp);
public delegate Task<object> ConvertImmutableWithBaseAsync(
   object src, 
   IMapper mapper, 
   IServiceProvider sp,
   object baseDest
);

/// <summary>
///   Delegate used to show to the user the lamda he has to write, with the concrete types relevant to the 
///   current transformation.
///   This one is used to allow the user to write a lambda that receives a Mapper instance to use it 
///   internally if he needs to.
/// </summary>
public delegate void ConvertMutable<TSrc, TDest>(TSrc src, TDest dest, IMapper mapper, IServiceProvider sp);
public delegate TDest ConvertImmutable<TSrc, TDest>(TSrc src, IMapper mapper, IServiceProvider sp);

// NOTE: This delegate exists as a (hacky?) solution for the case in which a update form needs to be mapped
// into an already existing model to modify it, and then the resulting updated record is returned.
public delegate TDest ConvertImmutableWithBase<TSrc, TDest>(
   TSrc src,
   IMapper mapper, 
   IServiceProvider sp,
   TDest baseDest
);

public delegate 
   Task ConvertMutableAsync<TSrc, TDest>(TSrc src, TDest dest, IMapper mapper, IServiceProvider sp);
public delegate 
   Task<TDest> ConvertImmutableAsync<TSrc, TDest>(TSrc src, IMapper mapper, IServiceProvider sp);
public delegate
   Task<TDest> ConvertImmutableWithBaseAsync<TSrc, TDest>(
      TSrc src, 
      IMapper mapper, 
      IServiceProvider sp,
      TDest baseDest
   );

/// <summary>
///   Container class for mapping transformations. It's necessary because we need a non-generic way to store 
///   generic transformation lambdas provided by the user.
/// </summary>
public record MappingHandler(
   Type SourceType,
   Type DestinationType,
   OneOf<
      ConvertMutable,
      ConvertImmutable, 
      ConvertMutableAsync, 
      ConvertImmutableAsync,
      ConvertImmutableWithBase,
      ConvertImmutableWithBaseAsync
   > Converter
)
{
   public override string ToString()
   {
      return $"[{SourceType} to {DestinationType} {nameof(MappingHandler)}]";
   }

   public static MappingHandler Make<TSrc, TDest>(
      OneOf<
         ConvertMutable<TSrc, TDest>, 
         ConvertImmutable<TSrc, TDest>,
         ConvertMutableAsync<TSrc, TDest>,
         ConvertImmutableAsync<TSrc, TDest>,
         ConvertImmutableWithBase<TSrc, TDest>,
         ConvertImmutableWithBaseAsync<TSrc, TDest>
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
                     (src, dest, mapper, sp) => convertMutable.Invoke((TSrc)src, (TDest)dest, mapper, sp)
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
                     (src, mapper, sp) => convertImmutable.Invoke((TSrc)src, mapper, sp)
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
                     async (src, dest, mapper, sp) => 
                        await convertMutableAsync.Invoke((TSrc)src, (TDest)dest, mapper, sp)
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
                     async (src, mapper, sp) => 
                        await convertImmutableAsync.Invoke((TSrc)src, mapper, sp)
                  )
               );
            },
            convertImmutableWithBase =>
            {
               return new MappingHandler(
                  typeof(TSrc),
                  typeof(TDest),
                  // Weird syntax because the lambda needs to get casted for OneOf to work this time.
                  (ConvertImmutableWithBase)
                  (
                     (src, mapper, sp, destBase) =>
                        convertImmutableWithBase.Invoke((TSrc)src, mapper, sp, (TDest)destBase)
                  )
               );
            },
            convertImmutableWithBaseAsync =>
            {
               return new MappingHandler(
                  typeof(TSrc),
                  typeof(TDest),
                  // Weird syntax because the lambda needs to get casted for OneOf to work this time.
                  (ConvertImmutableWithBaseAsync)
                  (
                     async (src, mapper, sp, destBase) =>
                        await convertImmutableWithBaseAsync.Invoke((TSrc)src, mapper, sp, (TDest)destBase)
                  )
               );
            }
         );

      return mappingHandler;
   }
}