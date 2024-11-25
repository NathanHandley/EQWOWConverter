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
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Creatures
{
    internal class CreatureModelVariation
    {
        public CreatureGenderType GenderType = CreatureGenderType.Neutral;
        public int BodyModelIndex = 0;
        public int HeadModelIndex = 0;
        public int BodyTextureIndex = 0;
        public int HeadTextureIndex = 0;
        public string ModelFileName = string.Empty;

        // IDs
        private static int CURRENT_DBCID_CREATUREMODELDATAID = Configuration.CONFIG_DBCID_CREATUREMODELDATA_ID_START;
        private static int CURRENT_DBCID_CREATUREDISPLAYINFOID = Configuration.CONFIG_DBCID_CREATUREDISPLAYINFO_ID_START;
        private static int CURRENT_DBCID_CREATURESOUNDDATAID = Configuration.CONFIG_DBCID_CREATURESOUNDDATA_ID_START;
        private static int CURRENT_SQL_CREATURETEMPLATEID = Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_LOW;
        
        public int DBCCreatureModelDataID;
        public int DBCCreatureDisplayID;
        public int DBCCreatureSoundDataID;
        public int SQLCreatureTemplateID;

        public CreatureModelVariation()
        {
            DBCCreatureModelDataID = CURRENT_DBCID_CREATUREMODELDATAID;
            CURRENT_DBCID_CREATUREMODELDATAID++;
            DBCCreatureDisplayID = CURRENT_DBCID_CREATUREDISPLAYINFOID;
            CURRENT_DBCID_CREATUREDISPLAYINFOID++;
            DBCCreatureSoundDataID = CURRENT_DBCID_CREATURESOUNDDATAID;
            CURRENT_DBCID_CREATURESOUNDDATAID++;
            SQLCreatureTemplateID = CURRENT_SQL_CREATURETEMPLATEID;
            CURRENT_SQL_CREATURETEMPLATEID++;
        }
    }
}
