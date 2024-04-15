#ifndef _WOWMAP_H_INCLUDED_
#define _WOWMAP_H_INCLUDED_

#include "wmo.h"
#include <vector>
#include <string>

using namespace std;

class WoWMapChunk
{




};

///////////////////////////////////////////////////////////////////////////////
// NOTES:
// - World\wmo\Everquest\World\ will be the base root for world
// - World\wmo\Everquest\Dungeon\ will be the base root for dungeons
// For World ID-- going to use range 3100 - 3600 inclusive

class WoWMap
{
public:
    WoWMap();
    ~WoWMap();

private:
    string mName;
    uint32 mVersion;

public:
    WMORootFile mRootFile;
    vector<WMOGroupFile*> mGroupFiles;

private:
    void FlipIdentifier(char* pIdentifier);
    void WriteChunkHeader(string identifier, uint32 size, FILE* pFile);
    void ReadChunkHeader(char* pIdentifier, uint32* pSize, FILE* pFile);
    string ConcatStringVector(vector<string> stringList);

    void WriteRootFile(string fileName);
    void WriteGroupFile(string fileName, uint32 index);

    void ReadRootFile(string fileName);
    void ReadGroupFile(string fileName);

public:
    void Create(string name);           // TODO:
    void WriteToDisk(string folder);    // Folders needs the trailing /
    void ReadFromDisk(string folder);
    void SetVersion(uint32 version) { mVersion = version; }
};


#endif // _WOWMAP_H_INCLUDED_
