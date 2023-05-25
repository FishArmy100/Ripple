# Classes Examples:
This works for when the Ripple Memory Update is running, probably wont work in the next few updates
```rust
class String
{
    private unsafe char mut* m_data;
    private usize m_length;

    public unsafe String(char* data)
    {
        length = strlen(data);
        capacity = length;

        m_data = malloc(sizeof<char>() * (length + 1)) as char*;
        strcpy(data, m_data);
    }

    public unsafe ~String()
    {
        free(m_data);
    }

    public func length(this&) -> usize { return m_length }
    public unsafe func data<'t>(this&'t) -> char* { return m_data; }

    public func at<'t>(this&'t, usize index) -> char { unsafe { return m_data[index]; }}
    public func at<'t>(this mut&'t, usize index) -> char mut&'t { unsafe {return &mut m_data[index];} }

    public func append<'t, 'o>(this mut&'t, String&'o other)
    {
        unsafe
        {
            char* old = m_data;
            m_data = malloc(sizeof<char>() * (m_length + other.m_length + 1)) as char*;

            strcpy(old, m_data);
            strcpy(other.m_data, m_data + other.m_length);

            m_length += other.m_length;
            free(old);
        }
    }

    public unsafe func append<'t>(this mut&'t, char* other)
    {
        char* old = m_data;
        usize other_len = strlen(other);
        m_data = malloc(sizeof<char>() * (m_length + other_len + 1)) as char*;
        
        strcpy(old, m_data);
        strcpy(other, m_data + other_len);

        m_length += other_len;
        free(old);
    }

    public func clone<'t>(this&'t) -> String
    {
        unsafe 
        {
            return String(m_data);
        }
    }
}

class StringSlice<'a>
{
    private String&'a m_string;
    private usize m_start;
    private usize m_length;

    public StringSlice(String&'a string, usize start, usize length)
    {
        assert(start < string.length());
        assert(start + length - 1 < m_string.length());
        assert(length != 0);

        m_string = string;
        m_start = start;
        m_end = end;
    }

    public char at(this&, usize index)
    {
        assert(index < m_length);
        return m_string.at(index + m_start);
    }

    public func start(this&) -> usize { return m_start; }
    public func length(this&) -> usize { return m_length; }
}
```

