//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones.Properties
{
    internal class BefallenZoneProperties : ZoneProperties
    {
        public BefallenZoneProperties() : base()
        {
            // Cave Sound - Zone Wide
            SetBaseZoneProperties("befallen", "Befallen", 35.22f, -75.27f, 2.19f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsIndoors(0, 0, 60, ZoneFogType.Heavy, 130, 130, 130);
            SetZonewideAmbienceSound("caveloop", "caveloop", 0.1f, 0.1f);
            OverrideVertexColorIntensity(0.6);

            AddZoneArea("Entry", "befallen-06", "befallen-06");
            AddZoneAreaBox("Entry", 54.481789f, 55.857861f, 37.370251f, -124.496246f, -144.035538f, -5.204370f);
            AddZoneAreaBox("Entry", 26.557690f, 11.077470f, 20.215931f, -203.251007f, -90.467781f, -4.588550f);
            
            AddZoneArea("Well", "befallen-00", "befallen-00");
            AddZoneAreaBox("Well", -262.989471f, 98.996452f, 7.501510f, -346.584534f, 69.684769f, -16.758711f);
            AddZoneAreaBox("Well", -337.689789f, 116.707527f, 7.654700f, -406.665894f, 54.447369f, -15.360780f);
            AddZoneAreaBox("Well", -346.760834f, 116.179741f, -50.434662f, -411.030212f, 49.695049f, -76.359451f);
            
            AddZoneArea("Bottom South", "befallen-07", "befallen-07");
            AddZoneAreaBox("Bottom South", -785.887939f, 193.538437f, -28.172581f, -1011.797668f, -113.528923f, -76.708099f);
            AddZoneAreaBox("Bottom South", -791.871948f, -177.910599f, -28.172581f, -1011.797668f, -248.485245f, -76.708099f);
            AddZoneAreaBox("Bottom South", -804.666870f, 193.538437f, -28.172581f, -1011.797668f, -248.485245f, -76.708099f);
            AddZoneAreaBox("Bottom South", -782.485229f, -111.742867f, -28.172581f, -860.202637f, -176.478546f, -70.821602f);
            AddZoneAreaBox("Bottom South", -782.741333f, -14.617890f, -28.172581f, -800.851929f, -111.868011f, -76.708099f);
            
            AddZoneArea("Middle West", "befallen-01", "befallen-01");
            AddZoneAreaBox("Middle West", -584.371948f, -26.771641f, -41.967979f, -598.573914f, -45.613731f, -47.362438f);
            
            AddZoneArea("Bottom Center", "befallen-04", "befallen-04");
            AddZoneAreaBox("Bottom Center", -629.656189f, 160.041992f, -23.937210f, -806.572266f, -202.661743f, -89.750160f);
            AddZoneAreaBox("Bottom Center", -531.246948f, -12.537450f, -42.361931f, -634.891602f, -71.659630f, -83.953720f);
            AddZoneAreaBox("Bottom Center", -514.774353f, -147.121918f, -42.216499f, -645.313721f, -209.536377f, -81.840134f);
            
            AddZoneArea("Bottom North", "befallen-05", "befallen-05");
            AddZoneAreaBox("Bottom North", -291.817841f, 133.591080f, -50.768059f, -629.886292f, -214.062393f, -92.168129f);
            
            AddZoneArea("Middle West", "befallen-01", "befallen-01");
            AddZoneAreaBox("Middle West", -357.848206f, 199.227127f, -23.644470f, -605.808105f, -13.567110f, -45.856098f);
            AddZoneAreaBox("Middle West", -465.827972f, 199.227127f, -23.644470f, -605.808105f, -74.008232f, -45.856098f);
            
            AddZoneArea("Middle East", "befallen-03", "befallen-03");
            AddZoneAreaBox("Middle East", -318.861420f, -12.284030f, -23.644470f, -560.344482f, -304.230774f, -45.856098f);
            AddZoneAreaBox("Middle East", -284.659149f, -253.402206f, -15.765840f, -414.804291f, -300f, -41.967731f);
            
            AddZoneArea("Top", "befallen-02", "befallen-02");
            AddZoneAreaBox("Top", 50.491291f, 281.136414f, 31.637880f, -410.858093f, -301.009155f, -28.019581f);
            
            AddZoneLineBox("commons", -1155.6317f, 596.3344f, -42.280308f, ZoneLineOrientationType.North, -49.9417f, 42.162197f, 12.469f, -63.428658f, 27.86666f, -0.5000006f);
        }
    }
}
