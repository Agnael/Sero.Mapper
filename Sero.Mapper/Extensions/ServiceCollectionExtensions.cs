using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSeroMapper(this IServiceCollection services, Action<IMapperBuilder> opts = null)
        {
            MapperBuilder builder = new MapperBuilder();

            if(opts != null)
                opts.Invoke(builder);

            services.AddSingleton(builder.Build());
        }
    }
}
