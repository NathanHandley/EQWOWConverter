#ifndef CONVERTER_H_INCLUDED
#define CONVERTER_H_INCLUDED

#include "Vec.h"
#include "wmo.h"
#include "EQMap.h"
#include "WoWMap.h"

class Converter
{
private:
        Converter() {}
        ~Converter() {}
public:
    static void ConvertEQMapToWoWMap(EQMap* eqMap, const string name, WoWMap* returnMap);
};

#endif // CONVERTER_H_INCLUDED
