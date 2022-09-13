using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper.Tests
{
    public class DefaultSheet : IMappingSheet
    {
        public void MappingRegistration(MapperBuilder builder)
        {
            builder.CreateMap<SrcModel, DestModel>((src, dest, mapper) => 
            {
                dest.IdSrc = src.Id;
                dest.NameSrc = src.Name;
                dest.DescriptionSrc = src.Description;
            });

            builder.CreateMap<ComplexSrcModel, ComplexDestModel>((src, dest, mapper) =>
            {
                dest.ComplexResultName = src.Name;
                dest.ComplexResultInternal = mapper.Map<DestModel>(src.Internal);
            });
        }
    }
}
