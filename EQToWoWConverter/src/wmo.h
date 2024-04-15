#ifndef _WMO_H_INCLUDED_
#define _WMO_H_INCLUDED_

#include "Types.h"
#include <string>
#include <vector>
#include "Vec.h"

using namespace std;

// Note-- Looks like all sections must have at least a header

///////////////////////////////////////////////////////////////////////////////
// WMO Root File

// MOHD - 64 bytes
struct WMOHeader
{
    uint32 NumTextures;
    uint32 NumGroups;
    uint32 NumPortals;
    uint32 NumLights;
    uint32 NumModels;
    uint32 NumDoodads;
    uint32 NumDoodadSets;
    uint32 AmbiantColor;            // ?? Maybe
    uint32 WMOID;                   // ID in WMOAreaTable.dbc?
    Vec3D BoundBox1;                // Corner 1
    Vec3D BoundBox2;                // Corner 2
    uint32 Unknown;                 // Always 0? What is this for?
};

// MOMT - 64 bytes
// Taken from wmo.h in wowmapview-- needs adjust
// Flags:   0x01    ?
//          0x04    Two-Sided (Disable backface culling)
//          0x10    Bright at Night (disable shading)
//          0x20    ?
//          0x23    Darkened (?)
//          0x40    ?
//          0x80    ?
// TODO: This way not understood, find out
struct WMOMaterial
{
	int32 UKflags;
	int32 UKd1;
	int32 UKtransparent;
	int32 UKnameStart;      // Where texture name starts in the buffer?
	uint32 UKcol1;
	int32 UKd3;
	int32 UKnameEnd;        // Where texture name ends in the buffer?
	uint32 UKcol2;
	int32 UKd4;
	float UKf1;
	float UKf2;
	int32 UKdx[5];
};

// MOGI - 32 bytes
// Flags:   0x8         Outdoor (use global lights?)
//          0x40        ?
//          0x80        ?
//          0x2000      Indoor (use local lights?) -- Disable this for now for MOCV
//          0x8000      Unknown but frequent
//          0x10000     Used in Stormwind?
//          0x40000     Show skybox if player is 'inside' the group
struct WMOGroupInformation
{
    uint32 Flags;
    Vec3D BoundBox1;
    Vec3D BoundBox2;
    int32 NameOffset;
};

// MOLT - 48 bytes
enum WMOLightType
{
    OMNI_LIGHT,
    SPOT_LIGHT,
    DIRECT_LIGHT,
    AMBIENT_LIGHT
};
class WMOLight
{
public:
    static const uint32 Size = 48;

    WMOLight() {}
    ~WMOLight() {}

    int8 LightType;
    int8 Type;          // ?
    int8 UseAtten;      // ?
    int8 Pad;           // ?
    int8 Color[4];      // B, G, R, A
    Vec3D Position;
    float Intensity;
    float AttenStart;
    float AttenEnd;
    float Unknown[4];

    // TODO: Remove this if it's not implemented in this manner
    void ConvertToCharArray(char* outputBuffer)
    {
        memcpy(&outputBuffer[0], &LightType, 1);
        memcpy(&outputBuffer[1], &Type, 1);
        memcpy(&outputBuffer[2], &UseAtten, 1);
        memcpy(&outputBuffer[3], &Pad, 1);
        memcpy(&outputBuffer[4], Color, 4);
        memcpy(&outputBuffer[8], &Position.X, 4);
        memcpy(&outputBuffer[12], &Position.Y, 4);
        memcpy(&outputBuffer[16], &Position.Z, 4);
        memcpy(&outputBuffer[20], &Intensity, 4);
        memcpy(&outputBuffer[24], &AttenStart, 4);
        memcpy(&outputBuffer[28], &AttenEnd, 4);
        memcpy(&outputBuffer[32], &Unknown, 16);
    }
/*    void FillFromCharArray(const char* inputBuffer)
    {


    } */
};

// MODS - 32 bytes
class WMODoodadSet
{
public:
    static const uint32 Size = 32;

    WMODoodadSet() {}
    ~WMODoodadSet() {}

    char Name[20];
    uint32 FirstDoodadIndex;
    uint32 NumDoodads;
    uint32 Unused;      // always 0
};

// MODD - 40 bytes
class WMODoodadInformation
{
public:
    static const uint32 Size = 40;

    WMODoodadInformation() {}
    ~WMODoodadInformation() {}

    uint32 NameIndex;       // Offset of model name is MODN
    Vec3D Position;
    float RotationW;        // Uses quaternions
    float RotationX;
    float RotationY;
    float RotationZ;
    float Scale;
    uint8 Color[4];         // ?? B, G, R, A
};

// MOPV - 48 bytes
class WMOPortal
{
public:
    static const uint32 Size = 48;

    WMOPortal() {}
    ~WMOPortal() {}

    Vec3D P1;
    Vec3D P2;
    Vec3D P3;
    Vec3D P4;
};

// MOVB - 4 bytes
class WMOVisibleBlockList
{
public:
    static const uint32 Size = 8;

    WMOVisibleBlockList() {}
    ~WMOVisibleBlockList() {}

    uint16 FirstVertex;
    uint16 Count;
};

// MOPR - 8 bytes
class WMOPortalRelationship
{
public:
    static const uint32 Size = 8;

    WMOPortalRelationship() {}
    ~WMOPortalRelationship() {}

    uint16 PortalIndex;
    uint16 GroupIndex;
    int16 Side;         // 1 or -1
    uint16 Filler;      // ?
};

// MOPT - 20 bytes
class WMOPortalInformation
{
public:
    static const uint32 Size = 20;

    WMOPortalInformation() {}
    ~WMOPortalInformation() {}

    uint16 StartVertex;
    uint16 Count;
    float Point1;
    float Point2;
    float Point3;
    float Point4;
};

// MFOG - 48 bytes
class WMOFog
{
public:
    static const uint32 Size = 48;

    WMOFog() {}
    ~WMOFog() {}

    uint32 Flags;
    Vec3D Position;
    float SmallerRadius;
    float LargerRadius;
    float End;
    float StartMultiplier;   // 0..1
    uint32 Color;
    float Unknown1;          // Always 222.222
    float Unknown2;          // Always -1 or -0.5
    uint32 Color2;
};

class WMORootFile
{
public:
    WMORootFile() {}
    ~WMORootFile() {}

public:
    WMOHeader mHeader;
    vector<WMOMaterial> mMaterials;
    vector<string> mTextureFileNames;
    vector<string> mGroupNames;
    vector<WMOGroupInformation> mGroupInformations;
    vector<WMOLight> mLights;
    vector<string> mModelFileNames;
    char* mpModelListBuffer;
    int32 mModelListBufferLength;
    string mSkyboxFileName;
    vector<WMOPortal> mPortals;
    vector<WMOPortalInformation> mPortalInformations;
    vector<WMOPortalRelationship> mPortalRelationships;
    vector<Vec3D> mVisibleBlockVerticies;   // Can be empty
    vector<WMOVisibleBlockList> mVisibleBlockLists;  // Can be empty
    vector<WMODoodadSet> mDoodadSets;
    vector<WMODoodadInformation> mDoodadInformations;
    vector<WMOFog> mFogs;
//    vector<float> mConvexVolumnPlanes;
};

///////////////////////////////////////////////////////////////////////////////
// WMO Group File

// MOGP - 68 byte
struct WMOGroupHeader
{
    int32 GroupNameStart;               // Offset into MOGN chunk
    int32 DescriptiveGroupNameStart;    // Offset into MOGN chunk
    int32 Flags;
    Vec3D BoundingBox1;                 // Same as MOGI
    Vec3D BoundingBox2;
    int16 PortalStart;                  // Index into MOPR
    int16 PortalCount;                  // Count of MOPR
    int16 NumBatchesA;
    int16 NumBatchesB;
    int32 NumBatchesC;
    uint8 Fogs[4];
    int32 Unknown1;                     // Always 15
    int32 GroupID;                      // Column 4 in WMOAreaTable.dbc
    int32 Unknown2;                     // Always 0?
    int32 Unknown3;                     // Always 0?
};

// MOPY - 2 bytes
enum WMOTriangleMaterialInfoFlags
{
    F_NOCAMCOLLIDE,
    F_DETAIL,
    F_COLLISION,
    F_HINT,
    F_RENDER,
    F_COLLIDE_HIT
};
struct WMOTriangleMaterialInfo
{
    uint8 Flags;
    uint8 MaterialID;           // Links to MOMT, sometimes 0xFF
};

// MOVI - 6 bytes
struct WMOVertexIndicies
{
    int16 Index1;
    int16 Index2;
    int16 Index3;
};

// MOBA - 24 bytes
struct WMOBatch
{
    int8 Bytes[12];     // ?
    uint32 IndexStart;
    uint16 IndexCount;
    uint16 VertexStart;
    uint16 VertexEnd;
    int8 Flags;
    int8 Texture;
};

// MLIQ - 30 bytes
struct WMOLiquid
{
    uint32 NumXVerticies;   // xverts
    uint32 NumYVerticies;   // yverts
    uint32 NumXTiles;       // xtiles = xverts-1
    uint32 NumYTiles;       // ytiles = yverts-1
    Vec3D Position;         // Base coordinates?
    uint16 Material;        // Type?
};

class WMOGroupFile
{
public:
    WMOGroupFile() {}
    ~WMOGroupFile() {}

public:
    WMOGroupHeader mHeader;
    vector<WMOTriangleMaterialInfo> mTriangleMaterialInfos;
    vector<WMOVertexIndicies> mVertexIndicies;  // MOVI
    vector<Vec3D> mVerticies;                   // MOVT
    vector<Vec3D> mNormals;                     // MONR
    vector<Vec2D> mTextureCoordinates;          // MOTV
    vector<WMOBatch> mBatches;                  // MOBA
    vector<int16> mLightReferences;             // MOLR
    vector<int16> mDoodadReferences;            // MODR
    // Need MOCV in here
};

#endif // _WMO_H_INCLUDED_
