#include "DBCFile.h"
#include <vector>
#include <string>

bool DBCFile::LoadFromFile(const string fileName, DBCFile* pDBCFile, const DBC_FILE_TYPE fileType, Localization localization)
{
    FILE* pFile = fopen(fileName.c_str(), "rb");
    if (!pFile)
        return false;

    // Check the header, it should be "WDBC"
    fread(pDBCFile->mHeader.Code, 4, 1, pFile);
    char* pHeaderCode = pDBCFile->mHeader.Code;
    if (pHeaderCode[0] != 'W' && pHeaderCode[1] != 'D' &&
        pHeaderCode[2] != 'B' && pHeaderCode[3] != 'C')
        {
            printf("%s is not a dbf file", fileName.c_str());
            fclose(pFile);
            return false;
        }
    pHeaderCode = NULL;

    // Read the rest of the header
    fread(&pDBCFile->mHeader.NumRecords, 4, 1, pFile);
    fread(&pDBCFile->mHeader.NumFields, 4, 1, pFile);
    fread(&pDBCFile->mHeader.RecordSize, 4, 1, pFile);
    fread(&pDBCFile->mHeader.StringBlockSize, 4, 1, pFile);

    // TODO: Debugging
    int32 numRecords = pDBCFile->mHeader.NumRecords;
    int32 numFields = pDBCFile->mHeader.NumFields;
    int32 recordSize = pDBCFile->mHeader.RecordSize;
    int32 stringBlockSize = pDBCFile->mHeader.StringBlockSize;

    // Fill with all the records
    for (uint32 i = 0; i < pDBCFile->mHeader.NumRecords; ++i)
    {
        switch(fileType)
        {
            case DBC_TYPE_AREA_TABLE:
            {
                DBCAreaTableRecord* newDBCAreaTableRecord = new DBCAreaTableRecord();

                fread(&newDBCAreaTableRecord->ID, sizeof(int32), 1, pFile);
                fread(&newDBCAreaTableRecord->MapID, sizeof(int32), 1, pFile);
                fread(&newDBCAreaTableRecord->AreaTableID, sizeof(int32), 1, pFile);
                fread(&newDBCAreaTableRecord->ExploreFlag, sizeof(int32), 1, pFile);
                fread(&newDBCAreaTableRecord->Flags, sizeof(int32), 1, pFile);
                fread(&newDBCAreaTableRecord->Unknown, sizeof(int32)*2, 1, pFile);
                fread(&newDBCAreaTableRecord->SoundAmbienceID, sizeof(int32), 1, pFile);
                fread(&newDBCAreaTableRecord->ZoneMusicID, sizeof(int32), 1, pFile);
                fread(&newDBCAreaTableRecord->ZoneIntroMusicTableID, sizeof(int32), 1, pFile);
                fread(&newDBCAreaTableRecord->AreaLevel, sizeof(int32), 1, pFile);
                newDBCAreaTableRecord->NameOffset = GetStringOffsetFromFile(pFile, localization);
                fread(&newDBCAreaTableRecord->Unknown2, sizeof(int32)*7, 1, pFile);

                DBCAreaTableFile *pWorkingFile = static_cast<DBCAreaTableFile*>(pDBCFile);
                pWorkingFile->mRecords.push_back(newDBCAreaTableRecord);
            }break;
            case DBC_TYPE_MAP:
            {
                DBCMapRecord* newDBCMapRecord = new DBCMapRecord();

                fread(&newDBCMapRecord->ID, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->InternalName, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->MapType, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->IsBattleground, sizeof(int32), 1, pFile);
                newDBCMapRecord->NameOffset = GetStringOffsetFromFile(pFile, localization);
                fread(&newDBCMapRecord->MinLevel, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->MaxLevel, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->MaxPlayers, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->Unknown1, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->Unknown2, sizeof(float), 1, pFile);
                fread(&newDBCMapRecord->Unknown3, sizeof(float), 1, pFile);
                fread(&newDBCMapRecord->AreaTableID, sizeof(int32), 1, pFile);
                newDBCMapRecord->HordeDescOffset = GetStringOffsetFromFile(pFile, localization);
                newDBCMapRecord->AllianceDescOffset = GetStringOffsetFromFile(pFile, localization);
                fread(&newDBCMapRecord->LoadingScreensID, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->LevelSteps, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->Unknown4, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->Unknown5, sizeof(float), 1, pFile);
                newDBCMapRecord->Unknown6Offset = GetStringOffsetFromFile(pFile, localization);
                newDBCMapRecord->HeroicReqDescOffset = GetStringOffsetFromFile(pFile, localization);
                newDBCMapRecord->Unknown7Offset = GetStringOffsetFromFile(pFile, localization);
                fread(&newDBCMapRecord->ParentMapID, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->Unknown8, sizeof(float), 1, pFile);
                fread(&newDBCMapRecord->Unknown9, sizeof(float), 1, pFile);
                fread(&newDBCMapRecord->Unknown10, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->WingedDungeon, sizeof(int32), 1, pFile);
                fread(&newDBCMapRecord->Unknown11, sizeof(int32), 1, pFile);

                DBCMapFile *pWorkingFile = static_cast<DBCMapFile*>(pDBCFile);
                pWorkingFile->mRecords.push_back(newDBCMapRecord);
            }break;
            case DBC_TYPE_WMO_AREA_TABLE:
            {
                DBCWMOAreaTableRecord* newDBCWMOAreaTableRecord = new DBCWMOAreaTableRecord();
                fread(&newDBCWMOAreaTableRecord->ID, sizeof(int32), 1, pFile);
                fread(&newDBCWMOAreaTableRecord->RootID, sizeof(int32), 1, pFile);
                fread(&newDBCWMOAreaTableRecord->Unknown1, sizeof(int32), 1, pFile);
                fread(&newDBCWMOAreaTableRecord->GroupID, sizeof(int32), 1, pFile);
                fread(&newDBCWMOAreaTableRecord->Unknown2, sizeof(int32)*7, 1, pFile);
                newDBCWMOAreaTableRecord->NameOffset = GetStringOffsetFromFile(pFile, localization);

                DBCWMOAreaTableFile *pWorkingFile = static_cast<DBCWMOAreaTableFile*>(pDBCFile);
                pWorkingFile->mRecords.push_back(newDBCWMOAreaTableRecord);
            }break;
        };
    }

    // Pull the string buffer
    char* stringBuffer = new char[pDBCFile->mHeader.StringBlockSize];file:///usr/share/ubuntu-artwork/home/index.html
    fread(stringBuffer, pDBCFile->mHeader.StringBlockSize, 1, pFile);

    // Map the strings
    for (uint32 i = 0; i < pDBCFile->mHeader.NumRecords; ++i)
    {
        switch(fileType)
        {
            case DBC_TYPE_AREA_TABLE:
            {
                DBCAreaTableFile *pWorkingFile = static_cast<DBCAreaTableFile*>(pDBCFile);
                DBCAreaTableRecord* pCurRecord = pWorkingFile->mRecords.at(i);
                string test = pCurRecord->Name = GetStringFromBuffer(stringBuffer, pDBCFile->mHeader.StringBlockSize, pCurRecord->NameOffset);
            }break;
            case DBC_TYPE_MAP:
            {
                DBCMapFile *pWorkingFile = static_cast<DBCMapFile*>(pDBCFile);
                DBCMapRecord* pCurRecord = pWorkingFile->mRecords.at(i);

                pCurRecord->Name = GetStringFromBuffer(stringBuffer, pDBCFile->mHeader.StringBlockSize, pCurRecord->NameOffset);
                pCurRecord->HordeDescription = GetStringFromBuffer(stringBuffer, pDBCFile->mHeader.StringBlockSize, pCurRecord->HordeDescOffset);
                pCurRecord->AllianceDescription = GetStringFromBuffer(stringBuffer, pDBCFile->mHeader.StringBlockSize, pCurRecord->AllianceDescOffset);
                pCurRecord->Unknown6 = GetStringFromBuffer(stringBuffer, pDBCFile->mHeader.StringBlockSize, pCurRecord->Unknown6Offset);
                pCurRecord->HeroicRequirementDescription = GetStringFromBuffer(stringBuffer, pDBCFile->mHeader.StringBlockSize, pCurRecord->HeroicReqDescOffset);
                pCurRecord->Unknown7 = GetStringFromBuffer(stringBuffer, pDBCFile->mHeader.StringBlockSize, pCurRecord->Unknown7Offset);
            }break;
            case DBC_TYPE_WMO_AREA_TABLE:
            {
                DBCWMOAreaTableFile *pWorkingFile = static_cast<DBCWMOAreaTableFile*>(pDBCFile);
                DBCWMOAreaTableRecord* pCurRecord = pWorkingFile->mRecords.at(i);
                pCurRecord->Name = GetStringFromBuffer(stringBuffer, pDBCFile->mHeader.StringBlockSize, pCurRecord->NameOffset);
            }break;
        }
    }

    SafeDelete(stringBuffer);
    fclose(pFile);
    return true;
};

int32 DBCFile::GetStringOffsetFromFile(FILE* pFile, Localization localization)
{
    int32 returnOffset = 0;

    for (int i = 0; i < 17; i++)
    {
        if (i == localization)
            fread(&returnOffset, sizeof(int32), 1, pFile);
        else
        {
            int32 dumpBuffer;
            fread(&dumpBuffer, sizeof(int32), 1, pFile);
        }
    }

    return returnOffset;
}

string DBCFile::GetStringFromBuffer(const char* buffer, const uint bufferLength, const int32 offset)
{
    string returnString = "";

    for(uint i = offset; i < bufferLength; ++i)
    {
        if (buffer[i] == '\0')
            return returnString;
        else
            returnString = returnString + buffer[i];
    }

    // Not a string
    return "";
}
