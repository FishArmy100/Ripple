# References:
References and safety inspired by rust:

```swift
safe class Ref<T> : IInderectable // not sure about the safe keyword
{
    private T& m_Ref;

    public Ref(T& ref) : m_Ref(ref) {}
    public func IInderectable::Get() -> T& { return m_Ref; }
}

// Compiler generated code:
class Ref<lifetime 'a, typename T>: IInderectable where 'a >= lifetimeof(this)
{
    private T &'a m_Ref;

    public Ref(T&'a ref) : m_Ref(ref) {}
    public func IInderectable::Get() -> T &'a { return m_Ref; }
}
```