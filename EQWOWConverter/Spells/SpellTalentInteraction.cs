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

namespace EQWOWConverter.Spells
{
    internal class SpellTalentInteraction
    {
        private static readonly object SpellTalentInteractionLock = new object();
        private static List<SpellTalentInteraction> SpellTalentInteractions = new List<SpellTalentInteraction>();

        public int SpellID = 0;
        public string WOWClassName = string.Empty;
        public string SpellName = string.Empty;
        public SpellTalentInteractionType InteractionType = SpellTalentInteractionType.Exclude;
        public int EffectIndex = 0;
        public int SchoolMask = 0;
        public bool AffectsDamage = false;
        public bool AffectsHealing = false;
        public SpellTalentAlignmentRestriction Restriction = SpellTalentAlignmentRestriction.None;
        public int MaxBaseCastTimeInMS = 0;
        public string DescriptionOrReason = string.Empty;

        public static List<SpellTalentInteraction> GetSpellTalentInteractions()
        {
            lock (SpellTalentInteractionLock)
            {
                if (SpellTalentInteractions.Count == 0)
                    PopulateSpellTalentInteractions();
                return SpellTalentInteractions;
            }
        }

        private static void PopulateSpellTalentInteractions()
        {
            string interactionsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpellTalentInteractions.csv");
            Logger.WriteDebug(string.Concat("Populating spell talent interactions via file '", interactionsFile, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(interactionsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                SpellTalentInteraction newInteraction = new SpellTalentInteraction();
                if (int.TryParse(columns["spell_id"].Trim(), out newInteraction.SpellID) == false || newInteraction.SpellID <= 0)
                {
                    Logger.WriteError("SpellTalentInteractions row had an invalid spell_id of '", columns["spell_id"], "'");
                    continue;
                }
                newInteraction.WOWClassName = columns["wow_class"].Trim();
                newInteraction.SpellName = columns["spell_name"].Trim();
                newInteraction.DescriptionOrReason = RemoveDoubleQuotesOnEnds(columns["description_or_reason"].Trim());
                string interactionText = columns["interaction"].Trim().ToLower();
                switch (interactionText)
                {
                    case "align": newInteraction.InteractionType = SpellTalentInteractionType.Align; break;
                    case "describe": newInteraction.InteractionType = SpellTalentInteractionType.Describe; break;
                    case "exclude": newInteraction.InteractionType = SpellTalentInteractionType.Exclude; break;
                    default:
                    {
                        Logger.WriteError("SpellTalentInteractions row for spell_id '", newInteraction.SpellID.ToString(), "' had an invalid interaction of '", interactionText, "'");
                        continue;
                    }
                }
                if (newInteraction.InteractionType == SpellTalentInteractionType.Describe && newInteraction.DescriptionOrReason.Length == 0)
                {
                    Logger.WriteError("SpellTalentInteractions describe row for spell_id '", newInteraction.SpellID.ToString(), "' has no description");
                    continue;
                }
                if (newInteraction.InteractionType != SpellTalentInteractionType.Align)
                {
                    SpellTalentInteractions.Add(newInteraction);
                    continue;
                }

                // Everything below only carries meaning for align rows
                if (int.TryParse(columns["effect_index"].Trim(), out newInteraction.EffectIndex) == false
                    || newInteraction.EffectIndex < 0 || newInteraction.EffectIndex > 2)
                {
                    Logger.WriteError("SpellTalentInteractions row for spell_id '", newInteraction.SpellID.ToString(), "' had an invalid effect_index of '", columns["effect_index"], "'");
                    continue;
                }
                if (int.TryParse(columns["school_mask"].Trim(), out newInteraction.SchoolMask) == false || newInteraction.SchoolMask < 0)
                {
                    Logger.WriteError("SpellTalentInteractions row for spell_id '", newInteraction.SpellID.ToString(), "' had an invalid school_mask of '", columns["school_mask"], "'");
                    continue;
                }
                newInteraction.AffectsDamage = columns["affects_damage"].Trim() == "1";
                newInteraction.AffectsHealing = columns["affects_healing"].Trim() == "1";
                string restrictionText = columns["restriction"].Trim().ToLower();
                switch (restrictionText)
                {
                    case "": case "none": newInteraction.Restriction = SpellTalentAlignmentRestriction.None; break;
                    case "direct_damage": newInteraction.Restriction = SpellTalentAlignmentRestriction.DirectDamage; break;
                    case "area_damage": newInteraction.Restriction = SpellTalentAlignmentRestriction.AreaDamage; break;
                    case "pbaoe_damage": newInteraction.Restriction = SpellTalentAlignmentRestriction.PointBlankAreaDamage; break;
                    case "direct_heal": newInteraction.Restriction = SpellTalentAlignmentRestriction.DirectHeal; break;
                    case "single_target_heal": newInteraction.Restriction = SpellTalentAlignmentRestriction.SingleTargetDirectHeal; break;
                    case "area_heal": newInteraction.Restriction = SpellTalentAlignmentRestriction.AreaHeal; break;
                    case "periodic_heal": newInteraction.Restriction = SpellTalentAlignmentRestriction.PeriodicHeal; break;
                    case "periodic_damage": newInteraction.Restriction = SpellTalentAlignmentRestriction.PeriodicDamage; break;
                    case "cure": newInteraction.Restriction = SpellTalentAlignmentRestriction.Cure; break;
                    case "strength_debuff": newInteraction.Restriction = SpellTalentAlignmentRestriction.StrengthDebuff; break;
                    case "pet_summon": newInteraction.Restriction = SpellTalentAlignmentRestriction.PetSummon; break;
                    default:
                    {
                        Logger.WriteError("SpellTalentInteractions row for spell_id '", newInteraction.SpellID.ToString(), "' had an invalid restriction of '", restrictionText, "'");
                        continue;
                    }
                }

                // Cures, strength debuffs and pet summons are neither damage nor healing, so only they may leave both flags off
                bool restrictionCarriesItsOwnReach = newInteraction.Restriction == SpellTalentAlignmentRestriction.Cure || newInteraction.Restriction == SpellTalentAlignmentRestriction.StrengthDebuff
                    || newInteraction.Restriction == SpellTalentAlignmentRestriction.PetSummon;
                if (restrictionCarriesItsOwnReach == false && newInteraction.AffectsDamage == false && newInteraction.AffectsHealing == false)
                {
                    Logger.WriteError("SpellTalentInteractions align row for spell_id '", newInteraction.SpellID.ToString(), "' affects neither damage nor healing");
                    continue;
                }
                if (newInteraction.AffectsDamage == true && newInteraction.SchoolMask == 0)
                {
                    Logger.WriteError("SpellTalentInteractions align row for spell_id '", newInteraction.SpellID.ToString(), "' affects damage but has no school_mask to align on");
                    continue;
                }
                if (int.TryParse(columns["max_base_cast_time_ms"].Trim(), out newInteraction.MaxBaseCastTimeInMS) == false || newInteraction.MaxBaseCastTimeInMS < 0)
                {
                    Logger.WriteError("SpellTalentInteractions row for spell_id '", newInteraction.SpellID.ToString(), "' had an invalid max_base_cast_time_ms of '", columns["max_base_cast_time_ms"], "'");
                    continue;
                }
                SpellTalentInteractions.Add(newInteraction);
            }
            Logger.WriteDebug("Finished load spell talent interactions.");
        }

        private static string RemoveDoubleQuotesOnEnds(string rawText)
        {
            if (rawText.Length < 2 || rawText.StartsWith("\"") == false || rawText.EndsWith("\"") == false)
                return rawText;
            return rawText.Substring(1, rawText.Length - 2).Replace("\"\"", "\"");
        }
    }
}
