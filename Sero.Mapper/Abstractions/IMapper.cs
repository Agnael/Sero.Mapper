using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public interface IMapper
    {
        TDestination Map<TDestination>(object obj, TDestination existingDestination = default(TDestination));
        ICollection<TDestination> MapList<TDestination>(IEnumerable<object> objList);
    }
}
