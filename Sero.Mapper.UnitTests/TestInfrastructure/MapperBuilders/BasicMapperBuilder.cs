using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sero.Mapper.Tests;

internal class BasicMapperBuilder
{
   private MapperBuilder _mapperbuilder;

   public BasicMapperBuilder()
   {
      IServiceProvider sp = new ServiceCollection().BuildServiceProvider();
      _mapperbuilder = new MapperBuilder(new NullLogger<Mapper>(), sp);
   }

   internal BasicMapperBuilder WithDefaultMapping()
   {
      _mapperbuilder.AddSheet<DefaultSheet>();
      return this;
   }

   internal BasicMapperBuilder WithMapping<TSrc, TDest>(ConvertMutable<TSrc, TDest> transformation)
   {
      _mapperbuilder.CreateMap<TSrc, TDest>(transformation);
      return this;
   }

   internal Mapper Build()
   {
      return _mapperbuilder.Build();
   }
}
