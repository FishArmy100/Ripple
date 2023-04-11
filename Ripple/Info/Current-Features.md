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
### Call:
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
### Index:
Any array or pointer can be indexed with an intager expression
`expression "[" argument "]"`
- Note: when unsigned intagers, and size types are added, it will replace intagers as the index argument for arrays and pointers
```swift
int[4] array = {1, 2, 3, 4};
print(array[0]); // prints out 1
```
## Variables
All variables can be defined with the following syntax: `typename varname "=" expression`.
```c
int i = 5;
int[10] array = {5, 6, 7, 8};
int& ref = &5;
```
### Mutability
Variables can optionally be made mutable. This allows the user to change the variable, or to take a mutable reference from it.
```rust
mut int number = 5;
number = 6;
int mut& ref = &mut number; // syntax still in development
*ref = 5;
```

### Lifetimes:
All variables have a lifetime, or how long they live, this allows references to make sure that at compile time the compiler can make sure you dont have any dangling pointers.
```rust
mut int& number = &5;
{
  number = &6; // error, the expression does not live long enouph.
}
```
As a rule of thumb, a reference which points to a variable with a longer lifetime, can be implicitly cast to a reference that points to a shorter lifetime.
```rust
int& number = &6;
{
  int& other = number; // compiles with no errors
}
```
