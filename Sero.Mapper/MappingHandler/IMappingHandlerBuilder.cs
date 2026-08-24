using System;

namespace Sero.Mapper;

public interface IMappingHandlerBuilder
{
	Type SourceType { get; }
	Type DestinationType { get; }

	MappingHandler Build();
}
