using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public interface IMapperBuilder
    {
        MapperBuilder CreateMap<TSource, TDestination>(TransformationMask<TSource, TDestination> funcMask);
        MapperBuilder CreateMap<TSource, TDestination>(TransformationMaskWithMapper<TSource, TDestination> funcMask);

        MapperBuilder AddSheet(IMappingSheet sheet);
        MapperBuilder AddSheet<T>() where T : IMappingSheet;

        IMapper Build();
    }
}
