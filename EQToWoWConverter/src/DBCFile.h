#ifndef _DBCFILE_H_INCLUDED_
#define _DBCFILE_H_INCLUDED_

#include "dbc.h"
#include "Memory.h"

///////////////////////////////////////////////////////////////////////////////

enum Localization
{
    LOCAL_ENGLISH = 0
};

enum DBC_FILE_TYPE
{
    DBC_TYPE_MAP,
    DBC_TYPE_AREA_TABLE,
    DBC_TYPE_WMO_AREA_TABLE
};

///////////////////////////////////////////////////////////////////////////////

class DBCFile
{
public:
    DBCFile() {}
    ~DBCFile() {}

private:
    virtual void DestroyRecords() {}
    int32 GetStringOffsetFromFile(FILE* pFile, Localization localization);
    string GetStringFromBuffer(const char* buffer, const uint bufferLength, const int32 offset);

protected:
    bool LoadFromFile(const string fileName, DBCFile* pDBCFile, const DBC_FILE_TYPE fileType,
        Localization localization);

public:
    virtual void Load(string fileName) {}

protected:
    bool mIsLoaded;

public:
    DBCHeader mHeader;
    char* mStringBlock;

public:
    const bool IsLoaded() { return mIsLoaded; }
};

///////////////////////////////////////////////////////////////////////////////

class DBCAreaTableFile : public DBCFile
{
public:
    DBCAreaTableFile() {mIsLoaded = false;}
    ~DBCAreaTableFile() { DestroyRecords(); }

public:
    void Load(string fileName)
    {   DBCFile* pThisFile = static_cast<DBCFile*>(this);
         mIsLoaded = LoadFromFile(fileName, pThisFile, DBC_TYPE_AREA_TABLE, LOCAL_ENGLISH); }

private:
    void DestroyRecords() {
        for (uint i = mRecords.size(); i > 0; i--)
            SafeDelete(mRecords.at(i-1));
    }

public:
    vector<DBCAreaTableRecord*> mRecords;
};

///////////////////////////////////////////////////////////////////////////////

class DBCMapFile : public DBCFile
{
public:
    DBCMapFile() {mIsLoaded = false;}
    ~DBCMapFile() { DestroyRecords(); }

public:
    void Load(string fileName)
    {   DBCFile* pThisFile = static_cast<DBCFile*>(this);
         mIsLoaded = LoadFromFile(fileName, pThisFile, DBC_TYPE_MAP, LOCAL_ENGLISH); }

private:
    void DestroyRecords() {
        for (uint i = mRecords.size(); i > 0; i--)
            SafeDelete(mRecords.at(i-1));
    }

public:
    vector<DBCMapRecord*> mRecords;
};

///////////////////////////////////////////////////////////////////////////////

class DBCWMOAreaTableFile : public DBCFile
{
public:
    DBCWMOAreaTableFile() {mIsLoaded = false;}
    ~DBCWMOAreaTableFile() { DestroyRecords(); }

public:
    void Load(string fileName)
    {   DBCFile* pThisFile = static_cast<DBCFile*>(this);
         mIsLoaded = LoadFromFile(fileName, pThisFile, DBC_TYPE_WMO_AREA_TABLE, LOCAL_ENGLISH); }

private:
    void DestroyRecords() {
        for (uint i = mRecords.size(); i > 0; i--)
            SafeDelete(mRecords.at(i-1));
    }

public:
    vector<DBCWMOAreaTableRecord*> mRecords;
};

#endif // _DBCFILE_H_INCLUDED_
