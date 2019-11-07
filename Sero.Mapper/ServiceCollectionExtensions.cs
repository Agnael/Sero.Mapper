using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Tries to register a Mapper instance as a Singleton service. 
        ///     It does nothing if another instance is already registered.
        /// </summary>
        /// <param name="builderConfig">
        ///     MapperBuilder configuration that will be used to internally build the Mapper.
        /// </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void AddSeroMapper(this IServiceCollection services, Action<MapperBuilder> builderConfig)
        {
            if (builderConfig == null)
                throw new ArgumentNullException("builderConfig");

            MapperBuilder builder = new MapperBuilder();
            builderConfig.Invoke(builder);

            services.TryAddSingleton(builder.Build());
        }
    }
}
