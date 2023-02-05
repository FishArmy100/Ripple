# Generic Specialization Examples:

### Function implementation:
```swift
comptime func IsMutable<T>() -> bool { return false; }
impl<T> func IsMutable<mut T>() -> { return true; }

func Test() -> void
{
  PrintLn(IsMutable<int>()); // true
  PrintLn(IsMutable<mut int>()); // false
}
```

### Class Implementations:
```swift
static comptime class TypeUtils<T>
{
  alias Underlying = T;
}

impl<T> class TypeUtils<T&>
{
  alias Underlying = T;
}

func Test() -> void
{
  TypeUtils<int&>.Underlying i = 5;
}
```
