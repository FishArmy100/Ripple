

Interesting quirk about having explicit move and copy
```cs
class Vec3
{
    public const int X;
    public const int Y;
    public const int Z;

    public Vec3(int x, int y, int z) : X(x), Y(y), Z(z) {}
}

class Entity
{
    public Vec3 Position;
    public Vec3 Rotation;
    public Vec3 Scale;

    public Vec3(Vec3 pos, Vec3 rot, Vec3 scale) : Position(pos), Rotation(rot), Scale(scale) {}f
}

func New1<T, TArgs...>(TArgs... args) -> T*
{
    T* ptr = malloc(sizeof(T)) as T*;
    ptr := (move args...);
}

func Main() -> int
{
    // does not compile, because types must be EXPLICITLY copied
    Entity* e1 = New1<Entity>(Vec3(3, 4, 5), Vec3(0, 0, 0), Vec3(1, 1, 1));
}
```