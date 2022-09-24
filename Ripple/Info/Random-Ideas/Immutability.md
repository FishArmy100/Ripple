
All veriables and functions are immutable by default.

```cs
int i = 15;
i = 4; // error

mut int i = 14;
i = 6; // this compiles
```

```cs
class Example
{
    public int Number = 8;
    public mut int value = 6; // cant do this???
}


int main()
{
    mut Example mutableExample = Example();
    Example example = Example();

    mutableExample.Number = 7; // Does work
    example.Number = 7; // Does not work
}
```
