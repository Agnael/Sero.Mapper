using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSeroMapping(this IServiceCollection services, IMapperBuilder builder)
        {
            services.AddSingleton(builder.Build());
        }
    }
}
