using System;
using System.Collections.Generic;

namespace Sero.Mapper;

/// <summary>
/// Implement this interface to register mapping transformations.
/// </summary>
public interface IMappingSheet
{
   /// <summary>
   /// This method will be called to register custom mappings into an existing MapperBuilder.
   /// </summary>
   /// <param name="builder">
   ///     MapperBuilder instance, use it to create new mapping transformations.
   /// </param>
   void MappingRegistration(MapperBuilder builder);
}
