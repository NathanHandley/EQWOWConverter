#include "Converter.h"
#include "WoWMap.h"
#include "wmo.h"
#include "EQMap.h"

void Converter::ConvertEQMapToWoWMap(EQMap* eqMap, const string name, WoWMap* returnMap)
{
    // Load in the Everquest zone mesh
    if ((eqMap == NULL) || (eqMap->IsLoaded() == false))
    {
        printf("EQMap was NULL or not loaded.\n");
        return;
    }
    EQZoneMesh* zoneMesh = eqMap->GetZoneMesh();

    // Create the map
    returnMap->Create(name);

    // Now the fun stuff!
    // All EQWoW maps have 1 root file and 1 group file, because they are so small (and I'm lazy?)

    // Fill the root header
    returnMap->mRootFile.mHeader.AmbiantColor = 0;
    returnMap->mRootFile.mHeader.BoundBox1.Set(0, 0, 0);
    returnMap->mRootFile.mHeader.BoundBox2.Set(1.0f, 1.0f, 1.0f);
    returnMap->mRootFile.mHeader.NumDoodads = 0;
    returnMap->mRootFile.mHeader.NumDoodadSets = 0;
    returnMap->mRootFile.mHeader.NumGroups = 1;
    returnMap->mRootFile.mHeader.NumLights = 0;
    returnMap->mRootFile.mHeader.NumModels = 0;
    returnMap->mRootFile.mHeader.NumPortals = 0;
    returnMap->mRootFile.mHeader.NumTextures = 0;
    returnMap->mRootFile.mHeader.Unknown = 0;           // Always
    returnMap->mRootFile.mHeader.WMOID = 0;             // This will need something, perhaps read current?

    // Fill in the group header
    WMOGroupFile* newGroupFile = new WMOGroupFile();
    newGroupFile->mHeader.GroupNameStart = -1;           // This will need something
    newGroupFile->mHeader.DescriptiveGroupNameStart = -1;
    newGroupFile->mHeader.Flags = 0;
    newGroupFile->mHeader.BoundingBox1.Set(returnMap->mRootFile.mHeader.BoundBox1);
    newGroupFile->mHeader.BoundingBox2.Set(returnMap->mRootFile.mHeader.BoundBox2);
    newGroupFile->mHeader.PortalStart = 0;
    newGroupFile->mHeader.PortalCount = 0;
    newGroupFile->mHeader.NumBatchesA = 0;               // Might have to change this to 1 and create a render batch
    newGroupFile->mHeader.NumBatchesB = 0;
    newGroupFile->mHeader.NumBatchesC = 0;
    newGroupFile->mHeader.Fogs[0] = 0;
    newGroupFile->mHeader.Fogs[1] = 0;
    newGroupFile->mHeader.Fogs[2] = 0;
    newGroupFile->mHeader.Fogs[3] = 0;
    newGroupFile->mHeader.Fogs[4] = 0;
    newGroupFile->mHeader.Unknown1 = 15;                 // Always 15
    newGroupFile->mHeader.GroupID = 0;                   // Will probably need this
    newGroupFile->mHeader.Unknown2 = 0;
    newGroupFile->mHeader.Unknown3 = 0;

    // Fill in group information --  Figure this out
    WMOGroupInformation newGroupInformation;
    newGroupInformation.Flags = 0;
    newGroupInformation.BoundBox1.Set(returnMap->mRootFile.mHeader.BoundBox1);
    newGroupInformation.BoundBox2.Set(returnMap->mRootFile.mHeader.BoundBox2);
    newGroupInformation.NameOffset = -1;
    returnMap->mRootFile.mGroupInformations.push_back(newGroupInformation);

    // Add a material - TEMP
    WMOMaterial newMaterial;
    newMaterial.UKflags = 0;
    newMaterial.UKd1 = 0;
    newMaterial.UKtransparent = 0;
    newMaterial.UKnameStart = 0;
    newMaterial.UKnameEnd = 0;
    newMaterial.UKcol1 = 0;
    newMaterial.UKd3 = 0;
    newMaterial.UKnameEnd = 0;
    newMaterial.UKcol2 = 0;
    newMaterial.UKd4 = 0;
    newMaterial.UKf1 = 0.0f;
    newMaterial.UKf2 = 0.0f;
    newMaterial.UKdx[0] = 0;
    newMaterial.UKdx[1] = 0;
    newMaterial.UKdx[2] = 0;
    newMaterial.UKdx[3] = 0;
    newMaterial.UKdx[4] = 0;
    returnMap->mRootFile.mMaterials.push_back(newMaterial);

    // MOPY - MaterialInfo
    WMOTriangleMaterialInfo newMaterialInfo;
    newMaterialInfo.Flags = F_COLLISION;
    newMaterialInfo.MaterialID = 0;
    newGroupFile->mTriangleMaterialInfos.push_back(newMaterialInfo);

    // Go through all the triangles
    for (uint32 ti = 0; ti < zoneMesh->Faces.size(); ++ti)
    {
        // MOVI - Vertex indicies
        WMOVertexIndicies newVertexIndicies;
        newVertexIndicies.Index1 = zoneMesh->Faces[ti].V1;
        newVertexIndicies.Index2 = zoneMesh->Faces[ti].V2;
        newVertexIndicies.Index3 = zoneMesh->Faces[ti].V3;
        newGroupFile->mVertexIndicies.push_back(newVertexIndicies);
    }

    // No BSP Tree... hmmmm

    // Go through all the verticies
    for (uint32 i = 0; i < zoneMesh->Verticies.size(); ++i)
    {
        // MOVT - Verticies (going to try X,Y,Z just in case it's the same as EQ-- suggested is X,Z,-Y)
        Vec3D newVertex;
        newVertex.Set(zoneMesh->Verticies[i].X, zoneMesh->Verticies[i].Y, zoneMesh->Verticies[i].Z);
        newGroupFile->mVerticies.push_back(newVertex);

        // MONR - Normals (going 0 for now)
        Vec3D newNormal;
        newNormal.Set(0, 0, 0);
        newGroupFile->mNormals.push_back(newNormal);

        // MOTV - Texture Coords
        Vec2D newTexCords;
        newTexCords.Set(zoneMesh->Verticies[i].U, zoneMesh->Verticies[i].V);
    }

    returnMap->mGroupFiles.push_back(newGroupFile);

    return;
}
