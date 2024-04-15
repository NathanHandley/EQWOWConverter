#include "sstream"
#include "stdio.h"
#include "WoWMap.h"
#include "wmo.h"
#include "Memory.h"

///////////////////////////////////////////////////////////////////////////////
// Constructor / Destructor

// For test work, going to with with KL_OrgrimmarLavaDungeon\LavaDungeon.wmo - _011.wmo
// TODO: Clean up how buffers are filled


WoWMap::WoWMap() :
    mName(""), mVersion(1)
{



}

WoWMap::~WoWMap()
{
    for (uint i = mGroupFiles.size(); i > 0; i--)
        SafeDelete(mGroupFiles.at(i-1));
}

void WoWMap::WriteChunkHeader(string identifier, uint32 size, FILE* pFile)
{
    char reversedIdentifier[4];
    memcpy(reversedIdentifier, identifier.c_str(), 4);
    FlipIdentifier(reversedIdentifier);
    fwrite(reversedIdentifier, 4, 1, pFile);
    fwrite(&size, 4, 1, pFile);
}

void WoWMap::ReadChunkHeader(char* pIdentifier, uint32* pSize, FILE* pFile)
{
    char reversedIdentifier[4];
    fread(reversedIdentifier, 4, 1, pFile);

    FlipIdentifier(reversedIdentifier);
    memcpy(pIdentifier, reversedIdentifier, 4);
    pIdentifier[4] = 0;

    fread(pSize, 4, 1, pFile);
}

void WoWMap::FlipIdentifier(char* pIdentifier)
{
    char swap = pIdentifier[0];
    pIdentifier[0] = pIdentifier[3];
    pIdentifier[3] = swap;

    swap = pIdentifier[2];
    pIdentifier[2] = pIdentifier[1];
    pIdentifier[1] = swap;
}

string WoWMap::ConcatStringVector(vector<string> stringList)
{
    string returnString = "";

    for (uint32 i = 0; i < stringList.size(); ++i)
        returnString = returnString + stringList[i] + '\0';

    return returnString;
}


///////////////////////////////////////////////////////////////////////////////

void WoWMap::Create(string name)
{
    mName = name;
}

///////////////////////////////////////////////////////////////////////////////

void WoWMap::WriteToDisk(string folder)
{
    string rootFileName = folder + mName + ".wmo";
    WriteRootFile(rootFileName);

    for (uint32 i = 0; i < mGroupFiles.size(); ++i)
    {
        string stringID;
        ostringstream stringIndex;
        stringIndex << i;
        if (i < 10)
            stringID = "_00" + stringIndex.str();
        else if (i < 100)
            stringID = "_0" + stringIndex.str();
        else
            stringID = "_" + stringIndex.str();

        string groupFileName = folder + mName + stringID + ".wmo";
        WriteGroupFile(groupFileName, i);
    }
}

///////////////////////////////////////////////////////////////////////////////
void WoWMap::WriteRootFile(string fileName)
{
    FILE *pFile;
    pFile = fopen(fileName.c_str(), "w");

    // MVER - Version
    WriteChunkHeader("MVER", 4, pFile);
    fwrite(&mVersion, 4, 1, pFile);

    // MOHD - Header
    WriteChunkHeader("MOHD", 64, pFile);
    fwrite(&mRootFile.mHeader.NumTextures, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.NumGroups, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.NumPortals, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.NumLights, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.NumModels, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.NumDoodads, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.NumDoodadSets, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.AmbiantColor, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.WMOID, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.BoundBox1.X, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.BoundBox1.Y, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.BoundBox1.Z, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.BoundBox2.X, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.BoundBox2.Y, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.BoundBox2.Z, 4, 1, pFile);
    fwrite(&mRootFile.mHeader.Unknown, 4, 1, pFile);

    // MOTX - Texture Names
    string textureNamesString = ConcatStringVector(mRootFile.mTextureFileNames);
    WriteChunkHeader("MOTX", textureNamesString.size(), pFile);
    fwrite(textureNamesString.c_str(), textureNamesString.size(), 1, pFile);

    // MOMT - Materials
    WriteChunkHeader("MOMT", 64 * mRootFile.mMaterials.size(), pFile);
    for (uint32 i = 0; i < mRootFile.mMaterials.size(); ++i)
    {
        fwrite(&mRootFile.mMaterials[i].UKflags, 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKd1, 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKtransparent, 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKnameStart, 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKcol1, 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKd3, 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKnameEnd, 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKcol2, 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKd4, 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKf1, 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKf2, 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKdx[0], 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKdx[1], 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKdx[2], 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKdx[3], 4, 1, pFile);
        fwrite(&mRootFile.mMaterials[i].UKdx[4], 4, 1, pFile);
    }

    // MOGN - Group Names
    string groupNamesString = ConcatStringVector(mRootFile.mGroupNames);
    WriteChunkHeader("MOGN", groupNamesString.size(), pFile);
    fwrite(groupNamesString.c_str(), groupNamesString.size(), 1, pFile);

    // MOGI - Group Informations
    WriteChunkHeader("MOGI", 32 * mRootFile.mGroupInformations.size(), pFile);
    for (uint32 i = 0; i < mRootFile.mGroupInformations.size(); ++i)
    {
        fwrite(&mRootFile.mGroupInformations[i].Flags, 4, 1, pFile);
        fwrite(&mRootFile.mGroupInformations[i].BoundBox1.X, 4, 1, pFile);
        fwrite(&mRootFile.mGroupInformations[i].BoundBox1.Y, 4, 1, pFile);
        fwrite(&mRootFile.mGroupInformations[i].BoundBox1.Z, 4, 1, pFile);
        fwrite(&mRootFile.mGroupInformations[i].BoundBox2.X, 4, 1, pFile);
        fwrite(&mRootFile.mGroupInformations[i].BoundBox2.Y, 4, 1, pFile);
        fwrite(&mRootFile.mGroupInformations[i].BoundBox2.Z, 4, 1, pFile);
        fwrite(&mRootFile.mGroupInformations[i].NameOffset, 4, 1, pFile);
    }

    // MOSB - Skybox, I think this is missing
    WriteChunkHeader("MOSB", 4, pFile);
    char skyboxDumpBuffer[4] = {0, 0, 0, 0};
    fwrite(&skyboxDumpBuffer, 4, 1, pFile);

    // MOPV - Portals
    WriteChunkHeader("MOPV", WMOPortal::Size * mRootFile.mPortals.size(), pFile);
    for (uint32 i = 0; i < mRootFile.mPortals.size(); ++i)
    {
        fwrite(&mRootFile.mPortals[i].P1.X, 4, 1, pFile);
        fwrite(&mRootFile.mPortals[i].P1.Y, 4, 1, pFile);
        fwrite(&mRootFile.mPortals[i].P1.Z, 4, 1, pFile);
        fwrite(&mRootFile.mPortals[i].P2.X, 4, 1, pFile);
        fwrite(&mRootFile.mPortals[i].P2.Y, 4, 1, pFile);
        fwrite(&mRootFile.mPortals[i].P2.Z, 4, 1, pFile);
        fwrite(&mRootFile.mPortals[i].P3.X, 4, 1, pFile);
        fwrite(&mRootFile.mPortals[i].P3.Y, 4, 1, pFile);
        fwrite(&mRootFile.mPortals[i].P3.Z, 4, 1, pFile);
        fwrite(&mRootFile.mPortals[i].P4.X, 4, 1, pFile);
        fwrite(&mRootFile.mPortals[i].P4.Y, 4, 1, pFile);
        fwrite(&mRootFile.mPortals[i].P4.Z, 4, 1, pFile);
    }

    // MOPT - Portal Informations
    WriteChunkHeader("MOPT", WMOPortalInformation::Size * mRootFile.mPortalInformations.size(), pFile);
    for (uint32 i = 0; i < mRootFile.mPortalInformations.size(); ++i)
    {
        fwrite(&mRootFile.mPortalInformations[i].StartVertex, 2, 1, pFile);
        fwrite(&mRootFile.mPortalInformations[i].Count, 2, 1, pFile);
        fwrite(&mRootFile.mPortalInformations[i].Point1, 4, 1, pFile);
        fwrite(&mRootFile.mPortalInformations[i].Point2, 4, 1, pFile);
        fwrite(&mRootFile.mPortalInformations[i].Point3, 4, 1, pFile);
        fwrite(&mRootFile.mPortalInformations[i].Point4, 4, 1, pFile);
    }

    // MOPR - Portal Relationships
    WriteChunkHeader("MOPR", WMOPortalRelationship::Size * mRootFile.mPortalRelationships.size(), pFile);
    for (uint32 i = 0; i < mRootFile.mPortalRelationships.size(); ++i)
    {
        fwrite(&mRootFile.mPortalRelationships[i].PortalIndex, 2, 1, pFile);
        fwrite(&mRootFile.mPortalRelationships[i].GroupIndex, 2, 1, pFile);
        fwrite(&mRootFile.mPortalRelationships[i].Side, 2, 1, pFile);
        fwrite(&mRootFile.mPortalRelationships[i].Filler, 2, 1, pFile);
    }

    // MOVV - Visable Block Verticies
    WriteChunkHeader("MOVV", 12 * mRootFile.mVisibleBlockVerticies.size(), pFile);
    for (uint32 i = 0; i < mRootFile.mVisibleBlockVerticies.size(); ++i)
    {
        fwrite(&mRootFile.mVisibleBlockVerticies[i].X, 4, 1, pFile);
        fwrite(&mRootFile.mVisibleBlockVerticies[i].Y, 4, 1, pFile);
        fwrite(&mRootFile.mVisibleBlockVerticies[i].Z, 4, 1, pFile);
    }

    // MOVB -Visable Block List
    WriteChunkHeader("MOVB", WMOVisibleBlockList::Size * mRootFile.mVisibleBlockLists.size(), pFile);
    for (uint32 i = 0; i < mRootFile.mVisibleBlockLists.size(); ++i)
    {
        fwrite(&mRootFile.mVisibleBlockLists[i].FirstVertex, 2, 1, pFile);
        fwrite(&mRootFile.mVisibleBlockLists[i].Count, 2, 1, pFile);
    }

    // MOLT - Lights
    WriteChunkHeader("MOLT", WMOLight::Size * mRootFile.mLights.size(), pFile);
    for (uint32 i = 0; i < mRootFile.mLights.size(); ++i)
    {
        fwrite(&mRootFile.mLights[i].LightType, 1, 1, pFile);
        fwrite(&mRootFile.mLights[i].Type, 1, 1, pFile);
        fwrite(&mRootFile.mLights[i].UseAtten, 1, 1, pFile);
        fwrite(&mRootFile.mLights[i].Pad, 1, 1, pFile);
        fwrite(&mRootFile.mLights[i].Color[0], 1, 1, pFile);
        fwrite(&mRootFile.mLights[i].Color[1], 1, 1, pFile);
        fwrite(&mRootFile.mLights[i].Color[3], 1, 1, pFile);
        fwrite(&mRootFile.mLights[i].Color[4], 1, 1, pFile);
        fwrite(&mRootFile.mLights[i].Position.X, 4, 1, pFile);
        fwrite(&mRootFile.mLights[i].Position.Y, 4, 1, pFile);
        fwrite(&mRootFile.mLights[i].Position.Z, 4, 1, pFile);
        fwrite(&mRootFile.mLights[i].Intensity, 4, 1, pFile);
        fwrite(&mRootFile.mLights[i].AttenStart, 4, 1, pFile);
        fwrite(&mRootFile.mLights[i].AttenEnd, 4, 1, pFile);
        fwrite(&mRootFile.mLights[i].Unknown[0], 4, 1, pFile);
        fwrite(&mRootFile.mLights[i].Unknown[1], 4, 1, pFile);
        fwrite(&mRootFile.mLights[i].Unknown[2], 4, 1, pFile);
        fwrite(&mRootFile.mLights[i].Unknown[3], 4, 1, pFile);
    }

    // MODS - Doodad Sets
    WriteChunkHeader("MODS", WMODoodadSet::Size * mRootFile.mDoodadSets.size(), pFile);
    for (uint32 i = 0; i < mRootFile.mDoodadSets.size(); ++i)
    {
        fwrite(&mRootFile.mDoodadSets[i].Name, 20, 1, pFile);
        fwrite(&mRootFile.mDoodadSets[i].FirstDoodadIndex, 4, 1, pFile);
        fwrite(&mRootFile.mDoodadSets[i].NumDoodads, 4, 1, pFile);
        fwrite(&mRootFile.mDoodadSets[i].Unused, 4, 1, pFile);
    }

    // MODN - Model Names
    string modelNamesString = ConcatStringVector(mRootFile.mModelFileNames);
    WriteChunkHeader("MODN", modelNamesString.size(), pFile);
    fwrite(modelNamesString.c_str(), modelNamesString.size(), 1, pFile);

    // MODD - Doodad Informations
    WriteChunkHeader("MODD", WMODoodadInformation::Size * mRootFile.mDoodadInformations.size(), pFile);
    for (uint32 i = 0; i < mRootFile.mDoodadInformations.size(); ++i)
    {
        fwrite(&mRootFile.mDoodadInformations[i].NameIndex, 4, 1, pFile);
        fwrite(&mRootFile.mDoodadInformations[i].Position.X, 4, 1, pFile);
        fwrite(&mRootFile.mDoodadInformations[i].Position.Y, 4, 1, pFile);
        fwrite(&mRootFile.mDoodadInformations[i].Position.Z, 4, 1, pFile);
        fwrite(&mRootFile.mDoodadInformations[i].RotationX, 4, 1, pFile); // May need to fiddle with this
        fwrite(&mRootFile.mDoodadInformations[i].RotationY, 4, 1, pFile);
        fwrite(&mRootFile.mDoodadInformations[i].RotationZ, 4, 1, pFile);
        fwrite(&mRootFile.mDoodadInformations[i].RotationW, 4, 1, pFile);
        fwrite(&mRootFile.mDoodadInformations[i].Scale, 4, 1, pFile);
        fwrite(&mRootFile.mDoodadInformations[i].Color[0], 1, 1, pFile);
        fwrite(&mRootFile.mDoodadInformations[i].Color[1], 1, 1, pFile);
        fwrite(&mRootFile.mDoodadInformations[i].Color[2], 1, 1, pFile);
        fwrite(&mRootFile.mDoodadInformations[i].Color[3], 1, 1, pFile);
    }

    // MFOG -
    WriteChunkHeader("MFOG", WMOFog::Size * mRootFile.mFogs.size(), pFile);
    for (uint32 i = 0; i < mRootFile.mFogs.size(); ++i)
    {
        fwrite(&mRootFile.mFogs[i].Flags, 4, 1, pFile);
        fwrite(&mRootFile.mFogs[i].Position.X, 4, 1, pFile);
        fwrite(&mRootFile.mFogs[i].Position.Y, 4, 1, pFile);
        fwrite(&mRootFile.mFogs[i].Position.Z, 4, 1, pFile);
        fwrite(&mRootFile.mFogs[i].SmallerRadius, 4, 1, pFile);
        fwrite(&mRootFile.mFogs[i].LargerRadius, 4, 1, pFile);
        fwrite(&mRootFile.mFogs[i].End, 4, 1, pFile);
        fwrite(&mRootFile.mFogs[i].StartMultiplier, 4, 1, pFile);
        fwrite(&mRootFile.mFogs[i].Color, 4, 1, pFile);
        fwrite(&mRootFile.mFogs[i].Unknown1, 4, 1, pFile);
        fwrite(&mRootFile.mFogs[i].Unknown2, 4, 1, pFile);
        fwrite(&mRootFile.mFogs[i].Color2, 4, 1, pFile);
    }

    // MCVP - Skipped for now, wasn't in input file

    fclose(pFile);
}

///////////////////////////////////////////////////////////////////////////////

void WoWMap::WriteGroupFile(string fileName, uint32 index)
{
    FILE *pFile;
    pFile = fopen(fileName.c_str(), "w");

    // Put the file header
    WriteChunkHeader("MVER", 4, pFile);
    fwrite(&mVersion, 4, 1, pFile);

    // MOGP - Header
    WriteChunkHeader("MOGP", 68, pFile);
    fwrite(&mGroupFiles[index]->mHeader.GroupNameStart, 4, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.DescriptiveGroupNameStart, 4, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.Flags, 4, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.BoundingBox1.X, 4, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.BoundingBox1.Y, 4, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.BoundingBox1.Z, 4, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.BoundingBox2.X, 4, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.BoundingBox2.Y, 4, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.BoundingBox2.Z, 4, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.PortalStart, 2, 1, pFile);       // Will need
    fwrite(&mGroupFiles[index]->mHeader.PortalCount, 2, 1, pFile);       // Will need
    fwrite(&mGroupFiles[index]->mHeader.NumBatchesA, 2, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.NumBatchesB, 2, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.NumBatchesC, 4, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.Fogs[0], 1, 1, pFile);           // Might need
    fwrite(&mGroupFiles[index]->mHeader.Fogs[1], 1, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.Fogs[2], 1, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.Fogs[3], 1, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.Unknown1, 4, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.GroupID, 4, 1, pFile);           // TODO: This is needed...
    fwrite(&mGroupFiles[index]->mHeader.Unknown2, 4, 1, pFile);
    fwrite(&mGroupFiles[index]->mHeader.Unknown3, 4, 1, pFile);

    // MOPY - Materials
    WriteChunkHeader("MOPY", 2*mGroupFiles[index]->mTriangleMaterialInfos.size(), pFile);
    for (uint32 i = 0; i < mGroupFiles[index]->mTriangleMaterialInfos.size(); ++i)
    {
        fwrite(&mGroupFiles[index]->mTriangleMaterialInfos[i].Flags, 1, 1, pFile);
        fwrite(&mGroupFiles[index]->mTriangleMaterialInfos[i].MaterialID, 1, 1, pFile);
    }

    // MOVI - Vertex indicies
    WriteChunkHeader("MOVI", 6*mGroupFiles[index]->mVertexIndicies.size(), pFile);
    for (uint32 i = 0; i < mGroupFiles[index]->mVertexIndicies.size(); ++i)
    {
        fwrite(&mGroupFiles[index]->mVertexIndicies[i].Index1, 2, 1, pFile);
        fwrite(&mGroupFiles[index]->mVertexIndicies[i].Index2, 2, 1, pFile);
        fwrite(&mGroupFiles[index]->mVertexIndicies[i].Index3, 2, 1, pFile);
    }

    // MOVT - Verticies
    WriteChunkHeader("MOVT", 12*mGroupFiles[index]->mVerticies.size(), pFile);
    for (uint32 i = 0; i < mGroupFiles[index]->mVerticies.size(); ++i)
    {
         fwrite(&mGroupFiles[index]->mVerticies[i].X, 4, 1, pFile);
         fwrite(&mGroupFiles[index]->mVerticies[i].Y, 4, 1, pFile);
         fwrite(&mGroupFiles[index]->mVerticies[i].Z, 4, 1, pFile);
    }

    // MONR - Normals
    WriteChunkHeader("MONR", 12*mGroupFiles[index]->mNormals.size(), pFile);
    for (uint32 i = 0; i < mGroupFiles[index]->mNormals.size(); ++i)
    {
         fwrite(&mGroupFiles[index]->mNormals[i].X, 4, 1, pFile);
         fwrite(&mGroupFiles[index]->mNormals[i].Y, 4, 1, pFile);
         fwrite(&mGroupFiles[index]->mNormals[i].Z, 4, 1, pFile);
    }

    // MOTV - Texture Coordinates
    WriteChunkHeader("MOTV", 8*mGroupFiles[index]->mTextureCoordinates.size(), pFile);
    for (uint32 i = 0; i < mGroupFiles[index]->mTextureCoordinates.size(); ++i)
    {
        fwrite(&mGroupFiles[index]->mTextureCoordinates[i].X, 4, 1, pFile);
        fwrite(&mGroupFiles[index]->mTextureCoordinates[i].Y, 4, 1, pFile);
    }

    // MOBA - None right now, skip.  May need this
    // MOLR - None right now, skip.
    // MODR - None right now, skip.
    // MOBN - None right now, skip.
    // MOBR - None right now, skip.
    // MOCV - None right now, skip.  This looks needed.
    // MLIQ - None right now, skip.

    fclose(pFile);
}

void WoWMap::ReadFromDisk(string folder)
{
    string rootFileName = folder + mName + ".wmo";
    ReadRootFile(rootFileName);
}

void WoWMap::ReadRootFile(string fileName)
{
    FILE *pFile;
    pFile = fopen(fileName.c_str(), "r");

    char identityBuffer[5];
    uint32 size;

    while (true)
    {
        ReadChunkHeader(identityBuffer, &size, pFile);
        if (feof(pFile))
            break;

        if (!strcmp(identityBuffer, "MVER"))
        {
            fread(&mVersion, 4, 1, pFile); // LavaDungeon is version 17 of time of testing
        }
        else if (!strcmp(identityBuffer, "MOHD"))
        {
            fread(&mRootFile.mHeader.NumTextures, 4, 1, pFile);
            fread(&mRootFile.mHeader.NumGroups, 4, 1, pFile);
            fread(&mRootFile.mHeader.NumPortals, 4, 1, pFile);
            fread(&mRootFile.mHeader.NumLights, 4, 1, pFile);
            fread(&mRootFile.mHeader.NumModels, 4, 1, pFile);
            fread(&mRootFile.mHeader.NumDoodads, 4, 1, pFile);
            fread(&mRootFile.mHeader.NumDoodadSets, 4, 1, pFile);
            fread(&mRootFile.mHeader.AmbiantColor, 4, 1, pFile);
            fread(&mRootFile.mHeader.WMOID, 4, 1, pFile);
            fread(&mRootFile.mHeader.BoundBox1.X, 4, 1, pFile);
            fread(&mRootFile.mHeader.BoundBox1.Y, 4, 1, pFile);
            fread(&mRootFile.mHeader.BoundBox1.Z, 4, 1, pFile);
            fread(&mRootFile.mHeader.BoundBox2.X, 4, 1, pFile);
            fread(&mRootFile.mHeader.BoundBox2.Y, 4, 1, pFile);
            fread(&mRootFile.mHeader.BoundBox2.Z, 4, 1, pFile);
            fread(&mRootFile.mHeader.Unknown, 4, 1, pFile);
        }
        else if (!strcmp(identityBuffer, "MOTX"))
        {
            char currentCharacter;
            string currentString;
            for (uint32 i = 0; i < size; ++i)
            {
                fread(&currentCharacter, 1, 1, pFile);
                if (currentCharacter == 0)
                {
                    mRootFile.mTextureFileNames.push_back(currentString);
                    currentString = "";
                }
                else
                    currentString = currentString + currentCharacter;
            }
        }
        else if (!strcmp(identityBuffer, "MOMT"))
        {
            for (uint32 i = 0; i < size / 64; ++i)
            {
                WMOMaterial newMaterial;
                fread(&newMaterial.UKflags, 4, 1, pFile);
                fread(&newMaterial.UKd1, 4, 1, pFile);
                fread(&newMaterial.UKtransparent, 4, 1, pFile);
                fread(&newMaterial.UKnameStart, 4, 1, pFile);
                fread(&newMaterial.UKcol1, 4, 1, pFile);
                fread(&newMaterial.UKd3, 4, 1, pFile);
                fread(&newMaterial.UKnameEnd, 4, 1, pFile);
                fread(&newMaterial.UKcol2, 4, 1, pFile);
                fread(&newMaterial.UKd4, 4, 1, pFile);
                fread(&newMaterial.UKf1, 4, 1, pFile);
                fread(&newMaterial.UKf2, 4, 1, pFile);
                fread(&newMaterial.UKdx[0], 4, 1, pFile);
                fread(&newMaterial.UKdx[1], 4, 1, pFile);
                fread(&newMaterial.UKdx[2], 4, 1, pFile);
                fread(&newMaterial.UKdx[3], 4, 1, pFile);
                fread(&newMaterial.UKdx[4], 4, 1, pFile);
                mRootFile.mMaterials.push_back(newMaterial);
            }
        }
        else if (!strcmp(identityBuffer, "MOGN"))
        {
            //GROUPNAMEARRAYSIZE = size;
            //fread(&GROUPNAMEARRAY, size, 1, pFile);

            char currentCharacter;
            string currentString;
            for (uint32 i = 0; i < size; ++i)
            {
                fread(&currentCharacter, 1, 1, pFile);
                if (currentCharacter == 0)
                {
                    mRootFile.mGroupNames.push_back(currentString);
                    currentString = "";
                }
                else
                    currentString = currentString + currentCharacter;
            }

        }
        else if (!strcmp(identityBuffer, "MOGI"))
        {
            for (uint32 i = 0; i < size / 32; ++i)
            {
                WMOGroupInformation newGroupInformation;
                fread(&newGroupInformation.Flags, 4, 1, pFile);
                fread(&newGroupInformation.BoundBox1.X, 4, 1, pFile);
                fread(&newGroupInformation.BoundBox1.Y, 4, 1, pFile);
                fread(&newGroupInformation.BoundBox1.Z, 4, 1, pFile);
                fread(&newGroupInformation.BoundBox2.X, 4, 1, pFile);
                fread(&newGroupInformation.BoundBox2.Y, 4, 1, pFile);
                fread(&newGroupInformation.BoundBox2.Z, 4, 1, pFile);
                fread(&newGroupInformation.NameOffset, 4, 1, pFile);
                mRootFile.mGroupInformations.push_back(newGroupInformation);
            }
        }
        else if (!strcmp(identityBuffer, "MOSB"))
        {
            if (size > 4)
            {
                char* skyboxFileName = new char[size];
                fread(&skyboxFileName, size, 1, pFile);
                mRootFile.mSkyboxFileName = skyboxFileName;
                SafeDelete(skyboxFileName);
            }
            else
            {
                mRootFile.mSkyboxFileName = "";
                uint32 dumpBuffer;
                fread(&dumpBuffer, 4, 1, pFile);
            }
        }
        else if (!strcmp(identityBuffer, "MOPV"))
        {
            for (uint32 i = 0; i < size / 48; ++i)
            {
                WMOPortal newPortal;
                fread(&newPortal.P1.X, 4, 1, pFile);
                fread(&newPortal.P1.Y, 4, 1, pFile);
                fread(&newPortal.P1.Z, 4, 1, pFile);
                fread(&newPortal.P2.X, 4, 1, pFile);
                fread(&newPortal.P2.Y, 4, 1, pFile);
                fread(&newPortal.P2.Z, 4, 1, pFile);
                fread(&newPortal.P3.X, 4, 1, pFile);
                fread(&newPortal.P3.Y, 4, 1, pFile);
                fread(&newPortal.P3.Z, 4, 1, pFile);
                fread(&newPortal.P4.X, 4, 1, pFile);
                fread(&newPortal.P4.Y, 4, 1, pFile);
                fread(&newPortal.P4.Z, 4, 1, pFile);
                mRootFile.mPortals.push_back(newPortal);
            }
        }
        else if (!strcmp(identityBuffer, "MOPT"))
        {
            for (uint32 i = 0; i < size / 20; ++i)
            {
                WMOPortalInformation newPortalInformation;
                fread(&newPortalInformation.StartVertex, 2, 1, pFile);
                fread(&newPortalInformation.Count, 2, 1, pFile);
                fread(&newPortalInformation.Point1, 4, 1, pFile);
                fread(&newPortalInformation.Point2, 4, 1, pFile);
                fread(&newPortalInformation.Point3, 4, 1, pFile);
                fread(&newPortalInformation.Point4, 4, 1, pFile);
                mRootFile.mPortalInformations.push_back(newPortalInformation);
            }
        }
        else if (!strcmp(identityBuffer, "MOPR"))
        {
            for (uint32 i = 0; i < size / 8; ++i)
            {
                WMOPortalRelationship newPortalRelationship;
                fread(&newPortalRelationship.PortalIndex, 2, 1, pFile);
                fread(&newPortalRelationship.GroupIndex, 2, 1, pFile);
                fread(&newPortalRelationship.Side, 2, 1, pFile);
                fread(&newPortalRelationship.Filler, 2, 1, pFile);
                mRootFile.mPortalRelationships.push_back(newPortalRelationship);
            }
        }
        else if (!strcmp(identityBuffer, "MOVV"))
        {
            for (uint32 i = 0; i < size / 12; ++i)
            {
                // TODO: This could be an error
                Vec3D visibleBlockVertex;
                fread(&visibleBlockVertex.X, 4, 1, pFile);
                fread(&visibleBlockVertex.Y, 4, 1, pFile);
                fread(&visibleBlockVertex.Z, 4, 1, pFile);
                mRootFile.mVisibleBlockVerticies.push_back(visibleBlockVertex);
            }
        }
        else if (!strcmp(identityBuffer, "MOVB"))
        {
            for (uint32 i = 0; i < size / WMOVisibleBlockList::Size; ++i)
            {
                WMOVisibleBlockList newVisibleBlockList;
                fread(&newVisibleBlockList.FirstVertex, 2, 1, pFile);
                fread(&newVisibleBlockList.Count, 2, 1, pFile);
                mRootFile.mVisibleBlockLists.push_back(newVisibleBlockList);
            }
        }
        else if (!strcmp(identityBuffer, "MOLT"))
        {
            for (uint32 i = 0; i < size / 48; ++i)
            {
                WMOLight newLight;
                fread(&newLight.LightType, 1, 1, pFile);
                fread(&newLight.Type, 1, 1, pFile);
                fread(&newLight.UseAtten, 1, 1, pFile);
                fread(&newLight.Pad, 1, 1, pFile);
                fread(&newLight.Color[0], 1, 1, pFile);
                fread(&newLight.Color[1], 1, 1, pFile);
                fread(&newLight.Color[3], 1, 1, pFile);
                fread(&newLight.Color[4], 1, 1, pFile);
                fread(&newLight.Position.X, 4, 1, pFile);
                fread(&newLight.Position.Y, 4, 1, pFile);
                fread(&newLight.Position.Z, 4, 1, pFile);
                fread(&newLight.Intensity, 4, 1, pFile);
                fread(&newLight.AttenStart, 4, 1, pFile);
                fread(&newLight.AttenEnd, 4, 1, pFile);
                fread(&newLight.Unknown[0], 4, 1, pFile);
                fread(&newLight.Unknown[1], 4, 1, pFile);
                fread(&newLight.Unknown[2], 4, 1, pFile);
                fread(&newLight.Unknown[3], 4, 1, pFile);
                mRootFile.mLights.push_back(newLight);
            }
        }
        else if (!strcmp(identityBuffer, "MODS"))
        {
            for (uint32 i = 0; i < size / 32; ++i)
            {
                WMODoodadSet newDoodadSet;
                fread(&newDoodadSet.Name, 20, 1, pFile);
                fread(&newDoodadSet.FirstDoodadIndex, 4, 1, pFile);
                fread(&newDoodadSet.NumDoodads, 4, 1, pFile);
                fread(&newDoodadSet.Unused, 4, 1, pFile);
                mRootFile.mDoodadSets.push_back(newDoodadSet);
            }
        }
        else if (!strcmp(identityBuffer, "MODN"))
        {
            char currentCharacter;
            string currentString;
            for (uint32 i = 0; i < size; ++i)
            {
                fread(&currentCharacter, 1, 1, pFile);
                if (currentCharacter == 0)
                {
                    mRootFile.mModelFileNames.push_back(currentString);
                    currentString = "";
                }
                else
                    currentString = currentString + currentCharacter;
            }
        }
        else if (!strcmp(identityBuffer, "MODD"))
        {
            for (uint32 i = 0; i < size / 40; ++i)
            {
                WMODoodadInformation newDoodadInformation;
                fread(&newDoodadInformation.NameIndex, 4, 1, pFile);
                fread(&newDoodadInformation.Position.X, 4, 1, pFile);
                fread(&newDoodadInformation.Position.Y, 4, 1, pFile);
                fread(&newDoodadInformation.Position.Z, 4, 1, pFile);
                fread(&newDoodadInformation.RotationX, 4, 1, pFile); // May need to fiddle with this
                fread(&newDoodadInformation.RotationY, 4, 1, pFile);
                fread(&newDoodadInformation.RotationZ, 4, 1, pFile);
                fread(&newDoodadInformation.RotationW, 4, 1, pFile);
                fread(&newDoodadInformation.Scale, 4, 1, pFile);
                fread(&newDoodadInformation.Color[0], 1, 1, pFile);
                fread(&newDoodadInformation.Color[1], 1, 1, pFile);
                fread(&newDoodadInformation.Color[2], 1, 1, pFile);
                fread(&newDoodadInformation.Color[3], 1, 1, pFile);
                mRootFile.mDoodadInformations.push_back(newDoodadInformation);
            }
        }
        else if (!strcmp(identityBuffer, "MFOG"))
        {
            for (uint32 i = 0; i < size / 48; ++i)
            {
                WMOFog newFog;
                fread(&newFog.Flags, 4, 1, pFile);
                fread(&newFog.Position.X, 4, 1, pFile);
                fread(&newFog.Position.Y, 4, 1, pFile);
                fread(&newFog.Position.Z, 4, 1, pFile);
                fread(&newFog.SmallerRadius, 4, 1, pFile);
                fread(&newFog.LargerRadius, 4, 1, pFile);
                fread(&newFog.End, 4, 1, pFile);
                fread(&newFog.StartMultiplier, 4, 1, pFile);
                fread(&newFog.Color, 4, 1, pFile);
                fread(&newFog.Unknown1, 4, 1, pFile);
                fread(&newFog.Unknown2, 4, 1, pFile);
                fread(&newFog.Color2, 4, 1, pFile);
                mRootFile.mFogs.push_back(newFog);
            }
        }
        else if (!strcmp(identityBuffer, "MCVP"))
        {
/*          for (uint32 i = 0; i < size / 4; ++i)
            {
                float newConvexVolumnPlane;
                fread(&newConvexVolumnPlane, 4, 1, pFile);
                mRootFile.mConvexVolumnPlanes.push_back(newConvexVolumnPlane);
            }
            */
        }
        else // Unknown
        {
            printf("Error loading WMO, unknown segment named %s", identityBuffer);
        }
    }

    fclose(pFile);
}

void WoWMap::ReadGroupFile(string fileName)
{



}
