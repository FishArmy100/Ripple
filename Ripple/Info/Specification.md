

# Ripple Language Specification
## Info:
Ripple is a low level OOP langauge, that is designed to be an improvment of c++. There have been many attempts at this, and Ripple probably wont be the last, but the idea is to make a language extreamly simmilar to c++, without all of the janky wierdness. ie: c-style casting, function pointer syntax, typedef, header files, c-style macros, etc. Lord willing, it may even be able to communicate with c++ libraries, but that is currently not a priority.

## Notes: 
- This syntax is subject to change, and so this may appear quite different in a few months time.
- Any syntax that is extreamly likely to change, will be marked as: **volotile**
- Many features will be missing from the specification at first, like macros, c++ intagration, etc, but may be added later.
- Finally, please forgive the spelling mistakes, I am quite notorius of them
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
The bane of all perspective programming employees, the FizzBuzz example.
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

### Literals: **Volatile**
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

 ### Const:
*Not implemented yet*

 ### Arrays: **Volatile**
 Arrays are contigus blobs of memory with a set size. The size of stack allocated arrays must be known at runtime.

 ```cpp
 int[5] array1 = int[5](); // array of 5 intagers

 var = array2 = int[4](42); // array of 4 intagers initialized with the number 42

 char[7] string = { 'H', 'e', 'l', 'l', 'o', '!', '\0' }; // initializer list

 var numbers = { 5u, 6u, 7u }; // type can be infered?

 char c = string[0]; // c == 'H'
 ```
 
### Control Flow:
There are three primary types of control flow in Ripple, the `if`/`else` statement, the `for` loop, and the `while` loop. All are implemented in the same way as in c++.

If/Else Statments:
```cpp
if(true)
{
    // Do something
}
else if(false)
{
    // Do something else
}
else
{
    // Do this instead
}
```

For Statment:
```cpp
for(int i = 0; i < 5; i++)
{
    // do this until i == 5
}
```

While Statement:
```cpp
bool expr = true;
while(expr)
{
    // do this untill the expression is false
}
```

### Casting:
*Not Implemented*

### Copying Data **Volatile**
More complex types, like arrays and classes must be explicitly copied. This is to show when things like lists and heap allocated strings are copied, so to make optomization easier.

```cpp
var string = "Hello World!"; // stack allocated string, copying could be quite expencive depending on the size

var newString = copy string; // must be explicitly copied

```

 ### Pointers and References:
 Pointers can point to any block of memory, or a single object. They can be assigned to `nullptr`, witch lets them point to nothing. References point to a single object in memory, and cannot point to `nullptr`. Note, you can take a reference from a r-value.

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
One note, is that references are dereferenced implicitly. So, if you had a `String& str`, you could call a method on it like `str.Length()`, without acctually dereferencing the reference. Although, it is entirly possible to do this: `*(str).Length()`. 

### Moving Data: **Volatile**
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
It should be noted, that the size of a heap allocated string does not need to be known at compile time, much like c++ arrays.

### Placement New:
Placement new allows you to allocate memory onto a pointer that already exists. **NOTE:** it does not call the destructor of the object when used

```cpp
int* ptr = new int(5);
new (ptr) int(6); // *ptr == 6
```
---
## Functions:
Although many of these examples show expressions outside of funcitons, expressions can only exist inside of functions. Only declarations can exist inside of either a modules or global scope.
### Function Basics:
Functions are declared almost exactly as in c++, with the syntax `returnType FunctionName(Arg1Type arg1, Arg2Type arg2, ...) { body }`. 
```cpp
int Add(int a, int b)
{
    return a + b; // return statement
}

// call the function
int sum = Add(5, 6); // sum == 11
```
One slight change in how they are used, is that all arguments to the functions, if they are arrays or classes, must be explicitly copied, or moved to the function. This is a bad example, but it showcases the syntax.

**Volatile**
```cpp
int[5] Index(int[5] array, usize index, int value)
{
    array[index] = value;
    return array;
}

// call the function
int[5] array = int[5](0);
array = Index(copy array, 2, 42); // must explicitly copy the array
```

returning a value will automatically copy or move the object, but you can explicitly call it with the `return copy`, or `return move` statements respectavely.

If you pass by reference however, the reference, like the `usize` and `int` are copied implicitly, and so dont require the `copy` keyword. Also, you dont need to return the array to see the results.
```cpp
Index(int[5]& array, usize index, int value)
{
    array[index] = value;
}

// call the function
int[5] array = int[5](0);
Index(&array, 2, 42); // do not need to copy the array, just need to get a reference
```
### Function Overloading:
function with the same name, can be overloaded with different arguments.
```cpp
int Add(int a, int b) { return a + b; }
float Add(float a, float b) { return a + b; }
```

### Function Pointers:
Function pointers allow you to bind a spisific function, with the same signature, to a pointer, then invoke that function throught the pointer.
```cpp
int Add(int a, int b)
{
    return a + b;
}

int Sub(int a, int b)
{
    return a - b;
}

// Function pointer declaration;
int(int, int)* funcPtr = nullptr; // function pointer does not reference any function
funcPtr = &Add; // assigning the function pointer

int value = functPtr(4, 10); // value == 14
```
The function pointer syntax is simmilar to the c++ style, but it should be much easier to reason about.

### Functions Inside of Functions:
You may declare functions inside of other functions. The only difference between these and normal function definitions, is that the defenition must exist before any uses of the function.

```cpp
int Add(int a, int b, int c)
{
    int Sum(int a, int b) { return a + b; }

    return Sum(a, Sum(b, c));
}
```

### Static Function Variables:
Static variables in functions will remain the same value across all instances of the function:

```cpp
void Init()
{
    static bool isInitialzied = false;
    if(!isInitialzied)
    {
        isInitialized = true;
        Core.Console.PrintLine("Initialized!");
    }
}

// "Initialized!" only printed out once
Init();
Init();
```

---
## Classes:
Ripple is an object oriented langauge, so many of the features revolve around using object to encabsulate data, and functionality. Classes are the primary focus of this. a class is a user defined type, that contains other types, and functionality.

### Overview:
Here is a basic example of a class, that represents a an Position in 3D space.

```cpp
class Point
{
    public float X = 0; // if not initialized, will be junk memory
    public float Y = 0;
    public float Z = 0;

    public Point(float x, float y, float z) : X(x), Y(y), Z(z) {}
    public Point() = default; // default constructor
    public copy Point() = default; // Copy constructor
    public move Point() = default; // Move constructor

    public Point operator+(const Point& other) // operator overloading
    {
        return Point(this.x + other.x, this.y + other.y, this.z + other.z);
    }

    public GetMagnitude() { return Core.Math.Mag(X, Y, Z); } // member function aka method
}
```

### Members:
A class has a array of members that each begin with a `public`, `protected`, or `private` keyword. `public` means that anything can access the member, `protected` means that only its own type, and inherited classes can access the member, finally, `private` means that only its own type can access the member. If the visibility is not listed, it will be implictily declared as private

```cpp
using module Core;

class Object
{
    public String Name = "Bob";
    protected int Index;
    u32 Id; // is private
}
```

### Properties:
A property, is a variable that is tied to a spisific instance of an object. For example, the name of a dog, the size of a house, the position of a car, etc. They are declared the same way as normal variables, but with a visibility attribute.

```cpp
using module Core;

class Dog
{
    public String Name = "Spot";
}

Dog d = Dog();
Console.PrintLine(d.Name) // will print out spot
```

### Member Functions, aka Methods
A method is a function that is a member of a class. It is declared the same as a normal function, exept with a visibility attribute.

```cpp
using module Core;
class Person
{
    private String m_Name = "Person"
    public const String& GetName() { return &m_Name; }
}

Person p = Person(); // creat the person
String name = copy p.GetName(); // call the method
```

### Constructors
Constructors are you you construct objects, and assign data to them. If no constructor is stated, a default constructor will be implicitly created. You can delete the defult constructor implicitly though. 
constructors are declared as `visibility ClassName(/*arguments*/)`.
Constructors are the only way you can modify a const veriable, to witch, you can only do with the member veriables of the `this` object. You cannot modify compile time veriables however.

```cpp
using module Core;

class Car
{
    public float Speed;
    public u8 EngineSize;

    public Car(float speed, u8 engineSize)
    {
        Speed = speed;
        EngineSize = engineSize;
    }
}

Car car = Car(30.0f, 4u);
```

Constructor initializer lists allow you to clean up your code a bit more, and allow for some forms of optomization

```cpp
using module Core;

class Car
{
    public float Speed;
    public u8 EngineSize;

    public Car(float speed, u8 engineSize) : Speed(speed), EngineSize(engineSize)
    {
    }
}

Car car = Car(30.0f, 4u);
```

### Copy and Move Constructors
For some Types, you may want to implement your own custom copy or move constructors, or you may want to disable them. Copy and move constructors are declared the same way as normal constructors, but you must place the `copy` or `move` keyword before the class name, and both take either a `const ClassName&`, or `ClassName&` as the input arguments. If not specified, copy and move constructors will implicitly be added to a class.

```cpp
class ChessBoard
{
    private u8[8][8] m_Data;
    public ChessBoard() = delete; // deleting constructor explicitly, dont need to do this, because have a different constructor
    public ChessBoard(const u8[8][8]& data) : m_Data(copy data)

    public copy ChessBoard(const ChessBoard&) = default; // defult implementation
    public move ChessBoard(const ChessBoard& other) : m_Data(move other.m_Data) {} // custom implementation of the move constructor
}

u8[8][8] data = u8[8](u8[8](0)) data;
ChessBoard board = ChessBoard(&data);
```

### Destructors
Destructors are called when an object at the end of the lifetime of an object. Destructors are declared with a `~` before the class name, and have no arguments.

```cpp
using module Core;

class Destructable()
{
    public Destructable() = default;
    public ~Destructable() { Console.PrintLine("Destroied!"); } // destructor
}

{
    Destructable d = Destructable();
} // destructor called here
```

### Inheritance:
A class can inherit from one or more other classes, getting all of there variables. This is the same as c++ inheritance, exept that all base classes are public.

```cpp
class Base
{
    public int Value = 5;
}

class Derived : Base
{

}

Derived d = Derived();
d.Value == 5; // true
```

You can call the constructor of base classes by calling putting the type name of the object, in the constructor initializer list. If you do this, the calls to the base constructors must be BEFORE initializations to any other member variables. Also, if a base class does not have a default constructor, you must call one of that classes constructor in your constructors.

```cpp
class Animal
{
    public float Health;
    public Animal(float health) : Health(health) {}
}

class Cat : Animal
{
    public String Name;
    public Cat(const String& name, float health) : Animal(health), Name(name) {}
}

Cat c = Cat("Shenobi", 0.0f);
```

### Member Object Pointers:
*Not implemented yet*

### Member Function Pointers:
*Not implemented yet*

### Polymorphism:
*Not implemented yet*

---
## Modules:
*Not implemented yet*

---
## Templates:
*Not implemented yet*

---
## Compile Time Evaluation:
*Not implemented yet*

---
## Preprocessor:
*Not implemented yet*