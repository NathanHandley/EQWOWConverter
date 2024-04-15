#ifndef EQMAP_H_INCLUDED
#define EQMAP_H_INCLUDED

#include <string>
#include <vector>

#include "Types.h"

using namespace std;

struct EQTexture
{
    string Name;
};

struct EQVertex
{
    double X, Y, Z;
    float U, V;
    EQTexture* pTexture;
};

struct EQFace
{
    int32 V1, V2, V3;
};

struct EQZoneMesh
{
    int32 VertexCount;
    vector<EQVertex> Verticies;
    vector<EQTexture> Textures;
    vector<EQFace> Faces;
};

// Main class object that holds the full formatted data for an EQ Map
class EQMap
{
public:
    EQMap();
    ~EQMap();

private:
    string mName;
    string mBaseS3DFullPath;
    EQZoneMesh* mZoneMesh;
    bool mLoaded;

private:
    void LoadAndFillBaseMapGeometry();

public:
    // Loads in all the data for an Everquest Map
    bool Load(string inputMapFolder, string zoneName);
    void Unload();

public:
    const string GetName() { return mName; }
    EQZoneMesh* GetZoneMesh() { return mZoneMesh; }

    const bool IsLoaded() { return mLoaded; }
};

#endif // EQMAP_H_INCLUDED
