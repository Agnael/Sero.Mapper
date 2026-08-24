using OneOf;
using System;
using System.Collections.Generic;

namespace Sero.Mapper;

public class MappingHandlerBuilder<TSrc, TDest> : IMappingHandlerBuilder
{
	public Type SourceType { get; private set; }
	public Type DestinationType { get; private set; }
	public readonly
	OneOf<
	   ConvertMutable<TSrc, TDest>,
	   ConvertImmutable<TSrc, TDest>,
	   ConvertMutableAsync<TSrc, TDest>,
	   ConvertImmutableAsync<TSrc, TDest>,
	   ConvertImmutableWithBase<TSrc, TDest>,
	   ConvertImmutableWithBaseAsync<TSrc, TDest>
	> Converter;

	private List<Tuple<Type, Type>> _preprocessingMaps { get; set; }

	public MappingHandlerBuilder(
		OneOf<
		   ConvertMutable<TSrc, TDest>,
		   ConvertImmutable<TSrc, TDest>,
		   ConvertMutableAsync<TSrc, TDest>,
		   ConvertImmutableAsync<TSrc, TDest>,
		   ConvertImmutableWithBase<TSrc, TDest>,
		   ConvertImmutableWithBaseAsync<TSrc, TDest>
		> converter)
	{
		SourceType = typeof(TSrc);
		DestinationType = typeof(TDest);
		Converter = converter;

		this._preprocessingMaps = new List<Tuple<Type, Type>>();
	}

	public MappingHandlerBuilder<TSrc, TDest> PreprocessWith<TSrcParent, TDestParent>()
		where TSrcParent : TSrc
		where TDestParent : TDest
	{
		_preprocessingMaps.Add(
			new Tuple<Type, Type>(typeof(TSrcParent), typeof(TDestParent))
		);

		return this;
	}

	public MappingHandler Build()
	{
		return MappingHandler.Make<TSrc, TDest>(
			Converter,
			_preprocessingMaps
		);
	}
}
