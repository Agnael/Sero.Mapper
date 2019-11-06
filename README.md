![SERO.Mapper banner](https://i.imgur.com/SQOMHea.png)

# Sero.Mapper
Lightweight mapping organization utility to keep track of type transformations.

This is what you look for if all you need is a clear, simple, straight forward, debuggeable and organized way to write **manual** type mappings.

**Index**    
&nbsp;&nbsp;[**The problem**](#the-problem)  
&nbsp;&nbsp;[**Known solutions**](#known-solutions)  
&nbsp;&nbsp;&nbsp;&nbsp;[Repeating assignations whenever needed](#Repeating-assignations-whenever-needed)  
&nbsp;&nbsp;&nbsp;&nbsp;[Assembler classes](#Assembler-classes)  
&nbsp;&nbsp;&nbsp;&nbsp;[AutoMapper](#AutoMapper)  
&nbsp;&nbsp;[**The Sero.Mapper solution**](#The-SeroMapper-solution)  
&nbsp;&nbsp;&nbsp;&nbsp;[Pros and Cons](#Pros-and-cons)  
&nbsp;&nbsp;[**Get started**](#Get-started)  
&nbsp;&nbsp;&nbsp;&nbsp;[Setting up Sero.Mapper](#Setting-up-SeroMapper)    
&nbsp;&nbsp;&nbsp;&nbsp;[Mapping a single instance](#Mapping-a-single-instance)  
&nbsp;&nbsp;&nbsp;&nbsp;[Mapping collections](#Mapping-collections)  
&nbsp;&nbsp;&nbsp;&nbsp;[Overwriting an already existing destination instance](#Overwriting-an-already-existing-destination-instance)      
&nbsp;&nbsp;&nbsp;&nbsp;[Executing mappings inside of a transformation definition](#Executing-mappings-inside-of-a-transformation-definition) 

**Installation**  
&nbsp;&nbsp;The quickest way to get the latest version is to add it to your project using **Nuget** [[Sero.Mapper](https://www.nuget.org/packages/Sero.Mapper/ "Sero.Mapper")]

&nbsp;

## The problem
Let&apos;s say we receive a new customer order, in the OrderDTO format. We need to store it in our DB but to do that, our ORM forces us to **convert the OrderDTO instance into an Order instance** before trying to save it.

As you can see, to be able to convert an OrderDTO, we also need to be able to convert it&apos;s properties. In this case, it will be rather simple because in order to save it, our ORM just happens to need only the FK ID (Order.IdUser) and we can ignore the Order.User property for saving, since in this case we assume this user already exists in the DB.

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

This approach can work, but your team must have a very clear naming concensous for the methods in the Assembler. Otherwise, you will end up with duplicate conversions registered, with slightly different names, or even names in different languages if your team is not english native.

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
**Note that the instantiation and returning of the instance is handled for us.**

&nbsp;

### Pros and Cons  
**PROS**  
&nbsp;&nbsp;- **Organization**: Your team now have a structure to follow, a key advantage when using AutoMapper that I really like and wanted to keep in this new mapper. The way of defining mappings and using them is normalized by the library and no team member can get creative and screw something up or waste his time creating a duplicate, like when creating Assembler methods.  
&nbsp;&nbsp;- **Convenience**: In order to improve readability and writing speed, the instantiation and the returning of the destination instance is handled by Sero.Mapper, so you only need to write the actual assignations.    
&nbsp;&nbsp;- **Debugging**: If any error arises, you get the exact line where the exception was thrown and you can put breakpoints inside of the mappings to debug them on runtime.  

**CONS**  
&nbsp;&nbsp;- You get none of the internal features that AutoMapper or some of the other existing libraries provide. For example, if your source and destination types are identical or you can trust that they follow defined property naming patterns, you will still have to write all of the assignations one by one. AutoMapper would infer them automatically in this case.

&nbsp;

## Get started  
### Setting up Sero.Mapper
1.  Install the "Sero.Mapper" NuGet package using the GUI or executing this command in the package manager console:
```
PM> Install-Package AutoMapper
```

2. Create a new class implementing **IMappingSheet**, it only has 1 method.
For our simple example (tl;dr: [this one](#the-problem)), we&apos;ll need to create a mapping to convert **OrderDTO** instances into **Order** instances: 
<pre lang="csharp">
public class MySheet : IMappingSheet
{
   public void MappingRegistration(IMapperBuilder builder)
   {
      builder.CreateMap&lt;OrderDTO, Order&gt;((src, dest) => 
      {
         if(src.User != null)
            dest.Id = src.User.UserId;

         if (src.Items != null)
            dest.OrderItems = src.Items.Select(x => new OrderItem { IdItem = x }).ToList();
      });
   }
}
</pre>
The **CreateMap&lt;TSource, TDestination&gt;()** method takes the transformation as a lambda, which will define how the source and the destination types should be mapped. 
You can create as many mappings you want in the same MappingRegistration method of the IMappingSheet.

3. Now we have a mapping sheet, but its just a definition, it&apos;s not actually being used. To execute it, we need to create an **IMapper** instance after registering the sheet:
<pre lang="csharp">
IMapper mapper = new MapperBuilder()
                          .AddSheet&lt;MySheet&gt;()
                          .Build();
</pre>
If you are using ASP.NET Core, you can register the IMapper as a singleton service by using the **AddSeroMapper** extension method in your **ConfigureServices** method in the **Startup.cs** file.
<pre lang="csharp">
services.AddSeroMapper(config => config.AddSheet<MainSheet>());
</pre>
In this example we are registering only one mapping sheet, but you can add as many as you need.
Once we have an IMapper instance available, we can start to actually execute our transformations.

### Mapping a single instance
Using our created/injected IMapper instance, just run the conversion with the **Map&lt;TDestination&gt;() **method:

*OrderManager.cs*
<pre lang="csharp">
public class OrderManager
{
   protected readonly IMapper _mapper;
   protected readonly DbContext _db;
   
   public OrderManager(IMapper mapper, DbContext db)
   {
      _mapper = mapper;
	  _db = db;
   }
   ...
   /// Saves a new Order. 
   public void SaveOrder(OrderDTO dto)
   {
      Order order = _mapper.Map&lt;Order&gt;(dto);  // &lt;&lt;&lt;&lt;&lt; CONVERSION
      _db.Orders.Add(order);
      _db.SaveChanges();
   }
   ...
}
</pre>

### Mapping collections
Use the **MapList&lt;TDestination&gt;()** method instead:

*OrderManager.cs*
<pre lang="csharp">
public class OrderManager
{
   protected readonly IMapper _mapper;
   protected readonly DbContext _db;
   
   public OrderManager(IMapper mapper, DbContext db)
   {
      _mapper = mapper;
	  _db = db;
   }
   ...
   /// Saves a new Order. 
   public void SaveOrderList(List&lt;OrderDTO&gt; dtoList)
   {
      ICollection&lt;Order&gt; order = _mapper.MapList&lt;Order&gt;(dtoList);  // &lt;&lt;&lt;&lt;&lt; CONVERSION
      _db.Orders.Add(order);
      _db.SaveChanges();
   }
   ...
}
</pre>


### Overwriting an already existing destination instance
Sometimes, it&apos;s useful to overwrite a destination instance with multiple different sources, which may be filling different properties of the destination.

To provide an already existing destination, pass it as the second parameter of the Map&lt;TDestination&gt;() method, and you&apos;ll get the instance you provided with the transformations made by the mapping, all properties untouched by the transformation will have the original values your instance had.

### Executing mappings inside of a transformation definition
In classes with comples properties, you will sometimes need to execute a mapping inside of a definition. To do that, you can define your transformation providing a lambda with 3 parameters instead of 2, the third one is the current mapper instance.

This example assumes we already have new **OrderAddress** and **OrderAddressDTO** POCO classes created and our **Order** and **OrderDTO** are using them, respectively.
<pre lang="csharp">
public class MySheet : IMappingSheet
{
   public void MappingRegistration(IMapperBuilder builder)
   {
      builder.CreateMap&lt;OrderDTO, Order&gt;((src, dest, mapper) => 
      {
         if(src.User != null)
            dest.Id = src.User.UserId;

         if (src.Items != null)
            dest.OrderItems = src.Items.Select(x => new OrderItem { IdItem = x }).ToList();
	    
	 // Here we use the injected mapper to execute a different mapping inside of this definition,
	 // to get the "OrderAddress" instance from the source OrderAddressDTO property.
	 if(src.AddressInfo != null)
	    dest.Address = mapper.Map&lt;OrderAddress&gt;(src.AddressInfo);
      });
   
      builder.CreateMap&lt;OrderAddressDTO, OrderAddress&gt;((src, dest) => 
      {
         dest.Address = src.StreetName;
	 
	 if(src.City != null)
	    dest.IdCity = src.City.CityId;
      });
   }
}
</pre>
Note that definition order is not important, since it&apos;s evaluated on runtime.
