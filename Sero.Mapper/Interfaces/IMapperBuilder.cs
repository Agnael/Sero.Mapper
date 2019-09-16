using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public interface IMapperBuilder
    {
        void CreateMap<TSource, TDestination>(TransformationMask<TSource, TDestination> funcMask);

        void AddSheet(AbstractMappingSheet sheet);

        IMapper Build();
    }
}
