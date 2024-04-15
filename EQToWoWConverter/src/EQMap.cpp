#include "wld.h"
#include "s3d.h"
#include "EQMap.h"
#include "Memory.h"
#include "stdio.h"
#include <string>
#include <vector>

using namespace std;

///////////////////////////////////////////////////////////////////////////////
// Constructor / Destructor
EQMap::EQMap() :
    mName(""),
    mBaseS3DFullPath(""),
    mZoneMesh(NULL),
    mLoaded(false)
{


}

EQMap::~EQMap()
{
    if (mLoaded)
        Unload();
}

///////////////////////////////////////////////////////////////////////////////

bool EQMap::Load(string inputMapFolder, string zoneName)
{
    if (mLoaded)
        Unload();

    mName = zoneName;

    // Load base map object
    mBaseS3DFullPath = inputMapFolder + zoneName + ".s3d";
    LoadAndFillBaseMapGeometry();

    // Load map objects (doodads)

    mLoaded = true;

    return mLoaded;
}

///////////////////////////////////////////////////////////////////////////////

void EQMap::LoadAndFillBaseMapGeometry()
{
    // This needs a lot of cleanup
    FILE* fp = fopen(mBaseS3DFullPath.c_str(), "r");
    uchar *buf;

    s3d_object s3DObject;
    wld_object wldObject;

    // TODO: Needs to be more generic
    S3D_Init(&s3DObject, fp);
    S3D_GetFile(&s3DObject, "halas.wld", &buf);

    WLD_Init(&wldObject, buf, &s3DObject, 1);

    ZoneMesh meshi;
    ZoneMesh *mesh = &meshi;
    WLD_GetZoneMesh(&wldObject, &meshi);

    mZoneMesh = new EQZoneMesh;
    mZoneMesh->VertexCount = meshi.vertexCount;
    for (int i = 0; i < meshi.vertexCount; ++i)
    {
        EQVertex newEQVertex;
        newEQVertex.pTexture = NULL;
        newEQVertex.X = meshi.verti[i]->x;
        newEQVertex.Y = meshi.verti[i]->y;
        newEQVertex.Z = meshi.verti[i]->z;
        newEQVertex.U = meshi.verti[i]->u;
        newEQVertex.V = meshi.verti[i]->v;

        mZoneMesh->Verticies.push_back(newEQVertex);
    }
    for (int j = 0; j < meshi.polygonCount; ++j)
    {
        EQFace newEQFace;
        newEQFace.V1 = meshi.poly[j]->v1;
        newEQFace.V2 = meshi.poly[j]->v2;
        newEQFace.V3 = meshi.poly[j]->v3;

        mZoneMesh->Faces.push_back(newEQFace);
    }
}

///////////////////////////////////////////////////////////////////////////////

void EQMap::Unload()
{
    SafeDelete(mZoneMesh);
    mLoaded = false;
    mName = "";
}
