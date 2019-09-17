using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.UnitTests.TestInfrastructure.Sheets
{
    public class DefaultTestMappings : IMappingSheet
    {
        public void MappingRegistration(IMapperBuilder builder)
        {
            builder.CreateMap<SrcTest, DestTest>((src, dest) => {
                dest.IdSrc = src.Id;
                dest.NameSrc = src.Name;
                dest.DescriptionSrc = src.Description;
            });
        }
    }
}
