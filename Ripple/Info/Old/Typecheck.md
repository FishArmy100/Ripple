## Borrow Checking
### Rules:
- You cannot move from a mutable reference
- You cannot have multiple live mutable references to the same object
- You cannot mutate a mutable reference while there is a live const reference
- You cannot use a moved value
- You cannot use a uninitalized variable
- Non-Lexical lifetimes??? 

### Examples:
```rs
mut int& r1 = &mut 5;
mut int& r2 = &mut *r1;
```

```rs
func Add<'a>(int&'a a, int&'a b) -> int { return *a + *b; }
func GetRef<'a>(int&'a a) -> int&'a { return a; }

mut int i = 5;

int sum = Add(&i, &i);

i = 6; // fine

GetRef(&i);

i = 6; // fine

int& i = GetRef(&i);

i = 6; // error
```
Notes: 
- A r-value only needs reference tracked ONLY if:
    - It is referenced
    - It survives being passed to a function
    - It is bound to a variable

## Functions with Lifetimes 
### Generic lifetimes: Work with any given lifetimes
```swift
func Add<'a>(int&'a a, int&'a b) -> int
{
    return *a + *b;
}

func<'a>(int&'a, int&'a)->int f = Add<'a>; // works with any lifetime 'a
```
### Spisific lifetimes
```swift
func Add<'a>(int&'a a, int&'a b) -> int
{
    return *a + *b;
}

func Test<'a, 'b>(int&'a a, int&'b b) -> void
{
    func(int&,int&)->int math = Add<'a>;

    math(a, a); // works
    math(b, b); // does not compile, as math is a func<'a>(int&'a, int&'a)->int type
}
``` 
