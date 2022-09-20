I was considering two different versions of references, one inspired from c++, the other from rust for Ripple, and I couldn't really decide on witch one.
Similarities in the design:
- Are like pointers
- Can only 'point' to one object in memory
- Cannot be null
- both can reference r-value temporaries

Rust like references: More complex and verbose, but have less edge cases
```cpp
int value = 0;
int& ref1 = &value; // must explicitly take a reference
int& ref2 = &9; // referencing an r value
ref2 = ref1;

*ref2 = 6;

print(value); // prints out 6

int&[3] arr = { 5, 6, 7 }; // can have an array of references

 // array indexing returns a reference of whatever is contained
 // references of references are valid
int&& val = arr[2];
print(**val) // prints out 7

String&&& s = &&&String("Hello World!");
s.Length() // implicit dereferencing
s->->->Length(); // also works
```

C++ like references: More limited, but simpler to use
```cpp
int value1 = 5;
int value2 = 6;

int& ref1 = value1; // initalization of the reference sets what it references
int& ref2 = value2; // implicit referencing
int& rref = 5; // referencing an r-value

ref2 = ref1;
ref2 = 9;

print(value1); // prints out 5
print(value2); // prints out 9 

// int&[1] arr; DOES NOT COMPILE
// int&& referf; DOES NOT COMPILE

int[2] arr = {1, 32};

int& ArrRef = arr[0]; // indexing returns a reference
```

C# like references: tiny bit more verbose, but fix all the other problems
```cs
// Pass by reference
operator+(ref const Vec2 a, ref const Vec2 b) -> Vec2 {...}

// return by reference 
func Get() -> ref int {...}

// bind by reference 
ref int value = obj.Get(); 
```
These would be the only legal uses of references, you cannot have a reference as a member, or as a template argument