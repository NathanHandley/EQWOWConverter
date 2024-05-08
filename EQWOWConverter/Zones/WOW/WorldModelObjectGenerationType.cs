using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    // Potential options
    // - Group on Z height (bottom up?)
    // - Group on normal direction ('floors' vs 'walls'?)
    // - Group based on range (start in a corner and group nearest
    // - Group connected triangles (stop if there are no connected)
    internal enum WorldModelObjectGenerationType
    {
        BY_TEXTURE,         // Group faces by material texture
        BY_MAP_CHUNK,       // Group by EQ MapChunks
        BY_XY_REGION        // Generate by breaking up the map into x/y regions (ignoring Z)
    }
}
