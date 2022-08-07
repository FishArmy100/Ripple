

# Ripple Language Specification
## Info:
Ripple is a low level OOP langauge, that is designed to be an improvment of c++. There have been many attempts at this, and Ripple probably wont be the last, but the idea is to make a language extreamly simmilar to c++, without all of the janky wierdness. ie: c-style casting, function pointer syntax, typedef, header files, c-style macros, etc. Lord willing, it may even be able to communicate with c++ libraries, but that is currently not a priority.

### **NOTE:** This syntax is subject to change
---
## Examples:
### Hello World:
Your first introduction to any programming language, the classic Hello World example.
```cpp
using module Core;

int Main(const char** vargs)
{
    // same as c++'s std::cout << "Hello World!" << "\n";
    Console.PrintLine("Hello World!");
}
```

### FizzBuzz
The bain of all perspective programming employees, the FizzBuzz example.
```cpp
using module Core;

void FizzBuzz(int count)
{
    for(int i = 0; i < count; i++)
    {
        if(i % 3 == 0 && i % 5 == 0)
            Console.PrintLine("FizzBuzz");
        else if(i % 3 == 0)
            Console.PrintLine("Fizz");
        else if(i % 5 == 0)
            Console.PrintLine("Buzz");
        else
            Console.PrintLine(i);
    }
}
```

### Scoped Pointer:
For a more complex example, and to show the more lower level/advanced aspects of Ripple, basicaly the same thing as a c++ unique_ptr.
```cpp
module Core
{
    public concept Copyable // concepts
    {
        public This(const This&);
    }

    public class Scoped<T> // templates
    {
        private T* m_Ptr; // raw memory mannagement

        public Scoped(T* ptr) : m_Ptr(ptr) {}
        public Scoped<TArgs...>(TArgs... args) : (new T(move args...)) {} // variadic templates
        
        public T& operator->() // overloadable operators
        {
            return &*m_Ptr;
        }

        public T* Get()
        {
            return m_Ptr;
        }

        public const T& operator->() const
        {
            return &*m_Ptr;
        }

        public Scoped<T> Clone() where T is Copyable
        {
            return move Scoped<T>(new T(*m_Ptr));
        }

        // explicit move and copy
        public move Scoped(const Scoped&) = default; 
        public copy Scoped(const Scoped&) = delete;

        // Destructor
        public ~Scoped()
        {
            delete m_Ptr;
        }
    }
}
```
---
## Ripple Basics:
### Comments
```cpp
// Here is a single line comment

/*
    Here is a 
    Multiline
    Comment
*/
```
### Primative types:

- Signed Intagers:
    - `i8` = 8 bit signed intager
    - `i16` = 16 bit signed intager
    - `i32` = 32 bit signed intager
    - `i64` = 64 bit signed intager
    - `i128` = 128 bit signed intager
    - `isize` = signed arch
- Unsiged Intagers:
    - `u8` = 8 bit unsigned intager
    - `u16` = 16 bit unsigned intager
    - `u32` = 32 bit unsigned intager
    - `u64` = 64 bit unsigned intager
    - `u128` = 128 bit unsigned intager
    - `usize` = unsigned arch
- Charactors:
    - `c8` = 8 bit charactor
    - `c16` = 16 bit charactor
- Floating point:
    - `f32` = 32 bit floating point IEEE-754 format
    - `f32` = 64 bit floating point IEEE-754 format
- Booleans:
    - `bool` = 8 bit boolean value
- Built in aliases:
    - `char` = c8
    - `int` = i32
    - `float` = f32
    - `byte` = u8

### Literals:
 - 5 = any intager type
 - 6u = any unsigned intager type
 - 4.5f = f32
 - 4.5f64 = f64
 - true/false = bool
 - 'c' = char
 - "Hello World!" = char[12], one extra for the \0 char

 ### Variables:
 Veriable declaration is the same as c++:
 [Type] [Name] = [Value];
 ```cpp
int number = 5;
var letter = 'f'; // automatic type inference with the var keyword

 ```

 ### Arrays:
 Arrays are contigus blobs of memory with a set size. The size of stack allocated arrays must be known at runtime.

 ```cpp
 int[5] array1 = int[5](); // array of 5 intagers

 var = array2 = int[4](42); // array of 4 intagers initialized with the number 42

 char[7] string = { 'H', 'e', 'l', 'l', 'o', '!', '\0' }; // initializer list

 var numbers = { 5u, 6u, 7u }; // type can be infered?

 char c = string[0]; // c == 'H'
 ```
 
### Copying Data
More complex types, like arrays and classes must be explicitly copied. This is to show when things like lists and heap allocated strings are copied, so to make optomization easier.

```cpp
var string = "Hello World!"; // stack allocated string, copying could be quite expencive depending on the size

var newString = copy string; // must be explicitly copied

```

 ### Pointers and References:
 Pointers can point to any block of memory, or a single object. They can be assigned to `nullptr`, witch lets them point to nothing. References point to a single object in memory, and cannot point to `nullptr`

 ```cpp
var array = {5, 6, 7};

int* ptr = &array[0];
int& ref = &array[2];


int value = *ref; // value == 7
int value2 = *ptr; // value2 == 5

int* ptr2 = ptr + 1; // pointer arithmatic
int value2 = *ptr2; // value3 == 6

float* floatPtr = nullptr;
*floatPtr; // UB, or error
 ```

### Moving Data:
Sometimes, you may not want to copy complex data, and you can instead move it. This can be done instead of explicitly copying, but you cannot use the old veriable, otherwise a compiler error will be thrown

```cpp
int value = 5;
int moved = move value; // bad example, but with classes this makes much more sense

// value = 6; -> compiler error, cannot use value after moving

moved = 7; // this is fine

```

 ### Heap Allocation:
 All of the previous examples show how to allocate memory on the stack, the syntax for allocating on the heap is almost exactly the same for c++.

 ```cpp
 int* ptr = new int(5); // value allocated on the heap, and a pointer to it is returned

 delete ptr; // deletes the memory

 float* array = new float[] { 4.5f, 3.7f, 4.6f, 1.55f } // allocates an array on the heap

 delete[] array;

 char* string = new byte[10](0); // allocats a block of 8 bytes on the heap
 ```

### Placement New:
Placement new allows you to allocate memory onto a pointer that already exists. **NOTE:** it does not call the destructor of the object when used

```cpp
int* ptr = new int(5);
new (ptr) int(6); // *ptr == 6
```


