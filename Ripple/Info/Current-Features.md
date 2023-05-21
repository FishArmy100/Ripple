# Current Language Features:
## Builtin Types:
Ripple currently supports:
- `int`s (32 bit signed intager)
- `float`s (32 bit IEEE floating point)
- `bool`s (8 bit value that can be `true` or `false`)
- `char`s (8 bit ASCII compliant character)
  - Please note, this is subject to change, and will be replaced by a characters and strings that are  UTF-8 compliant.

## Expressions:
### Literals:
- `5`: Intager
- `3.14159`: Floating Point
- `true`/`false`: Boolean
- `'c'`: Charactor
- `c"Hello World!"`: C String
- `"Hi"`: Strings **Not Supported Yet**
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
Pointers can also be indexed
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
int mut number = 5;
number = 6;
int mut& ref = &mut number; // syntax still in development
*ref = 5;
```

### Lifetimes:
All variables have a lifetime, or how long they live, this allows references to make sure that at compile time the compiler can make sure you dont have any dangling pointers.
```rust
int& mut number = &5;
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
## Functions:
Functions can be defined as: `"func " funcname ("<" lifetime ("," lifetime)* ">")? "(" (typename varname "," (typename varname)*)? ")" "->" typename "{" statement* "}"`. `void` as the return type name means that the function does not return a value.
Example:
```swift
func example_func(int x, int y) -> int 
{
  return x + y;
}

print(example_func(5, 6));
```

### Calling Functions:
Functions can be called as shown above. Lifetime arguments are not used in the function call, and are inferred based on the lifetimes of the given arguments.

### Functions Lifetimes:
When declaring functions, with references, you must manually add a lifetime to each reference in the parameter list.
```rust
func increment<'a, 'b>(int mut&'a incrementee, int&'b value) -> void
{
  *incrementee = *incrementee + *value;
}
```

### Function Overloading:
Functions can have the same name, as long as they do not have the same parameters. Lifetime parameters do not count to this distinction. 
