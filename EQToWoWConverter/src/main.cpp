#include <iostream>

#include "WoWMap.h"
#include "EQMap.h"
#include "Converter.h"
#include "DBCFile.h"

using namespace std;

int main()
{
    /*
    EQMap testMap;
    testMap.Load("/home/nathan/Development/MergedWorlds/MapChunks/Halas/", "halas");

    WoWMap newWoWMap;
    Converter::ConvertEQMapToWoWMap(&testMap, "LavaDungeon", &newWoWMap);

    // Why is this comming back null?  hmmmm
    newWoWMap.WriteToDisk("/home/nathan/Development/MergedWorlds/MapChunks/");
    */

    WoWMap currentWoWMap;
    currentWoWMap.Create("LavaDungeon");
 //   currentWoWMap.ReadFromDisk("/home/nathan/Development/MergedWorlds/MapChunks/");
    currentWoWMap.ReadFromDisk("/home/nathan/Development/MergedWorlds/MapChunks/World/wmo/Dungeon/KL_OrgrimmarLavaDungeon/");
 //   currentWoWMap.SetVersion(900);
    currentWoWMap.WriteToDisk("/home/nathan/Development/MergedWorlds/MapChunks/");

/*    DBCAreaTableFile* pAreaTableFile = new DBCAreaTableFile();
    pAreaTableFile->Load("/home/nathan/Development/MergedWorlds/MapChunks/DBFilesClient/AreaTable.dbc");
    SafeDelete(pAreaTableFile);

    DBCWMOAreaTableFile* pWMOAreaTableFile = new DBCWMOAreaTableFile();
    pWMOAreaTableFile->Load("/home/nathan/Development/MergedWorlds/MapChunks/DBFilesClient/WMOAreaTable.dbc");
    SafeDelete(pWMOAreaTableFile);

    DBCMapFile* pMapFile = new DBCMapFile();
    pMapFile->Load("/home/nathan/Development/MergedWorlds/MapChunks/DBFilesClient/Map.dbc");
    SafeDelete(pMapFile); */

    return 0;
}
