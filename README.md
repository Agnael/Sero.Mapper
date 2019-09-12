# Sero.Mapper
Lightweight mapping organization utility to keep track of type transformations. 

### Why not just use AutoMapper instead?
AutoMapper is a very powerful, battle proven tool, but it&apos;s just an overkill and even inconvenient for some scenarios.

Please have in mind:

- **Readability**: Unless you are mapping completely identical classes, AutoMapper ends up being very verbose, sometimes even more than if the mappings were done by hand using simple value assignations. Each field you have to specify by hand in AutoMapper its a long, bloated fluent method chain for that property, which is basically just a hard to read value assignation.
- **Debugging**: In AutoMapper you define your transformations using a fluent notation and then it processes them in a magic black box. If it happens to break, you rely on AutoMapper&apos;s exception messages, which are generally useful but sometimes can be confusing or misleading. Using SeroMapper, you get the exact line that causes the exception because its just a regular code block, so you can also set breakpoints inside of it.
- **Performance**: SeroMapper is presumably faster, since its way simplier, doesn&apos;t need to build thransformations since the developer is defining them explicitly (and not via rule definition) and doesn&apos;t rely on reflection as much. It also lacks a lot of features AutoMapper provides.


### Usage
Our goal is to transform the **User** object into an **UserDTO** object.

- Register your transformation, using the static method **Mapper.CreateMap&lt;SourceType, DestinationType&gt;()**:
```csharp
Mapper.CreateMap<User, UserDTO>(user =>
{
	UserDTO dto = new UserDTO();
	dto.IdUser = user.Id;
	dto.Name = user.Usermame;

	return dto;
});
```

- Use your transformation, using the static method **Mapper.Map&lt;DestinationType&gt;(SourceType obj)**:
```csharp
User userDb = _db.Users.GetById(100);
UserDto userDto = Mapper.Map<UserDTO>(user);
```
