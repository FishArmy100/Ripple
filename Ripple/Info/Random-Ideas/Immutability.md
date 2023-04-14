
All veriables are immutable by default.

Variables:
```cs
int val = 5;
mut int val2 = val; // fine

int& r = &5;
mut int& r2 = r; // does not compile
int mut& r3 = r; // does compile
```

Classses:
```cs
class Example : ICopyable
{
    public mut int& Ref;
    public mut int Mutable;
    public int Normal;
}

Example e1 = {};
e1.Ref = &5; // does not compile
*e1.Ref = 5; // compiles
e1.Mutable = 6; // compiles
e1.Normal = 7; // doesnt compile


mut Example e2 = {};
e1.Ref = &5; // compiles
*e1.Ref = 5; // compiles
e1.Mutable = 6; // compiles
e1.Normal = 7; // compiles
```
