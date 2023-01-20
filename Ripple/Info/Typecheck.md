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
mut int i = 5;

{
    int&& r = &&i;
    i = 6; // error
}

i = 6; // fine
```

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
