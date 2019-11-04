using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public interface IMapperBuilder
    {
        void CreateMap<TSource, TDestination>(TransformationMask<TSource, TDestination> funcMask);
        void CreateMap<TSource, TDestination>(TransformationMaskWithMapper<TSource, TDestination> funcMask);

        void AddSheet(IMappingSheet sheet);
        void AddSheet<T>() where T : IMappingSheet;

        IMapper Build();
    }
}
