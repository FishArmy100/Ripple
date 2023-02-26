
Ripple Example with current syntax
```swift
extern "C" func core_print_number_compiler_debug(int i) -> void;

func Fib(int count) -> int
{
    if(count <= 0)
        return 0;

    if(count == 1)
        return 1;

    mut int first = 1;
    mut int second = 1;
    for(mut int i = 0; i < count; i = i + 1)
    {
        int temp = second;
        second = first + second;
        first = temp;
    }

    return second;
}

func main() -> int
{
    int i = Fib(5);
}
```