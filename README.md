![Latest Release](https://internetexception.com/wp-content/uploads/datamapper/release.svg) ![Status](https://internetexception.com/wp-content/uploads/datamapper/pipeline.svg) ![Coverage](https://internetexception.com/wp-content/uploads/datamapper/coverage.svg)

# Introduction

HamsterWheel.Data.Mapper package provides a simple way for mapping one object to another with the possibility of
providing the custom mapping rules. The library is compatible with .NET9.
The goal of the project is to be able to provide mapping of objects via rules that can be defined from the outside of the .NET code.
This means that while in other mapping libraries, you define mapping rules in the code, via lambda expressions:
```csharp
Map(c => c.Source(s => s.Name).Destination(d => d.Name);
```
in HamsterWheel.Data.Mapper you can use strings with names of properties to define mapping rules instead.
This way objects can be mapped:
- by logic rules defined by the user via UI
- by rules sent to the API from the HTTP client.

This library is dependent on HLinq, which provides data conversion capabilities for mapper.

This project is part of Hamster Wheel platform, a dynamically configurable, extensible API that aims to be an
easy-to-use, secure solution for data manipulation of your choice. It is intended to be used for personal projects,
hobbyists and small companies.

# Reference links

- [HLinq](https://github.com/npodbielski/HamsterWheel.HLinq)
- [Hamster Wheel](https://internetexception.com/why-hamster-wheel/)

## What's contained in this project

This project contains single library Nuget package:

- main package: HamsterWheel.Data.Mapper that allows mapping one object to another, with optionally custom mapping
  rules.

Navigation:
- [How to use](#how-to-use)
  - [Getting started](#getting-started)
  - [Usage](#usage)
    - [Using without a custom map](#using-without-a-custom-map)
    - [Using without the map with automated conversion](#using-without-the-map-with-automated-conversion)
    - [Mapping nested objects](#mapping-nested-objects)
    - [Mapping with an explicit map](#mapping-with-an-explicit-map)
    - [Customizing implicit map](#customizing-implicit-map)
    - [Matching properties by wildcard](#matching-properties-by-wildcard)
    - [Wildcard usage](#wildcard-usage)
    - [Mapping objects at different levels](#mapping-objects-at-different-levels)
    - [Caching mapping rules](#caching-mapping-rules)
    - [Usage from services](#usage-from-services)
    - [Mapper customization](#mapper-customization)

# How to use

Below you can find instructions on how to get you started using Data Mapper.

## Getting started

To use the mapper, you need to add the package to your project.

```xml

<PackageReference Include="HamsterWheel.Data.Mapper" Version="0.5.1"/>
```

After that, you can write the code that will map one object to another. For example:

```csharp
DataMapper.Map(source, destination);
```

The following method does not return any value but changes the properties of the destination object according to the
source object.

# Usage

Below you will find examples of how to use the mapper in specific scenarios.

## Using without a custom map

For example, if the source object has the following definition:

```csharp
public class Source
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}
```

and the destination object has the following definition:

```csharp
public class Destination
{
    public string LastName { get; set; }
    public int Age { get; set; }
}
```

Using the mapper:

```csharp
DataMapper.Map(source, destination);
```

it will automatically map the `LastName` source property to the `LastName` destination property. The same will happen to
the `Age` properties. `FirstName` will not be mapped because there is nothing to map to.

## Using without the map with automated conversion

For example, if the source object has the following definition:

```csharp
public class Source
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Active { get; set; }
    public int Age { get; set; }
}
```

and the destination object has the following definition:

```csharp
public class Destination
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserStatus Active { get; set; }
    public string Age { get; set; }
}

public enum UserStatus
{
    Active,
    Inactive
}
```

executing `Map` method:

```csharp
DataMapper.Map(source, destination);
```

it will automatically map all the matching properties (by the name and type) by using the appropriate destination setter
methods with values from the source object. Those are `FirstName` and `Lastname` properties.
Those that are partial match, `Active` and `Age`, will be mapped with automated conversion:

- `Active` will be mapped to `UserStatus by calling `Enum.TryParse` method.
- `Age` will be mapped to `string` by calling `int.ToString` method.

There are many built-in conversion methods that can be used for mapping. For example:

- from string to primitive and enum types
- from string to dates and times
- from string to guids
- from one primitive type to another (i.e. int to decimal, double to float, char to string)
- from nullable string to nullable primitive types or nullable enums
- from class to interface that class implements
- from one class to another using JSON serialization

## Mapping nested objects

Mapper tries to map, by default, any existing property to any existing property, with the same name, in the destination
object.
This works too even if properties are custom types.

For example, if the source object has the following definition:

```csharp
public class Source
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address Address { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}
```

And the destination object has the following definition:

```csharp
public class Destination 
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DestinationAddress Address { get; set; }
}

public class DestinationAddress
{
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}
```
Running the following code:
```csharp
DataMapper.Map(source, destination);
```

Will map the first level properties of the source object to the destination object:
- `FirstName`
- `LastName`

`Address` also will be mapped by mapping all the nested properties separately. This means that `Country` property if set on destination, considering that do not have similar property on source, won't be mapped and will retain its original value.
This will also work if there is 3rd or more nested properties. In example if `Source` class will have `Address` property which type have of `Contact` with another set of properties:

```csharp
public class Source
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address Address { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public Contact Contact { get; set; }
}

public class Contact
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

And we will have similar structure of `Destination` type:
```csharp
public class Destination
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DestinationAddress Address { get; set; }
}

public class DestinationAddress
{
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public PersonOfContact Contact { get; set; }
}

public class PersonOfContact
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```
Running the following code:
```csharp
DataMapper.Map(source, destination);
```
will map `Contact` to `PersonOfContact`.

## Mapping with an explicit map
If source and destination types are not compatible in a way for a mapper to automatically map every property you need to map you can define your custom map. For example, if the source is:

```csharp
public class Source 
{
    public string FullName { get; set; }
}
```
and destination is:
```csharp
public class Destination 
{
    public string Name { get; set; }
}
```
With instances of those types, running mapper without a map will do nothing because there is no compatible properties.
To correct this when we want to map `FullName` to `Name` we need to define the map:
```csharp
DataMapper.Map(source, destination, [("FullName", "Name")]);
```
This will map `FullName` to `Name`. 
An important thing to note is that providing an explicit map will only map properties mentioned in the map. Other properties will not be mapped.
The same can be achieved by using the wildcard:
```csharp
DataMapper.Map(source, destination, [("*Name", "Name")]);
```

## Customizing implicit map
To map properties implicitly but to still be able to provide an explicit map for incompatible properties, you can use any-to-any map wildcard (used by default when no map is provided), with an additional list of explicit mappings:
```csharp
DataMapper.Map(source, destination, [("*","*"), ("FullName", "Name")]);
```
Above will map all properties implicitly and also map `FullName` to `Name`. For example, it will work for the following types:
```csharp
public class Source
{
    public string FullName { get; set; }
    public int Age { get; set; }
    public bool Active { get; set; }
}

public class Destination
{
    public string Name { get; set; }
    public int Age { get; set; }
    public bool Active { get; set; }
}
```
and will map:
- `FullName` to `Name`
- `Age` to `Age`
- `Active` to `Active`

## Matching properties by wildcard

If you have several properties named similarly, for example, with the same prefix:
```csharp
public class Source
{
    public string Name { get; set; }
    public string PropString { get; set; }
    public int PropInt { get; set; }
    public bool PropBool { get; set; }
    public byte PropByte { get; set; }
}
public class Destination
{
    public string Name { get; set; }
    public string PropString { get; set; }
    public int PropInt { get; set; }
    public bool PropBool { get; set; }
    public byte PropByte { get; set; }
}
```
And you want to map all of them to the same properties on the destination object, you can use the wildcard:
```csharp
DataMapper.Map(source, destination, [("Prop*", "Prop*")]);
```
This will map all properties starting with `Prop` to properties with the same name on the destination object.

## Wildcard usage
Wildcard can be used at the start or end of the mapping rule.
```csharp
DataMapper.Map(source, destination, [("Prop*", "Prop*")]);//matching by prefix
```
```csharp
DataMapper.Map(source, destination, [("*Name", "Name")]);//matching by suffix
```
Wildcard can be also used in the middle of the mapping rule.
```csharp
DataMapper.Map(source, destination, [("*", "User*Address")]);//matching all user addresses: UserResidenceAddress, UserWorkAddress 
```

## Mapping objects at different levels
Usage of the wildcards, with explicit mapping rules allows flattening the source object into the destination object. It is also possible to map flat objects into finer data structures.
For example:
```csharp
public class UserWithAddress
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AddressStreet { get; set; }
    public string AddressCity { get; set; }
}
public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address Address { get; set; }
}
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}
```
If you want to map `UserWithAddress` to `User` with address data inside a sub-object you can run mapper with the following map:
```csharp
DataMapper.Map(source, destination, [("*", "*"), ("AddressCity", "Address.City"), ("AddressStreet", "Address.Street")]);
```
Similar can be done in reverse to map `User` to `UserWithAddress` and flatten the address data:
```csharp
DataMapper.Map(source, destination, [("*", "*"), ("Address.City", "AddressCity"), ("Address.Street", "AddressStreet")]);
```
If flattened object structure would not have `Address` suffix in properties:
```csharp
public class UserWithAddress
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
}
```
map could be simplified to:
```csharp
DataMapper.Map(source, destination, [("*", "*"), ("Address.*", "*")]);
```

## Setting property value via mapper
Mapper allows setting property dynamically in an object by providing a specific map.
In example:

```csharp
DataMapper.Map(source, destination, [(".", "DestinationProperty")]);
```

will set the entire source object as the value of the destination property. In short: the above code is equiavalent to: `destination.DestinationProperty = source;`

## Caching mapping rules

By default, when you execute `Map` method, the mapper will create a new map definition every time. This can be a performance issue if you map objects very often (map creation uses reflection and creates delegates for setter and getter methods) 
To avoid this, you can cache the map definition and reuse it for subsequent calls. For example, if you do not have custom mapping rules:
```csharp
var map = DataMapper.BuildMap(typeof(Source), typeof(Destination);
```
and then you can reuse the map definition:
```csharp
DataMapper.Map(source, destination, cachedMap: map);
```
This will cause the mapper to be just 50–100 slower than doing manual mapping like this (which will take microseconds):
```
destination.FullName = source.Name;
destination.Age = int.Parse(source.Age);
destination.Active = source.Active;
```

Without the cached map, for example, one million map calls will take several seconds, while executing the same with the cached map will take less than a second. The performance difference is significant.

## Usage from services

Using static methods from `DataMapper` class is not necessary. You can use it from a dependency injection container. For example, from `Program.cs`:

```csharp
builder.Services.ConfigureDataMapper();
```

will add Data Mapper services to the dependency injection container. Then you can resolve it inside any other class via constructor injection:

```csharp
public class MyController(IDataMapper mapper)
{
    [HttpGet]
    public void Get([FromBody] Source source)
    {
        var destination = new Destination();
        mapper.Map(source, destination);
        return Json(destination);
    }
}
```

## Mapper customization

By adding Data Mapper services to the dependency injection container, you can customize the behavior of the mapper. For example, if you have custom type that have data available via other means than properties, you can write your own `PropertyAccessor` provider. For example, dictionaries have their own provider:

```csharp
public class DictionaryPropertyAccessorProvider : IPropertyAccessorProvider
{
    private readonly Type _supportedType = typeof(IDictionary<string, object>);

    public IPropertyAccessor? GetAccessor(Type type, string propertyName)
    {
        if (!type.IsAssignableTo(_supportedType))
        {
            return null;
        }

        return new PropertyAccessor(_supportedType, typeof(object), PropertyPath.From(propertyName),
            o => ((IDictionary<string, object?>)o)[propertyName],
            (o, v) => ((IDictionary<string, object?>)o)[propertyName] = v);
    }
}
```

If your class can retrieve and mutate data via i.e. `GetData` and `SetData` methods, your provider can look like this:

```csharp
public class MyCustomClassPropertyAccessorProvider : IPropertyAccessorProvider
{
    private readonly Type _supportedType = typeof(MyCustomClass);
    public IPropertyAccessor? GetAccessor(Type type, string propertyName)
    {
        if (!type.IsAssignableTo(_supportedType))
        {
            return null;
        }
        
        return new PropertyAccessor(_supportedType, typeof(object), PropertyPath.From(propertyName),
            o => ((MyCustomClass)o).GetData(propertyName),
            (o, v) => ((MyCustomClass)o).SetData(propertyName, v));
    }
}
```

After that it needs to be registered in the dependency injection container:
```csharp
services.AddSingleton<IPropertyAccessorProvider, MyCustomClassPropertyAccessorProvider>();
```

Data Mapper will use your provider to retrieve data if your `MyCustomClass` will be the source. On the other hand, if this type is the destination mapper will use `MyCustomClass.SetData` method to mutate data.

