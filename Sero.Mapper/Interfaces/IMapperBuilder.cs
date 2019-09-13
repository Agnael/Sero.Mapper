using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public interface IMapperBuilder
    {
        void CreateMap<TSource, TDestination>(Func<TSource, TDestination> func);

        void AddSheet(AbstractMappingSheet sheet);

        IMapper Build();
    }
}
