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
```swift
public class Ref<T, 'a> : IInderectable<T, 'a>, ICopyable
{
    private T&'a m_Ref;

    public Ref(T&'a ref) : m_Ref(ref) {}

    public func Indirect() -> T&'a {
        return m_Ref; 
    }
}


func Main() -> void
{
    int i = 0;
    lifetime l = lifetimeof(i);

    mut Option<Ref<int>> opt = None; // lifetimes will be infered by the compiler
    opt = Ref<int>(i); // lifetime infered
}
```

```swift
// there is a problem with:
public class Test<T>
{
    public T&'lifetimeof(this) Ref;
    public Test(T&'lifetimeof(this) ref) : Ref(ref) {}
}

func HeresYourProblem() -> Test<int>
{
    int i = 5;
    return Test<int>(&i);
}

func Main() -> void
{
    Test<int> problem = HeresYourProblem();
}
```

