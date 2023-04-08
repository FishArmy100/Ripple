# Current Language Features:
## Builtin Types:
Ripple currently supports:
- `int`s (32 bit signed intager)
- `float`s (32 bit IEEE floating point)
- `bool`s (8 bit value that can be `true` or `false`)
- `char`s (8 bit ASCII compliant character)
  - Please note, this is subject to change, and will be replaced by a 16 bit characters that are utf compliant

## Expressions:
### Binary Operators:
- `+`: Add
- `-`: Subtract 
- `*`: Multiply
- `/`: Divide
- `%`: Remainder/Modulus
- `&&`: Logical And
- `||`: Logical Or
- `<`, `>`, `<=`, `>=`: Comparison 
- `==`, `!=`: Equality
### Unary Operators:
- `&`: Immutable reference of
- `&mut`: Mutable reference of
- `-`: Negation 
- `!`: Inverse 
- `*`: Dereference
### Call;
Any function, or function pointer can be called.
`expression "(" comma seperated arguments ")"`
```swift
func Add(int a, int b) -> int
{
  return a + b:
}

func(int,int)->int func_ptr = Add;
int v1 = Add(5, 6); // v1 == 11
int v2 = func_ptr(2, 3); // v2 == 5
```
