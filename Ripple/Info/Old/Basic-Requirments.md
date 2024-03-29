# Basic Ripple Requirments
## Core Requirements:
- Primatives
    - Int: `i8`, `i16`, `i32`, `i64` types, and `int`
    - Unsigned int: `u8`, `u16`, `u32`, `u64` types
    - Float: `f32`, `f64` bit types, and `float`
    - Charactor: `char` bit type
    - Bool: `bool` type, `true`/`false` values
    - All have C-Style operations
    - Logical:
    - Casting: `expr "as" typename`
        - Float -> Int, <->
        - Int -> Uint, <->
        - Float -> Uint, <->
        - Ptr -> Ref, <->
        - Int -> Int, <->
        - Float -> Float, <->
        - Uint -> Uint, <->
- References: `typename "mut"? "&"`
    - Reference of operators:
        - Mutable: `"&" "mut" expr`
        - Immutable: `"&" expr`
    - Lifetimes: `"'" identifier`
        - Anotate references: `"&" "'" identifier`
        - Lifetime inference for functions
    - Rust reference rules:
        - Only one live mutable reference
        - Cannot mutate an object with a live immutable reference
        - Lifetime of referenced object must outlive the reference
    - Unsafe References:
    - Dereferencing: `"*" expr`
- Pointers: `typename "mut"? "*"`
    - Unsafe: cannot be used outside of unsafe code
    - Indexing: `expr "[" expr "]"`
    - Dereferencing: `"*" expr`
- Arrays: `typename "[" size "]"`
    - Stack Allocated 
    - Statically sized
    - Index: `expr "[" expr "]"`
- Veriables: `typename id_list "=" expr`
    - Assignment
    - Allocation: RAII???
- Mutablility:
    - Anything marked as `mut` can be changed
- Scope: `"{" statment* "}"`
- Generics:
    - Lifetime
    - Compile time expression
    - Type name
- Control Flow:
    - If: `"if" "(" expr ")" statement ("else" statement)?`
    - For: same as c++
    - While: same as c++
    - Break: `break`
    - Continue: `continue`
- Functions: `"func" identifier generic_params? "(" param_list ")" "->" typename body`
    - Call: `expr "(" expr* ")"`
    - Function pointers: `"(" type_list ")" "->" typename`
    - Return statement: `"return" expr`
    - Generic inference
    - Where clause
- Classes: `"class" identifier generic_params? "{" members "}"`
    - Member visibility: `public`, `private`
    - Constructor: `identifier "(" param_list ")"`
    - Destructor `"~" identifier "(" ")"`
    - Move by default for classes
    - Where clause
- External Code: 
    - Functions: `"extern" string_literal "func" identifier generic_params? "(" param_list ")" "->" typename`
    - Classes `"extern" "class" identifier "{" members "}"`
    - Linking:
        - For c code, will attempt to statically link
        - Only c valid for now


