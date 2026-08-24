using Microsoft.Extensions.Logging;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sero.Mapper;

/// <summary>
///     Registers and holds mapping transformations to finally create a Mapper instance with them.
/// </summary>
public class MapperBuilder
{
	private readonly ILogger _logger;
	private readonly IServiceProvider _serviceProvider;
	private MappingCollectionBuilder _mappingCollectionBuilder;

	public MapperBuilder(ILogger logger, IServiceProvider serviceProvider)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
		_mappingCollectionBuilder = new MappingCollectionBuilder();
	}

	/// <summary>
	///     Takes an IMappingSheet implementing class and registers it's mapping definitions.
	/// </summary>
	/// <param name="sheet">
	///     IMappingSheet implementing class.
	/// </param>
	public MapperBuilder AddSheet(IMappingSheet sheet)
	{
		sheet.MappingRegistration(this);
		return this;
	}

	/// <summary>
	///     Convenience method that only needs the IMappingSheet implementation class name to register it's definitions.
	/// </summary>
	/// <typeparam name="TSheet">
	///     A class that implements IMappingSheet.
	/// </typeparam>
	public MapperBuilder AddSheet<TSheet>() where TSheet : IMappingSheet
	{
		IMappingSheet instance = Activator.CreateInstance<TSheet>();
		return AddSheet(instance);
	}

	/// <summary>
	///   Creates a single mapping definition for the SOURCE-DESTINATION types provided.
	///   This method overload allows you to register a transformation lambda that receives the current 
	///   Mapper instance, to execute mappings in the lambda.
	/// </summary>
	/// <typeparam name="TSrc">
	///   Source type that the transformation should receive.
	/// </typeparam>
	/// <typeparam name="TDest">
	///   Destination type that the transformation must return.
	/// </typeparam>
	/// <param name="funcMask">
	///   Lambda that takes 3 parameters: (TSource src, TDestination dest, Mapper mapper) and defines the
	///   transformation you need. It must not return the resulting transformed "dest" parameter, the 
	///   library takes care of that.
	/// </param>
	public MappingHandlerBuilder<TSrc, TDest> CreateMap<TSrc, TDest>(ConvertMutable<TSrc, TDest> funcMask)
	{
		MappingHandlerBuilder<TSrc, TDest> newMappingBuilder = new MappingHandlerBuilder<TSrc, TDest>(funcMask);
		_mappingCollectionBuilder.Add(newMappingBuilder);
		return newMappingBuilder;
	}

	public MappingHandlerBuilder<TSrc, TDest> CreateMap<TSrc, TDest>(ConvertMutableAsync<TSrc, TDest> funcMask)
	{
		MappingHandlerBuilder<TSrc, TDest> newMappingBuilder = new MappingHandlerBuilder<TSrc, TDest>(funcMask);
		_mappingCollectionBuilder.Add(newMappingBuilder);
		return newMappingBuilder;
	}

	/// <summary>
	///   Creates a single mapping definition for the SOURCE-DESTINATION types provided.
	/// </summary>
	/// <typeparam name="TSrc">
	///   Source type that the transformation should receive.
	/// </typeparam>
	/// <typeparam name="TDest">
	///   Destination type that the transformation must return.
	/// </typeparam>
	/// <param name="funcMask">
	///   Lambda that takes 3 parameters: (TSource src, TDestination dest) and defines the transformation 
	///   you need. It must not return the resulting transformed "dest" parameter, the library takes care 
	///   of that.
	/// </param>
	public MapperBuilder CreateMap<TSrc, TDest>(ConvertImmutable<TSrc, TDest> funcMask)
	{
		MappingHandlerBuilder<TSrc, TDest> newMappingBuilder = new MappingHandlerBuilder<TSrc, TDest>(funcMask);
		_mappingCollectionBuilder.Add(newMappingBuilder);
		return this;
	}

	public MapperBuilder CreateMap<TSrc, TDest>(ConvertImmutableAsync<TSrc, TDest> funcMask)
	{
		MappingHandlerBuilder<TSrc, TDest> newMappingBuilder = new MappingHandlerBuilder<TSrc, TDest>(funcMask);
		_mappingCollectionBuilder.Add(newMappingBuilder);
		return this;
	}

	public MapperBuilder CreateMap<TSrc, TDest>(ConvertImmutableWithBase<TSrc, TDest> funcMask)
	{
		MappingHandlerBuilder<TSrc, TDest> newMappingBuilder = new MappingHandlerBuilder<TSrc, TDest>(funcMask);
		_mappingCollectionBuilder.Add(newMappingBuilder);
		return this;
	}

	public MapperBuilder CreateMap<TSrc, TDest>(ConvertImmutableWithBaseAsync<TSrc, TDest> funcMask)
	{
		MappingHandlerBuilder<TSrc, TDest> newMappingBuilder = new MappingHandlerBuilder<TSrc, TDest>(funcMask);
		_mappingCollectionBuilder.Add(newMappingBuilder);
		return this;
	}

	/// <summary>
	///   Uses the MapperBuilder configurations to build a Mapper instance.
	/// </summary>
	public Mapper Build()
	{
		IMappingCollection mappingCollection = _mappingCollectionBuilder.Build();
		return new Mapper(_logger, _serviceProvider, mappingCollection);
	}
}
