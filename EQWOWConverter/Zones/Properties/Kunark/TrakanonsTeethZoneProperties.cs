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
    internal class TrakanonsTeethZoneProperties : ZoneProperties
    {
        public TrakanonsTeethZoneProperties() : base()
        {
            // Big lake in the open air
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_w1b", 2027.573242f, 2519.995117f, -308.817078f, 766.641052f, -450.531128f, 500f);

            // River and cave lake
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1849.669067f, 2148.218994f, 1808.432617f, 2142.884521f, -445.021027f, -450.463287f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1848.512329f, 2195.042480f, 1810.330200f, 2148.184326f, -421.902649f, -445.024475f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1867.952271f, 2201.937012f, 1816.423096f, 2195.008057f, -420.754028f, -421.906067f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1864.834595f, 2208.954102f, 1818.095093f, 2201.902588f, -414.203766f, -420.757507f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1860.012817f, 2220.198730f, 1809.942505f, 2208.919678f, -409.132111f, -414.207214f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1857.922241f, 2240.126709f, 1808.689331f, 2220.164307f, -394.496552f, -409.135559f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1857.111328f, 2260.264648f, 1811.781860f, 2240.092285f, -379.435883f, -394.500000f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1858.618408f, 2291.224365f, 1817.336060f, 2260.230225f, -369.218628f, -379.439331f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1916.068970f, 2880.231689f, 1787.057617f, 2291.189697f, -369.218628f, -369.222076f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1874.273926f, 2900.186035f, 1804.153076f, 2880.197021f, -363.107849f, -369.222076f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1867.172363f, 2910.433838f, 1808.774414f, 2900.151611f, -361.318573f, -363.111267f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1866.018433f, 2935.093750f, 1810.950928f, 2910.398926f, -353.134796f, -361.321991f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1867.218750f, 2955.049805f, 1813.390381f, 2935.059082f, -351.187317f, -353.138245f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1873.639160f, 3119.970459f, 1796.048218f, 2955.015381f, -351.191132f, -351.190765f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1875.894531f, 3471.045654f, 1794.684692f, 3119.936279f, -351.191681f, -351.194550f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1977.243164f, 3670.686768f, 1810.481323f, 3471.011230f, -351.187012f, -351.195129f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 2150.663818f, 3870.640869f, 1929.226196f, 3670.652588f, -351.088806f, -351.190491f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 2360.020996f, 4401.899414f, 2128.447021f, 3870.606445f, -351.216766f, -351.092255f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 2376.281738f, 4449.630371f, 2302.659668f, 4401.864746f, -351.218506f, -351.220215f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 2330.541504f, 4487.497559f, 2088.791748f, 4353.533691f, -351.127686f, -351.227295f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1970.869263f, 4546.419922f, 1718.944824f, 4328.158203f, -342.350067f, -342.350067f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 2088.826172f, 4546.419922f, 1970.869263f, 4328.158203f, -342.350067f, -351.131134f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);          
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1718.979370f, 4521.872070f, 1693.714355f, 4390.941406f, -335.495331f, -342.353546f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1693.748901f, 4528.749023f, 1668.224609f, 4403.584961f, -323.133270f, -335.498779f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1668.259155f, 4523.508789f, 1643.855469f, 4398.695801f, -303.650909f, -323.136719f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1643.890137f, 4526.929199f, 1592.751099f, 4394.583496f, -282.548676f, -303.654358f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1592.785645f, 4517.142090f, 1569.628906f, 4376.719238f, -276.308044f, -282.552124f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1569.663330f, 4510.000000f, 1519.832764f, 4376.765625f, -276.124390f, -276.311493f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1519.867310f, 4511.801270f, 1494.917725f, 4380.252441f, -265.352936f, -276.127838f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1494.952271f, 4524.722656f, 1469.364990f, 4391.897461f, -246.669357f, -265.356384f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1469.399536f, 4527.876953f, 1418.633545f, 4403.879883f, -222.256180f, -246.672806f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1418.668213f, 4532.038086f, 1367.153076f, 4400.397461f, -195.617920f, -222.259613f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1367.187622f, 4530.801270f, 1346.300049f, 4390.177734f, -188.730774f, -195.621353f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "t75_spwt01b", 1346.334595f, 4858.749512f, 758.170227f, 4232.570312f, -187.999893f, -188.734238f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);

            AddDiscardGeometryBox(-2683.587891f, 751.149780f, 912.378296f, -2961.855957f, 463.150513f, 446.286072f); // Floating tree in the south
            AddDiscardGeometryBox(-154.989319f, 3470.648926f, 618.667847f, -356.593872f, 3271.615479f, 414.874786f); // Floating plant in the west (south-ish)
        }
    }
}
