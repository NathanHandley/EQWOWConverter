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
    internal class CreatureIllusionDisplayRow
    {
        public int FormSpellID;
        public int BodySet;
        public int TintID;
        public int HelmOn;
        public int DisplayID;

        public CreatureIllusionDisplayRow(int formSpellID, int bodySet, int tintID, int helmOn, int displayID)
        {
            FormSpellID = formSpellID;
            BodySet = bodySet;
            TintID = tintID;
            HelmOn = helmOn;
            DisplayID = displayID;
        }
    }

    internal static class CreatureIllusionVersionRegistry
    {
        private class IllusionFormRecord
        {
            public int FormSpellID;
            public CreatureRace Race;
            public CreatureGenderType Gender;
            public float ModelTemplateScale;

            public IllusionFormRecord(int formSpellID, CreatureRace race, CreatureGenderType gender, float modelTemplateScale)
            {
                FormSpellID = formSpellID;
                Race = race;
                Gender = gender;
                ModelTemplateScale = modelTemplateScale;
            }
        }

        private static readonly object RegistryLock = new object();
        private static List<IllusionFormRecord> FormRecords = new List<IllusionFormRecord>();
        private static List<CreatureIllusionDisplayRow> DisplayRows = new List<CreatureIllusionDisplayRow>();

        public static bool IsRobeCapableEQRaceID(int eqRaceID)
        {
            if (eqRaceID == 1 || eqRaceID == 3 || eqRaceID == 5 || eqRaceID == 6 || eqRaceID == 12 || eqRaceID == 128)
                return true;
            return false;
        }

        public static void RegisterFormSpell(int formSpellWOWSpellID, CreatureRace race, CreatureGenderType gender, float modelTemplateScale)
        {
            lock (RegistryLock)
            {
                FormRecords.Add(new IllusionFormRecord(formSpellWOWSpellID, race, gender, modelTemplateScale));
            }
        }

        public static void CreateModelTemplatesForRegisteredForms()
        {
            lock (RegistryLock)
            {
                DisplayRows.Clear();
                foreach (IllusionFormRecord formRecord in FormRecords)
                {
                    foreach (CreatureIllusionChestAppearance chestAppearance in CreatureIllusionTintPalette.GetChestAppearances())
                    {
                        // Robe geometry only exists for the robe-capable races
                        if (chestAppearance.BodySet >= 10 && IsRobeCapableEQRaceID(formRecord.Race.ID) == false)
                            continue;
                        int colorTintID = CreatureIllusionTintPalette.GetColorTintIDForTintID(chestAppearance.TintID);

                        // Helm-off version (bare head)
                        // FaceIndex 99 marks an illusion variant with replaceable face textures (real faces are only 0-9), which keeps these templates from sharing M2s with NPC templates that have the same (race, gender, texture, tint, scale) key
                        CreatureModelTemplate helmOffTemplate = CreatureModelTemplate.GetOrCreateCreatureModelTemplate(formRecord.Race,
                            formRecord.Gender, 0, chestAppearance.BodySet, CreatureModelTemplate.ILLUSION_REPLACEABLE_FACE_INDEX,
                            colorTintID, formRecord.ModelTemplateScale, false, false);
                        DisplayRows.Add(new CreatureIllusionDisplayRow(formRecord.FormSpellID, chestAppearance.BodySet, chestAppearance.TintID,
                            0, helmOffTemplate.DBCCreatureDisplayID));

                        // Helm-on version.  Body sets 1-3 use the matching helm mesh (races without that helm mesh fall back to a bare head), while set 0 and the robe sets have no helm mesh so they reuse the helm-off display
                        if (chestAppearance.BodySet >= 1 && chestAppearance.BodySet <= 3)
                        {
                            CreatureModelTemplate helmOnTemplate = CreatureModelTemplate.GetOrCreateCreatureModelTemplate(formRecord.Race,
                                formRecord.Gender, chestAppearance.BodySet, chestAppearance.BodySet, CreatureModelTemplate.ILLUSION_REPLACEABLE_FACE_INDEX,
                                colorTintID, formRecord.ModelTemplateScale, false, false);
                            DisplayRows.Add(new CreatureIllusionDisplayRow(formRecord.FormSpellID, chestAppearance.BodySet, chestAppearance.TintID,
                                1, helmOnTemplate.DBCCreatureDisplayID));
                        }
                        else
                        {
                            DisplayRows.Add(new CreatureIllusionDisplayRow(formRecord.FormSpellID, chestAppearance.BodySet, chestAppearance.TintID,
                                1, helmOffTemplate.DBCCreatureDisplayID));
                        }
                    }
                }
            }
        }

        public static List<CreatureIllusionDisplayRow> GetDisplayRows()
        {
            lock (RegistryLock)
            {
                return new List<CreatureIllusionDisplayRow>(DisplayRows);
            }
        }
    }
}
