using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using Xunit;

namespace Sero.Mapper.Tests.MapperBuilderTests;

public class CreateMap
{
   [Fact]
   public void SimpleTransformation__CreatesDuplicateMapping__DuplicatedMappingException()
   {
      IServiceProvider sp = new ServiceCollection().BuildServiceProvider();

      MapperBuilder builder = new MapperBuilder(new NullLogger<Mapper>(), sp);
      builder.CreateMap<SrcModel, DestModel>((src, dest, mapper, sp) => { });

      Assert.Throws<MappingCollectionDuplicateException>(
          () => builder.CreateMap<SrcModel, DestModel>((src, dest, mapper, sp) => { })
      );
   }

   [Fact]
   public void TransformationWithMapper__CreatesDuplicateMapping__DuplicatedMappingException()
   {
      IServiceProvider sp = new ServiceCollection().BuildServiceProvider();

      MapperBuilder builder = new MapperBuilder(new NullLogger<Mapper>(), sp);
      builder.CreateMap<SrcModel, DestModel>((src, dest, mapper, sp) => { });

      Assert.Throws<MappingCollectionDuplicateException>(
          () => builder.CreateMap<SrcModel, DestModel>((src, dest, mapper, sp) => { })
      );
   }
}
