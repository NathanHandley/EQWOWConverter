#ifndef _VEC3D_INCLUDED_
#define _VEC3D_INCLUDED_

class Vec2D
{
public:
    Vec2D() {}
    ~Vec2D() {}

public:
    float X;
    float Y;

public:
    void Set(float x, float y) { X = x; Y = y;}
    void Set(Vec2D vec) { X = vec.X; Y = vec.Y; }
};

class Vec3D
{
public:
    Vec3D() {}
    ~Vec3D() {}

public:
    float X;
    float Y;
    float Z;

public:
    void Set(float x, float y, float z) { X = x; Y = y; Z = z; }
    void Set(Vec3D vec) { X = vec.X; Y = vec.Y; Z = vec.Z; }
};

struct Vec4DColor
{
    float A;
    float R;
    float G;
    float B;
};


#endif // VEC3D_INCLUDED
