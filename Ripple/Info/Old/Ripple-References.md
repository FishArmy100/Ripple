## L-Values vs R-Values

### L-Values
L-Values are any `ref`s, or variable declarations. Both of these are L-Values:
```cs
int val = 5;
ref int r = val;

func Bar(ref int i, float x) -> void // both i and x are lvalues
{

}
```
You cannot return an L-Value from a function, and you cannot pass an L-Value to a function. This forces you to either pass a reference, move, or copy an L-Value to a function. The only L-Value that you can return from, or pass to a function, are references.


### R-Values
R-Values or tempararies, are any value that is returned from a function, or constructor call. Here are some examples:

```cs
SomeFunc() // returns an r-value
SomeConstructor(); // returns a r-value
String s = "Hello!";
copy s; // returns an rvalue, as is calling the copy contructor
```

### Examples:
```cs
func Foo(ref String name, Player p) -> String
{
    return copy name; // must return a copy, as name is an l-value
}

String name = "Bob";
// name is passed by reference, so dont need to copy
// p argument is constructed, so does not need to by copied or moved
// 
String s = Foo(name, Player("Bart", 10));
```

As a quick note, all primitive types are copied implicitly. So, you can assign values without having to explicityly `copy` each instance:
```cs
int a = 5;
mut int b = a; // a is implicitly copied here
```


## References
References can be treated like any other value, and are not counted as type qualifiers. And so, cant be passed in as a template argument.

A reference, is basically just a pointer to an object, but with some special querks about it. You can treat it exactly as if it was the object that it is referencing.

```cs
mut int i = 5;
ref mut int r = i;

r = 6; // i also now is 6
```

They can only be used in three spisific instances:
- Binding by reference
- Passing by reference
- Returning by reference


### Bind by reference
Binding by reference allows you to take either a reference, or an l-value, bind it to a veriable, to be used for later.
```cs
int a = 5;
ref int r = a; // binding by reference
```
The reference CANNOT outlive its referenced object, otherwise, you will get a compiler error.

### Pass by reference
One way to pass a value to a funciton, without copying or moving it, is via passing by reference.
```cs

func Print(ref String s) -> void {...}

String s = "Hello World!";
Print(s); // dont have to copy or move it, because passing by reference
```
It is possible to pass a reference to an r-value, as long as the reference is immutable:
```cs
func Test(ref String s) -> void {} // s is immutable

Test(String("Hello!")); // technically referencing an r-value
```

### Return by reference
You can return a reference to an either a global object, or an object that is a member of the methods object. If a value is returned from a non-static member funciton, the compiler enforces that the reference lives, as long as the object does.

```cs
String DefaultPlayerName = "Steve";

class Player
{
    private String m_Name;

    public mut func SetName(ref String name) -> void { m_Name = copy name; }
    public func GetName() -> ref String { return m_Name; } // returns a reference
}

func Test() -> ref String
{
    mut Player p = Player();
    String name = "Bob";
    p.SetName(name);

    ref String nameRef = p.GetName(); // returns a reference
    return nameRef; // does not compile, because nameRef would live longer than its owner
    return DefaultPlayerName; // would compile, because DefaultPlayerName is a global
}

```

### Notes on references:
- You cannot have a reference as a member object
- References cannot be used in type names, or in template arguments, as they are value qualifiers, and not type qualifiers