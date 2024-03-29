# Ripple
 Ripple is a systems level programming language, that has the static memory safety of Rust, with the syntax of C#, and many of the powerful generics/templates features of C++.

### Primary Features:
- Static memory safety (with lifetime inference)
- C# like syntax
- Turing complete generics
- OOP with classes
- Memory safe lambdas
- Module and Package system
- C and C++ Interoperability 
- Traits and Type-classes with Interfaces
- Rust like move, copy, and clone semantics 

### Examples:

Hello World:
```swift
using module Core.IO;

func Main() -> void
{
  Console.PrintLine("Hello World!");
}
```

External Code and Generics:
```swift
extern "C" malloc(usize size) -> mut void*;
extern "C" free(void* ptr) -> void;

module Core.Memory
{
  unsafe func New<T, TArgs...>(TArgs args...) -> T*
    where T is IConstructable<TArgs...>
  { 
    T mut* ptr = malloc(sizeof<T>()) as T mut*;
    ptr := T(args...);
    return ptr;
  }

  unsafe func<T> Delete(T mut* ptr) -> void
  {
    comptime if(T is Destructible)
      ptr->~T();

    free(ptr as void*);
  }
}
```

Classes and Data Structures:
```swift
using module Core.Memory;

module Core
{
  class Unique<T> : IDereferencable<T>
  {
    private unsafe T* ptr;
    public Unique<TArgs...>(TArgs args...) 
      where T is IConstructable<TArgs...>
    {
      unsafe
      {
        ptr = New<T>(args...);
      }
    }
    
    public func Indirect(this ?mut&) -> T& { return ptr as T ?mut&; }

    public ~Unique()
    {
      unsafe
      {
        Delete(ptr);
      }
    }
  }
  
  impl<T> ICloneable for Unique<T> where T is ICloneable
  {
    public func Clone(this&) -> Unique<T>
    {
      return Unique(ptr->Clone());
    }
  }
}
```

### Notes:
- Ripple is *EXTREMELY* incomplete, and 90% of the above features are not implemented yet
- [Here is a partial documentation of the current features of Ripple](https://github.com/FishArmy100/Ripple/blob/main/Ripple/Info/Current-Features.md)
