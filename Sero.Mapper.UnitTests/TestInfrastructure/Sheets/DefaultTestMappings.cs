using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests.TestInfrastructure.Sheets
{
    public class DefaultTestMappings : IMappingSheet
    {
        public void MappingRegistration(MapperBuilder builder)
        {
            builder.CreateMap<SrcTest, DestTest>((src, dest) => 
            {
                dest.IdSrc = src.Id;
                dest.NameSrc = src.Name;
                dest.DescriptionSrc = src.Description;
            });

            builder.CreateMap<ComplexSrcTest, ComplexDestTest>((src, dest, mapper) =>
            {
                dest.ComplexResultName = src.Name;
                dest.ComplexResultInternal = mapper.Map<DestTest>(src.Internal);
            });
        }
    }
}
