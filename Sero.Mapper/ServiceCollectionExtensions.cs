using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;

namespace Sero.Mapper;

public static class ServiceCollectionExtensions
{
   /// <summary>
   ///   Tries to register a Mapper instance as a Singleton service. 
   ///   It does nothing if another instance is already registered.
   /// </summary>
   /// <param name="builderConfig">
   ///   MapperBuilder configuration that will be used to internally build the Mapper.
   /// </param>
   /// <exception cref="System.ArgumentNullException"></exception>
   public static IServiceCollection AddSeroMapper(
      this IServiceCollection services, 
      Action<MapperBuilder> builderConfig) 
   {
      if (builderConfig == null)
         throw new ArgumentNullException("builderConfig");

      services.TryAddScoped<IMapper>(
         serviceProvider =>
         {
            ILogger logger = 
               serviceProvider.GetService<ILogger<Mapper>>() ?? new NullLogger<Mapper>();

            MapperBuilder builder = new MapperBuilder(logger, serviceProvider);
            builderConfig.Invoke(builder);

            IEnumerable<IMappingSheet> mappingSheetServices = 
               serviceProvider.GetRequiredService<IEnumerable<IMappingSheet>>();

            if (mappingSheetServices != null)
            {
               foreach (IMappingSheet mappingSheetService in mappingSheetServices)
               {
                  builder.AddSheet(mappingSheetService);
               }
            }

            return builder.Build();
         }
      );

      return services;
   }

   public static IServiceCollection AddSeroMapper(this IServiceCollection services)
   {
      return services.AddSeroMapper(_ => { });
   }
}
