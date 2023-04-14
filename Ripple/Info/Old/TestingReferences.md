

Interesting quirk about having explicit move and copy
```cs
public class Scoped<T> // templates
{
    private T* m_Ptr; // raw memory mannagement

    public Scoped(T* ptr) : m_Ptr(ptr) {}
    public Scoped<TArgs...>(TArgs... args) : (alloc<T>(1) := T(move args...)) {} // variadic templates
    
    public const func operator->() -> const ref T // overloadable operators
    {
        return *m_Ptr;
    }

    public func operator->() -> ref T
    {
        return *m_Ptr;
    }

    public func Clone() -> Scoped<T> where T is Copyable
    {
        return Scoped<T>(malloc(sizeof(T)) := T(*m_Ptr));
    }

    // explicit move and copy
    public move Scoped(const ref Scoped other)
    {
        m_Ptr = other.m_Ptr; // error, other.m_Ptr is techncially const
    }

    public copy Scoped(const ref Scoped) = delete;

    // Destructor
    public ~Scoped()
    {
        comptime if(T is Destructable)
            m_Ptr->~T();

        free(m_Ptr);
    }
}
```