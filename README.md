![SERO.Mapper banner](https://i.imgur.com/SQOMHea.png)

# Sero.Mapper
Lightweight mapping organization utility to keep track of type transformations. 

### Why not just use AutoMapper instead?
AutoMapper is a very powerful, battle proven tool, but it&apos;s just an overkill and even inconvenient for some scenarios.

Please have in mind:

- **Readability**: Unless you are mapping completely identical classes, AutoMapper ends up being very verbose, sometimes even more than handmade assignations. Each field you have to specify by hand in AutoMapper its a bloated method chain for that definition, which is basically just a hard to read value assignation.
- **Debugging**: In AutoMapper you define your transformations using a fluent notation and then it processes it in a magic black box. If it happens to break, you rely on AutoMapper&apos;s exception messages, which are generally useful but sometimes can be confusing or misleading. Using SeroMapper, you get the exact line number that causes the exception because its just a regular code block, so you can also set breakpoints inside of it.
- **Performance**: SeroMapper is presumably faster (benchmarks pending), as it lacks a lot of features AutoMapper provides. It doesn&apos;t need to build thransformations since the developer is defining them explicitly (and not via rule definition) and doesn&apos;t rely on reflection as much.

## Installation
The quickest way to get the latest version is to add it to your project using **Nuget** [[Sero.Mapper](https://www.nuget.org/packages/Sero.Mapper/ "Sero.Mapper")]

## Usage
Our goal is to transform instances of the class **User** into instances of **UserDTO**.
```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Birthdate { get; set; }
}

public class UserDTO
{
    public int IdUser { get; set; }
    public string Username { get; set; }
    public DateTime UserBirthday { get; set; }
}
```

-----

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
