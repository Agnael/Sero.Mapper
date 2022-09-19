using System;

namespace Sero.Mapper;

public interface IMapping
{
   Type SourceType { get; }
   Type DestinationType { get; }
}
