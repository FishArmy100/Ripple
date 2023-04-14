```swift
enum Option<T>
{
    Some: T,
    None
}

using Option::*;

class StringReader
{
    private String& m_Source;
    private usize m_Index;

    public StringReader(String& str) : m_Source(str), m_Index(0) {}

    public func GetSource() -> String& {}
    public func Index() -> usize { return m_Index; }

    public mut func Advance() -> char
    {
        if(AtEnd())
            panic("Tried to advance past the end of the string");

        char c = Current();
        m_Index++;
        return c;
    }

    public mut func Advance(usize count) -> Slice<char>
    {
        if(m_Index + count >= m_Source.Length())
            panic("Tried to advance past the end of the string");
        
        Slice<char> slice = m_Source.Slice(m_Index, count);
        m_Index += count;
        return slice;
    }

    public func Current() -> char { return m_Source[m_Index]; }
    public func Previous() -> char { return m_Source[m_Index - 1]; }
    public func AtEnd() -> bool { return m_Index >= m_Source.Length(); }
}

class Lexer
{

}


```