//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace EQWOWConverter.Zones.Properties
{
    internal class CazicThuleZoneProperties : ZoneProperties
    {
        public CazicThuleZoneProperties() : base()
        {
            // TODO: Indoor Lighting
            // TODO: Bug: Near the entry pools, the textures collide on the surface          
            AddZoneArea("Archon Pyramid");
            AddZoneAreaBox("Archon Pyramid", 1224.566162f, -26.975229f, 213.322754f, 711.677063f, -519.299316f, 26.708981f);
            
            AddZoneArea("Archon Pools");
            AddZoneAreaBox("Archon Pools", 1097.259155f, -132.695816f, 43.793690f, 807.896057f, -424.505066f, -174.853439f);
            
            AddZoneArea("Avatar of Fear Shrine");
            AddZoneAreaBox("Avatar of Fear Shrine", 1450.210083f, 773.020630f, 84.898651f, 828.155518f, 345.046356f, -8.928060f);
            AddZoneAreaBox("Avatar of Fear Shrine", 1203.429443f, 691.566345f, -4.095730f, 993.050110f, 484.290924f, -120f);

            AddZoneArea("Sewers");
            AddZoneAreaBox("Sewers", 838.692871f, 616.776123f, -36.019669f, 455.754242f, 119.732773f, -89f);
            AddZoneAreaBox("Sewers", 1037.522583f, 401.439331f, -14.377440f, 604.456482f, 71.858856f, -146.028687f);

            AddZoneArea("Gator Pit");
            AddZoneAreaBox("Gator Pit", 257.812225f, -51.112339f, -154.093826f, -144.449753f, -653.038757f, -311.966217f);

            AddZoneArea("High Throne");
            AddZoneAreaBox("High Throne", 403.826721f, 418.116028f, 72.933083f, 208.478760f, 167.585373f, -1.137330f);

            AddZoneArea("The Maze");
            AddZoneAreaBox("The Maze", 656.258179f, -73.121597f, 108.121300f, 85.339203f, -534.369446f, -19.916740f);
            AddZoneAreaBox("The Maze", 203.535263f, -91.370308f, 108.121300f, -127.016510f, -534.369446f, -19.916740f);

            AddZoneLineBox("feerrott", -1460.633545f, -109.760483f, 47.935600f, ZoneLineOrientationType.North, 42.322739f, -55.775299f, 10.469000f, -0.193150f, -84.162201f, -0.500000f);
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t50_grnwtr1", 219.559280f, -267.584229f, 118.939217f, -513.355408f, -209.916219f, 100f); // Bottom southmost east green pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 205.585724f, -62.912350f, 7.619390f, -261.991211f, -209.916235f, 150f); // Bottom southmost west blue pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 34.715931f, 6.890010f, 20.303070f, -7.677340f, -41.968750f, 250f); // Well into the southmost area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 72.394638f, -7.667340f, -13.658730f, -63.726299f, -217.888000f, 100f); // Path between well into the bottom southwest west blue pool (north part)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 20.313070f, 11.799870f, -13.658730f, -7.677340f, -217.888000f, 100f); // Path between well into the bottom southwest west blue pool (south part)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 407.618530f, 43.798050f, 389.914734f, 19.075970f, 16.000031f, 3.25f); // Main level (southern area) two pools in front of the mask and door, west pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 407.731812f, -19.086250f, 390.065369f, -43.806450f, 16.000031f, 200f); // Main level (southern area) two pools in front of the mask and door, east pool well
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 523.011658f, 126.719681f, 357.898254f, -19.096250f, -83.968727f, 200f); // Second southern lower area with two ladder
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 523.011658f, -19.086250f, 407.741812f, -89.008179f, -83.968727f, 200f); // Second southern lower area, bottom of well north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 390.075369f, -19.086250f, 357.898254f, -89.008179f, -83.968727f, 200f); // Second southern lower area, bottom of well south
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 407.731812f, -43.796450f, 390.065369f, -89.008179f, -83.968727f, 200f); // Second southern lower area, bottom of well east
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 982.359985f, 1.667420f, 919.420166f, -125.411972f, -83.968719f, 200f); // Lower area just west of the pyramid, west of the 'box room'
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1095.464600f, -102.121727f, 808.316101f, -223.348434f, -83.968719f, 200f); // Box room, west north-to-south strip
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1095.464600f, -266.567535f, 808.316101f, -293.227356f, -83.968719f, 200f); // Box room, middle north-to-south strip
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1095.464600f, -336.594269f, 808.316101f, -425.962585f, -83.968719f, 200f); // Box room, east north-to-south strip
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1095.464600f, -102.121727f, 1010.181763f, -425.962585f, -83.968719f, 200f); // Box room, north west-to-east strip
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 966.708252f, -102.121727f, 936.318237f, -425.962585f, -83.968719f, 200f); // Box room, middle west-to-east strip
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 892.906250f, -102.121727f, 808.316101f, -425.962585f, -83.968719f, 200f); // Box room, south west-to-east strip
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1012.899658f, -220.574554f, 964.489685f, -269.064453f, -102.968727f, 200f); // Box room, northwest lower inset box
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1012.693848f, -290.770050f, 964.413025f, -338.923492f, -102.968727f, 200f); // Box room, northeast lower inset box
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 938.528625f, -220.663116f, 890.579651f, -268.775848f, -102.968727f, 200f); // Box room, southwest lower inset box
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 938.573975f, -290.665588f, 890.756714f, -338.776917f, -102.968727f, 200f); // Box room, southeast lower inset box
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1046.981201f, -184.259537f, 1023.519897f, -207.940338f, 12.000030f, 25.25f); // Box room, top floating boxes, NW
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1047.296997f, -352.026215f, 1023.714478f, -375.895355f, 12.000030f, 25.25f); // Box room, top floating boxes, NE
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 879.563843f, -184.107864f, 855.742249f, -208.065643f, 12.000030f, 25.25f); // Box room, top floating boxes, SW
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 879.431519f, -351.990387f, 855.728210f, -375.791809f, 12.000030f, 25.25f); // Box room, top floating boxes, SE
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1068.337280f, -268.078522f, 1044.354248f, -292.106171f, -1.999960f, 25.25f); // Box room, top floating boxes, N
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 963.263550f, -163.235046f, 939.509338f, -186.844406f, -1.999960f, 25.25f); // Box room, top floating boxes, W
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 858.450073f, -267.749237f, 834.732056f, -291.823425f, -1.999960f, 25.25f); // Box room, top floating boxes, S
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 963.366028f, -373.022614f, 939.551147f, -396.703369f, -1.999960f, 25.25f); // Box room, top floating boxes, E
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1089.363037f, -212.148621f, 1065.942749f, -235.578781f, -15.999990f, 25.25f); // Box room, bottom floating boxes, NW
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1089.126099f, -324.102631f, 1065.771240f, -347.771088f, -15.999990f, 25.25f); // Box room, bottom floating boxes, NE
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 837.303040f, -212.146423f, 813.819275f, -235.875565f, -15.999990f, 25.25f); // Box room, bottom floating boxes, SW
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 837.325134f, -324.070801f, 813.694092f, -347.815552f, -15.999990f, 25.25f); // Box room, bottom floating boxes, SE
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1019.393066f, -142.063370f, 995.592651f, -166.004639f, -15.999990f, 25.25f); // Box room, bottom floating boxes, WN
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 907.446106f, -142.299225f, 883.726135f, -165.849716f, -15.999990f, 25.25f); // Box room, bottom floating boxes, WS
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1019.354065f, -394.024109f, 995.515015f, -417.751160f, -15.999990f, 25.25f); // Box room, bottom floating boxes, EN
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 907.568542f, -394.017395f, 883.673218f, -417.884613f, -15.999990f, 25.25f); // Box room, bottom floating boxes, ES
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 955.654907f, -275.840149f, 947.576904f, -283.912537f, 105.968758f, 7.75f); // Box room, water column cap
            AddOctagonLiquidShape(ZoneLiquidType.Water, "t50_water1", 955.654907f, 947.576904f, -275.840149f, -283.912537f, -278.828613f, -280.928589f,
                -278.828613f, -280.928589f, 952.661194f, 950.571838f, 952.661194f, 950.571838f, 105.968758f, 118f, 0.4f); // Box room, water column
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 980.440186f, -254.128174f, 923.192810f, -305.867035f, 0.001150f, 13.5f); // Box room, upper main pool 1
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 977.539307f, -251.266257f, 923.788025f, -308.644434f, 0.001150f, 13.5f); // Box room, upper main pool 2
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t50_grnwtr1", 1000.311096f, 234.174606f, 678.456055f, 89.293968f, -71.968689f, 50f); // Green Pools, 2 NE pools (one north of the other)
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t50_grnwtr1", 607.513184f, 430.828156f, 470.775665f, 123.239029f, -71.968689f, 50f); // Green Pools, 2 SW pools (one west of the other)
            AddLiquidPlaneZLevel(ZoneLiquidType.GreenWater, "t50_grnwtr1", 822.394958f, 609.747925f, 682.704468f, 287.781982f, -71.968689f, 50f); // Green Pools, 2 NW pools (one west of the other)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_water1", 1152.813599f, 646.499207f, 1034.159546f, 530.071350f, -41.978620f, 6.2f); // Water above green orb
            AddCazicThuleLiquidSphere(ZoneLiquidType.GreenWater, "t50_grnwtr1");
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        public void AddCazicThuleLiquidSphere(ZoneLiquidType liquidType, string materialName)
        {
            // TODO: Redo this to be calculated smarter. It's very hacky now. Consider that the "sphere" is wider than it is tall.

            // Set boundaries
            float maxX = 1115.565186f;
            float maxY = 611.748718f;
            float maxZ = -49.992290f; // Top of sphere
            float minZ = -93f;  // Bottom of sphere
            float sphereRadius = 24f;
            float sphereCenterX = 1091.563904f;
            float sphereCenterY = 587.748718f;
            float sphereTrueCenterZ = -71.49229f;

            // Create the center column
            AddLiquidPlaneZLevel(liquidType, materialName, sphereCenterX + 4.01f, sphereCenterY + 4.01f, sphereCenterX - 4.01f, sphereCenterY - 4.01f, maxZ, (maxZ - sphereTrueCenterZ) * 2f);

            // Walk across the x in 2 unit steps, total of 48. Center column is 8 units.
            for (int xi = 0; xi < 48; xi += 2)
            {
                // Walk across the y in 2 unit steps, total of 48.  Center column is 8 units.
                for (int yi = 0; yi < 48; yi += 2)
                {
                    // Skip center column
                    if (yi > 20 && yi < 27 && xi > 20 && xi < 27)
                        continue;

                    // Skip the corners by seeing if this point is in the sphere's max circle.  Test 4 points just inside the square this occupies
                    float testXPosition = maxX - (Convert.ToSingle(xi) + 1.25f); // nw
                    float testYPosition = maxY - (Convert.ToSingle(yi) + 1.25f); // nw
                    float distanceSquared = MathF.Pow(testXPosition - sphereCenterX, 2) + MathF.Pow(testYPosition - sphereCenterY, 2);
                    if (distanceSquared <= MathF.Pow(sphereRadius, 2) == false)
                    {
                        testXPosition = maxX - (Convert.ToSingle(xi) + 1.25f); // ne
                        testYPosition = maxY - (Convert.ToSingle(yi) + 0.75f); // ne
                        distanceSquared = MathF.Pow(testXPosition - sphereCenterX, 2) + MathF.Pow(testYPosition - sphereCenterY, 2);
                        if (distanceSquared <= MathF.Pow(sphereRadius, 2) == false)
                        {
                            testXPosition = maxX - (Convert.ToSingle(xi) + 0.75f); // sw
                            testYPosition = maxY - (Convert.ToSingle(yi) + 1.25f); // sw
                            distanceSquared = MathF.Pow(testXPosition - sphereCenterX, 2) + MathF.Pow(testYPosition - sphereCenterY, 2);
                            if (distanceSquared <= MathF.Pow(sphereRadius, 2) == false)
                            {
                                testXPosition = maxX - (Convert.ToSingle(xi) + 0.75f); // se
                                testYPosition = maxY - (Convert.ToSingle(yi) + 0.75f); // se
                                distanceSquared = MathF.Pow(testXPosition - sphereCenterX, 2) + MathF.Pow(testYPosition - sphereCenterY, 2);
                                if (distanceSquared <= MathF.Pow(sphereRadius, 2) == false)
                                    continue;
                            }
                        }
                    }

                    // Calculate the size of tiles
                    float tileXSize = 2f;
                    float tileYSize = 2f;
                    if (yi == 19)
                        tileYSize = 8f;
                    if (xi == 19)
                        tileXSize = 8f;

                    // Calculate the height
                    float curZ = maxZ;

                    // Calculate the square
                    float curTopX = maxX - Convert.ToSingle(xi);
                    float curBottomX = maxX - (Convert.ToSingle(xi) + tileXSize + 0.01f);
                    float curTopY = maxY - Convert.ToSingle(yi);
                    float curBottomY = maxY - (Convert.ToSingle(yi) + tileYSize + 0.01f);

                    // Get the center of the tile
                    float relativeTileCenterX = ((curTopX + curBottomX) * 0.5f) - sphereCenterX;
                    float relativeTileCenterY = ((curTopY + curBottomY) * 0.5f) - sphereCenterY;

                    // Get distance from the center
                    float distanceFromCenter = MathF.Sqrt((relativeTileCenterX * relativeTileCenterX) + (relativeTileCenterY * relativeTileCenterY));

                    // Reduce based on distance from center 
                    distanceFromCenter -= 10f; // Remove the center area
                    float workingRadius = 35f;
                    float proportionToMaxDistance = distanceFromCenter / workingRadius;
                    float dropDownAmount = (proportionToMaxDistance * 42f);
                    curZ -= dropDownAmount;
                    curZ = MathF.Min(curZ, maxZ);

                    // Calculate the depth
                    float curDepth = ((curZ - sphereTrueCenterZ) * 2f) + 2f;
                    float maxDepth = curZ - minZ;
                    curDepth = MathF.Min(maxDepth, curDepth);

                    // Advance if it was on a long edge
                    // Calculate the size of tiles
                    if (yi == 19)
                        yi += 6;
                    if (xi == 19)
                        xi += 6;

                    // Create the plane
                    AddLiquidPlaneZLevel(liquidType, materialName, curTopX, curTopY, curBottomX, curBottomY, curZ, curDepth);
                }
            }
        }
    }
}
