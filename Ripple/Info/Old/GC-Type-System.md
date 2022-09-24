# Ripple Type System Version 4

### First Iteration:
- Mannaged pointers work very simmilar to how GO references/pointers function. The compiler will attempt to evaluate the lifetiem of the pointer to the best of its ability, and if it can, it will manuely drop it at the end of its lifetime. If it cannot find teh lifetime of the object, it will allocated using GC, witch will be automatically cleaned up when all references are destroyed.
    - This same step will happen when you get a mannaged poionter to a object on the stack.
    - There is also a mannaged array type, that works in the same way
- Raw pointers work very simmilarly to mannaged pointers, however, they do not have the GC step. These allow you to manuely manipulate memory, and call external functions, without using GC.
    - There is no garentee, that when you get a raw pointer to an GC object, that the object will be on the heap, or when it will be destroyed.
    - Raw pointers are basically C style pointers, but without implicit conversion.
    - They must be used in a `unsafe` block
- Value types are allocated as a `struct` would be in c, or c++. Each value is stored in a contiguas alligned section of bytes, that can be accessed.
    - Compile time size arrays are value types
- Function pointers allow the user to call assign a function to a veriable, witch can be called.
    - are not a value type, and do not technically reference any data
- References can only be used on function parameters, and on return values from funcitons. 
    - The compiler will check to make sure, that you are returning a reference that will be valid, when the function is exited.

### NOTES:
- I want to try and work in immutable by default, if it can easily be done, and wont effect the user all that much
- This would garentee safty with memory, in non `unsafe` code, but thread races are still an issue
    - although, const by default would still solve this

### Version 4.1 Examples:
```cs
int i = 5; // value type

// Mannaged:
int^ mptr = &i; // mannaged pointer
int[4] farr = {2, 3, 4, 5}; // compile time size array
int[] arr = new int[i]; // runtime sized array

// Unmannaged:
int* rptr = @farr; // raw pointer pointing to an array
rptr[2] = 42; // set value of index 2, with pointer indexing

// References:
func SomeFunc(ref string a) -> ref string
{
    return a; // valid
}
```

### Version 4.2 Examples:
```cs
int i = int(1); // value type
int^ ptr = new int(42); // pointer type, ???

int[10] sarr = int[10](); // must be known at compile time
int[] darr = new int[i]();

```