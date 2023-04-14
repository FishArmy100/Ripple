# Ripple Interfaces
Ripple interfaces are basically a combination of rusts traits, csharps interfaces, and c++s concepts. They have two different, for lack of a better word, 'modes'. One, requires a explicit implementation of the interface, but, allows for runtime polymorphism. The other, does not require a explicit implementation, but can only be run at compile time.

## Implicit Interface Implementation:
With implicit interface implementation, the class does not have to explicitly have to implement the interface, but, you can still require a function to have all the functions included the interface.

```cs
interface IStringable
{
  func ToString() -> String;
}

class Player
{
  public String Name = "Bob";
  public func ToString() -> String
  {
    return Name.Clone();
  }
}

// forces any value passed to have the methods defined in Stringable
func Print<T>(T& stringable) -> void where T is IStringable
{
  PrintLine(stringable->ToString());
}
```

## Explicit Interface Implementation:
Interfaces can also be explicitly implemented, either on the class itself, or externally, for extension method like behavior. It also allows for runtime polymorphism, simmilar to virtual methods.

```cs
interface IStringable
{
  func ToString() -> String;
}

class Entity : IStringable
{
  private String Tag = "Entity";
  public func IStringable::ToString() -> String
  {
    return Tag.Clone();
  }
}

class Pet
{
  public String Name = "Leroy";
}

impl IStringable for Player
{
  func ToString() -> String
  {
    return Name.Clone();
  }
}

func Print(IStringable& stringable) -> void // runtime polymorphism
{
  PrintLine(stringable->ToString());
}

// same as implicit interface implementation, but, the impl requires T to explicitly implement IStringable
func OtherPrint<T>(T& stringable) -> void where T impl IStringable
{
  PrintLine(stringable->ToString());
}
```
