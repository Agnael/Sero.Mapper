using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            MapperBuilder builder = new MapperBuilder();
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
      MapperBuilder builder = new MapperBuilder();
      return services.AddSeroMapper(_ => { });
   }
}
