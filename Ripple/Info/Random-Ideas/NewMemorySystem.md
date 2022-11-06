# New Ripple Memory System
## References:
References are the backbone of ripples safe memory management. A reference represents an objects location in memory. What differentiates it from a pointer, is that as long as it is not in an unsafe block, you will never have to deal with Undefined Behavior (UB).
## Rules:
- References:
    - A mutable object can only have one live mutable reference to it
    - You cannot mutate a mutable reference if a liveing immutable object exists
    - A reference cannot live longer than the object it references
- Moveing:
    - Any object that does not implement the `ICopyable` interface, will be moved when assigned.
    - You cannot use a moved object
    - You cannot move an object out of a reference, if you do not own the object
    - You cannot move a member object
- Unsafe Code:
    - Mutability safety requirements of references are removed
    - Lifetime requirements removed???
- Lifetime Inference:
    - The compiler will attempt to infer all lifetimes, and lifetimes rules, of references for non public declarations
    - The compiler will enforce explicit lifetimes for public declarations


## Code:
### Basic Lifetimes:
```swift
public class Ref<T, 'a> : IInderectable<T, 'a>, ICopyable
{
    private T&'a m_Ref;

    public Ref(T&'a ref) : m_Ref(ref) {}

    public func Indirect() -> T&'a {
        return m_Ref; 
    }
}

func DoesntWork<'a>() -> int&'a
{
    return &5; // lifetime of 5, is shorter than the required lifetime
}

func Main() -> void
{
    int i = 0;
    mut Option<Ref<int>> opt = None; // lifetimes will be infered by the compiler
    opt = Ref<int>(i); // lifetime infered
}
```

### Mutability:
```swift
func Main() -> void
{
    mut int i = 5;
    int ci = 42;
    {
        mut int& ref1 = &mut i;
        mut int& ref2 = &mut i; // does not compile

        int& cref = &i;
        *ref1 = 6; // does not compile
    }

    mut int& ref = &mut i; // works fine
    *ref = 6; // also works

    mut int& other = &mut i; // does not compile
}
```

