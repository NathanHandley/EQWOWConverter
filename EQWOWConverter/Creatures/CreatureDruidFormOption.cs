//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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

namespace EQWOWConverter.Creatures
{
    internal enum CreatureDruidFormType
    {
        Bear = 1,   // Covers both Bear Form and Dire Bear Form
        Cat = 2,
        Travel = 3,
        Tree = 4,   // Tree of Life
        Moonkin = 5
    }

    internal class CreatureDruidFormOption
    {
        public CreatureDruidFormType FormType;
        public int OptionID;
        public int WOWDisplayID = 0;
        public int CreatureTemplateID = 0;

        private CreatureDruidFormOption(CreatureDruidFormType formType, int optionID, int wowDisplayID, int creatureTemplateID)
        {
            FormType = formType;
            OptionID = optionID;
            WOWDisplayID = wowDisplayID;
            CreatureTemplateID = creatureTemplateID;
        }

        public static List<CreatureDruidFormOption> GetOptions()
        {
            List<CreatureDruidFormOption> options = new List<CreatureDruidFormOption>();
            options.Add(new CreatureDruidFormOption(CreatureDruidFormType.Bear, 1, Configuration.DBCID_CREATUREDISPLAYINFO_DRUID_BEAR_ALLIANCE_ID, 0));
            options.Add(new CreatureDruidFormOption(CreatureDruidFormType.Bear, 2, Configuration.DBCID_CREATUREDISPLAYINFO_DRUID_BEAR_HORDE_ID, 0));
            options.Add(new CreatureDruidFormOption(CreatureDruidFormType.Bear, 3, 0, Configuration.GENERATE_DRUID_FORM_NORRATH_GRIZZLY_CREATURE_TEMPLATE_ID));
            options.Add(new CreatureDruidFormOption(CreatureDruidFormType.Cat, 1, Configuration.DBCID_CREATUREDISPLAYINFO_DRUID_CAT_ALLIANCE_ID, 0));
            options.Add(new CreatureDruidFormOption(CreatureDruidFormType.Cat, 2, Configuration.DBCID_CREATUREDISPLAYINFO_DRUID_CAT_HORDE_ID, 0));
            options.Add(new CreatureDruidFormOption(CreatureDruidFormType.Cat, 3, 0, Configuration.GENERATE_DRUID_FORM_NORRATH_PANTHER_CREATURE_TEMPLATE_ID));
            options.Add(new CreatureDruidFormOption(CreatureDruidFormType.Cat, 4, 0, Configuration.GENERATE_DRUID_FORM_NORRATH_SABERTOOTH_CREATURE_TEMPLATE_ID));
            options.Add(new CreatureDruidFormOption(CreatureDruidFormType.Travel, 1, 0, Configuration.GENERATE_DRUID_FORM_NORRATH_LEOPARD_CREATURE_TEMPLATE_ID));
            options.Add(new CreatureDruidFormOption(CreatureDruidFormType.Tree, 1, 0, Configuration.GENERATE_DRUID_FORM_NORRATH_TREANT_CREATURE_TEMPLATE_ID));
            return options;
        }

        public static HashSet<int> GetCreatureTemplateIDs()
        {
            HashSet<int> creatureTemplateIDs = new HashSet<int>();
            foreach (CreatureDruidFormOption option in GetOptions())
                if (option.CreatureTemplateID != 0)
                    creatureTemplateIDs.Add(option.CreatureTemplateID);
            return creatureTemplateIDs;
        }
    }
}
