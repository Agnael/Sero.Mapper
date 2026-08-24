using System;
using System.Collections.Generic;
using System.Linq;

namespace Sero.Mapper;

public class MappingCollectionBuilder : SynchronizedCollectionBase<IMappingHandlerBuilder>
{
	public new void Add(IMappingHandlerBuilder mappingBuilder)
	{
		// Check-then-act under one lock; see MappingCollection.Add.
		lock (SyncRoot)
		{
			bool isDuplicate =
			   this.Any(
				  existingMapping =>
					 existingMapping.SourceType == mappingBuilder.SourceType &&
					 existingMapping.DestinationType == mappingBuilder.DestinationType
			   );

			if (isDuplicate)
			{
				throw new MappingCollectionDuplicateException(
					mappingBuilder.SourceType,
					mappingBuilder.DestinationType
				);
			}

			base.Add(mappingBuilder);
		}
	}

	public IMappingCollection Build()
	{
		IMappingCollection mappingCollection = new MappingCollection();

		foreach (IMappingHandlerBuilder handlerBuilder in this)
		{
			mappingCollection.Add(handlerBuilder.Build());
		}

		return mappingCollection;
	}
}
