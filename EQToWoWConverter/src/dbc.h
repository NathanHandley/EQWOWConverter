#ifndef _DBC_H_INCLUDED_
#define _DBC_H_INCLUDED_

#include <vector>
#include "Memory.h"
#include "Types.h"
#include <string>

using namespace std;

// All DBC files have the same header
struct DBCHeader
{
    char Code[4];               // Always WDBC
    uint32 NumRecords;
    uint32 NumFields;
    uint32 RecordSize;          // Typically mNumFields * 4
    uint32 StringBlockSize;
};

// All records have an ID as the first column
class DBCRecord
{
public:
    DBCRecord() {}
    ~DBCRecord() {}

public:
    int32 ID;                       // 0
};

///////////////////////////////////////////////////////////////////////////////

/* AreaTable.dbc
Game Version: 2.3.3
0       Index
1       MapID                   - /ref_Map/ This pointer will refer only to the continents.
2       AreaTableID              - /ref_AreaTable/ Recursive, if this is a sub-area it will point to parent area
3       ExploreFlag             - Could be wrong
4       Flags                   - BitMask? Possibly 312 for cities
5       Unknown
6       Unknown
7       SoundAmbience           - /ref_SoundAmbience/
8       ZoneMusic               - /ref_ZoneMusic/ The zone music
9       ZoneIntroMusicTableID   - /ref_ZoneIntroMusicTable/
10      AreaLevel
11-27   Name                    - Map Name
28-34   Unknown
*/
class DBCAreaTableRecord : public DBCRecord
{
public:
    DBCAreaTableRecord() {}
    ~DBCAreaTableRecord() {}

public:
    int32 MapID;                    // 1
    int32 AreaTableID;              // 2
    int32 ExploreFlag;              // 3
    int32 Flags;                    // 4
    int32 Unknown[2];               // 5-6
    int32 SoundAmbienceID;          // 7
    int32 ZoneMusicID;              // 8
    int32 ZoneIntroMusicTableID;    // 9
    int32 AreaLevel;                // 10
    int32 NameOffset;               // 11-27
    int Unknown2[7];                // 28-34

    string Name;
};

///////////////////////////////////////////////////////////////////////////////

/* Map.dbc
Game Version: 2.3.3
0       Index
1       InternalName                - Seems small, TODO: test this
2       MapType                     - 0 = Normal, 1 = Dungeon, 2 = Raid, 3 = PVP Zone, 4 = Arena
3       IsBattleground              - It looks to be 4 bytes, TODO: need to compare the value
4-20    Name
21      MinLevel                    - Minimum level to enter/ Only set for BGs/Arenas
22      MaxLevel                    - Maximum level to enter/ Only set for BGs/Arenas
23      MaxPlayers                  - Maximum number of players/ Only set for BGs/Arenas
24      Unknown                     - -1 for all except BRS / BRD / PVPZone02 (Azshara)
25      Unknown                     - Only set on PVPZone01 & 02 (Alterac and Azshara)
26      Unknown                     - Only set on PVPZone01 & 02 (Alterac and Azshara)
27      AreaTableID                 - /ref_AreaTable/
28-44   HordeDescription            - Shown only for horde
45-61   AllianceDescription         - Shown only for alliance
62      LoadingScreensID            - /ref_LoadingScreens/ ?
63      LevelSteps                  - Increment in level per battlefield instance (?)
64      Unknown                     - All 1 except for BFD (bool?)
65      Unknown                     - All 1.0 except PVPZone04 (Arathi)
66-82   Unknown                     - Unknown mapping to string block (6320?)
83-99   HeroicRequirement           - Description for heroic requirement
100-116 Unknown                     - Unknown mapping to string block (6320?)
117     ParentMapID                 - Not always right. -1 for unfinished/unaccessable (and in Azeroth)
118     Unknown
119     Unknown
120     Unknown                     - 0x93A80 = 40-25-10man raid, 0x3F480 = 20-man raid, 0x15180 = ? (only for ED)
121     WingedDungeon               - 86400 (0x15180) if winged.  Doesn't apply to raid dungeons
122     Unknown
*/

class DBCMapRecord : public DBCRecord
{
public:
    DBCMapRecord() {}
    ~DBCMapRecord() {}

public:
    int32 InternalName;             // 1
    int32 MapType;                  // 2
    int32 IsBattleground;           // 3
    int32 NameOffset;               // 4-20
    int32 MinLevel;                 // 21
    int32 MaxLevel;                 // 22
    int32 MaxPlayers;               // 23
    int32 Unknown1;                 // 24
    float Unknown2;                 // 25
    float Unknown3;                 // 26
    int32 AreaTableID;              // 27
    int32 HordeDescOffset;          // 28-44
    int32 AllianceDescOffset;       // 45-61
    int32 LoadingScreensID;         // 62
    int32 LevelSteps;               // 63
    int32 Unknown4;                 // 64
    float Unknown5;                 // 65
    int32 Unknown6Offset;           // 66-82
    int32 HeroicReqDescOffset;      // 83-99
    int32 Unknown7Offset;           // 100-116
    int32 ParentMapID;              // 117
    float Unknown8;                 // 118
    float Unknown9;                 // 119
    int32 Unknown10;                // 120
    int32 WingedDungeon;            // 121
    int32 Unknown11;                // 122

    string Name;
    string HordeDescription;
    string AllianceDescription;
    string Unknown6;
    string HeroicRequirementDescription;
    string Unknown7;
};

///////////////////////////////////////////////////////////////////////////////

/* WMOAreaTable.dbc
// TODO: Clean this up
Game Version: 2.3.3
0       ID
1       RootID                      - WMO-Root file?
2       Unknown
3       GroupID                     - WMO-Group file? -1 for "overall" like "Stormwind".  all 0's if this is not -1
4-10    Unknown
11-20   Name                        - (Localization) Name
*/

// TODO: This still needs to be figured out
class DBCWMOAreaTableRecord : public DBCRecord
{
public:
    DBCWMOAreaTableRecord() {}
    ~DBCWMOAreaTableRecord() {}

public:
    int32 RootID;                   // 1
    int32 Unknown1;                 // 2
    int32 GroupID;                  // 3
    int32 Unknown2[7];              // 4-10
    int32 NameOffset;               // 11-20

    string Name;
};

#endif // _DBC_H_INCLUDED_
