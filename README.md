![SERO.Mapper banner](https://i.imgur.com/SQOMHea.png)

# Sero.Mapper
Lightweight mapping organization utility to keep track of type transformations. 

**Index**    
&nbsp;&nbsp;[**The problem**](#the-problem)  
&nbsp;&nbsp;[**Known solutions**](#known-solutions)  
&nbsp;&nbsp;&nbsp;&nbsp;[Repeating assignations whenever needed](#Repeating-assignations-whenever-needed)  
&nbsp;&nbsp;&nbsp;&nbsp;[Assembler classes](#Assembler-classes)  
&nbsp;&nbsp;&nbsp;&nbsp;[AutoMapper](#AutoMapper)  
&nbsp;&nbsp;[**The Sero.Mapper solution**](#The-SeroMapper-solution)  
&nbsp;&nbsp;&nbsp;&nbsp;[Pros and Cons](#Pros-and-cons)  
&nbsp;&nbsp;[**Usage**](#Usage)  

**Installation**
&nbsp;&nbsp;The quickest way to get the latest version is to add it to your project using **Nuget** [[Sero.Mapper](https://www.nuget.org/packages/Sero.Mapper/ "Sero.Mapper")]

## The problem
Let&apos;s say we receive a new customer order, in the OrderDTO format. We need to store it in our DB but in order to do that, our ORM forces us to **convert the OrderDTO instance into an Order instance** before trying to save it.

As you can see, to be able to convert an OrderDTO, we also need to be able to convert it&apos;s complex properties. 

![Example classes diagram](https://i.imgur.com/172n43G.png)

## Known solutions
### Repeating assignations whenever needed
Each time you need this conversion, you manually map each property:

*OrderManager.cs* 
<pre lang="csharp">
public class OrderManager
{
   ...
   /// Saves a new Order. 
   public void SaveOrder(OrderDTO dto)
   {
      Order order = new Order();
      
      if(dto.User != null)
         order.IdUser = dto.User.UserId;

      if(dto.Items != null)
         order.OrderItems = dto.Items.Select(x => new OrderItem { IdItem = x }).ToList();

      _db.Orders.Add(order);
      _db.SaveChanges();
   }
   ...
}
</pre>

Of course, this approach is the worst one.

--- 

### Assembler classes
All the conversions are centralized in specialized classes:

*OrderManager.cs*
<pre lang="csharp">
public class OrderManager
{
   ...
   /// Saves a new Order. 
   public void SaveOrder(OrderDTO dto)
   {
      Order order = _entityAssembler.OrderDtoToOrderEntity(dto);
      _db.Orders.Add(order);
      _db.SaveChanges();
   }
   ...
}
</pre>

*EntityAssembler.cs*
<pre lang="csharp">
public class EntityAssembler
{
   ...
   public Order OrderDtoToOrderEntity(OrderDTO dto)
   {
      Order order = new Order();

      if(dto.User != null)
         order.IdUser = dto.User.UserId;

      if(dto.Items != null)
         order.OrderItems = dto.Items.Select(x => new OrderItem { IdItem = x }).ToList();
		
      return order;
   }
   ...
}
</pre>

This approach can work, but it&apos;s usually verbose and your team must have a very clear naming concensous for the methods in the Assembler. Otherwise, you will end up with duplicate conversions registered, with slightly different names, or even names in different languages if your team is not english native.

---

### AutoMapper
A very powerful tool, you are pretty much covered if you go this route. However, in our specific (but not rare) case, AutoMapper is not all that helpful:
- Since the property names are not identical, nor nested in the same way, AutoMapper will not be able to automatically infer the transformation and we&apos;ll have to define it manually instead.
- Manual definitions in AutoMapper are verbose, sometimes even more than if they are made with simple assignations, since it ends up looking like a text wall, quite difficult to read in transformations for big classes.
- If you screw up anything in the definition you rely solely on AutoMapper&apos;s runtime error messages, which are very good, but sometimes they can be unclear or even misleading. 

*OrderManager.cs*
<pre lang="csharp">
public class OrderManager
{
   ...
   /// Saves a new Order. 
   public void SaveOrder(OrderDTO dto)
   {
      Order order = Mapper.Map&lt;Order&gt;(dto);
      _db.Orders.Add(order);
      _db.SaveChanges();
   }
   ...
}
</pre>

*DtoToEntityProfile.cs*
<pre lang="csharp">
public class DtoToEntityProfile : Profile
{
   public DtoToEntityProfile()
   {
      CreateMap&lt;OrderDTO, Order&gt;()
         .ForMember(dest => dest.IdUser, dto => dto.PreCondition(x => x.User != null))
            .ForMember(dest => dest.IdUser, dto => dto.MapFrom(x => x.User.UserId))
         .ForMember(dest => dest.OrderItems, dto => dto.PreCondition(x => x.Items != null)
	    .ForMember(dest => dest.OrderItems, dto => dto.Items.Select(x => new OrderItem { IdItem = x }).ToList())
         .ForMember(dest => dest.User, dto => dto.Ignore());
   }
}
</pre>

&nbsp;
  
## The Sero.Mapper solution
It&apos;s a middle ground between Assemblers and the AutoMapper kind of solution.

This example is a quick overview of the end result of using Sero.Mapper. Detailed setup instructions can be found in the next section.

*OrderManager.cs*
<pre lang="csharp">
public class OrderManager
{
   ...
   /// Saves a new Order. 
   public void SaveOrder(OrderDTO dto)
   {
      Order order = _mapper.Map&lt;Order&gt;(dto);
      _db.Orders.Add(order);
      _db.SaveChanges();
   }
   ...
}
</pre>

*EntityMappings.cs*
<pre lang="csharp">
public class EntityMappings : IMappingSheet
{
   public void EntityMappings(IMapperBuilder builder)
   {
      builder.CreateMap&lt;OrderDTO, Order&gt;((entity, dto) => 
      {
         if(dto.User != null)
            entity.IdUser = dto.User.UserId;

         if(dto.Items != null)
            entity.OrderItems = dto.Items.Select(x => new OrderItem { IdItem = x }).ToList();
      });
   }
}
</pre>

&nbsp;

### Pros and Cons  
**PROS**  
&nbsp;&nbsp;- **Organization**: Your team now have a structure to follow, a key advantage when using AutoMapper that we really like and wanted to keep in this new mapper. The way of defining mappings and using them is normalized by the library and no team member can get creative and screw something up or waste his time creating a duplicate, like when creating Assembler methods.  
&nbsp;&nbsp;- **Convenience**: In order to improve readability and writing speed, the instantiation and the returning of the destination instance is handled by Sero.Mapper, so you only need to write the actual assignations.    
&nbsp;&nbsp;- **Debugging**: If any error arises, you get the exact line where the exception was thrown and you can put breakpoints inside of the mappings to debug them on runtime.  

**CONS**
&nbsp;&nbsp;- If your source and destination types are identical or you can trust that follow defined property naming patterns, you will still have to write all of the assignations one by one. AutoMapper would infer them automatically.

## Usage

1. Create one or many mapping collection sheets, implementing the **IMappingSheet** interface:
```csharp
public class YourMappings : IMappingSheet
{
   public void MappingRegistration(IMapperBuilder builder)
   {
      builder.CreateMap<User, UserDTO>((user, userDto) => 
      {
         userDto.IdUser = user.Id;
         userDto.Username = user.Name;
         userDto.UserBirthday = user.Birthdate;
      });
   }
}
```

1. Instantiate a new SeroMapper instance using the **MapperBuilder** class and register your mapping sheet:
```csharp
IMapper mapper = new MapperBuilder()
                        .AddSheet<YourMappings>()
                        .Build();
```
*You can also register mappings without a sheet, using the same **CreateMap** method of the MapperBuilder, used in the sheet implementations.*

1. Use your transformation, using the **Map&lt;DestinationType&gt;** method:
```csharp
User user = new User { Id = 100, Name = "Test user", Birthdate = DateTime.UtcNow.AddYears(-20) };
UserDto userDto = mapper.Map<UserDTO>(user);
```
