using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public interface IMapper
    {
        TDestination Map<TDestination>(object obj);
    }
}
