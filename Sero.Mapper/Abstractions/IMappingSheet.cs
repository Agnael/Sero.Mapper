using System;
using System.Collections.Generic;
using System.Text;

namespace Sero.Mapper
{
    public interface IMappingSheet
    {
        void MappingRegistration(IMapperBuilder builder);
    }
}
