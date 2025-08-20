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

using EQWOWConverter.Common;
using EQWOWConverter.Items;
using EQWOWConverter.Tradeskills;
using EQWOWConverter.WOWFiles;
using System.Text;

namespace EQWOWConverter.Spells
{
    internal class SpellTemplate
    {
        internal class SpellLearnScrollProperties
        {
            public int LearnLevel = -1;
            public int WOWItemTemplateID = -1;
        }

        public static Dictionary<int, int> SpellCastTimeDBCIDsByCastTime = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellRangeDBCIDsBySpellRange = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellRadiusDBCIDsBySpellRadius = new Dictionary<int, int>();
        public static Dictionary<int, int> SpellGroupStackRuleByGroup = new Dictionary<int, int>();

        private static Dictionary<int, SpellTemplate> SpellTemplatesByEQID = new Dictionary<int, SpellTemplate>();
        private static readonly object SpellTemplateLock = new object();       

        public class Reagent
        {
            public int WOWItemTemplateEntryID;
            public int Count;

            public Reagent(int wowItemTemplateEntryID, int count)
            {
                WOWItemTemplateEntryID = wowItemTemplateEntryID;
                Count = count;
            }
        }

        public int WOWSpellID = 0;
        public int WOWSpellIDWorn = -1;
        public int EQSpellID = -1;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public string AuraDescription = string.Empty;
        public UInt32 Category = 1;
        public UInt32 InterruptFlags = 15;
        public int SpellIconID = 0;
        public TradeskillRecipe? TradeskillRecipe = null;
        protected int _SpellCastTimeDBCID = 1; // First row, instant cast
        public int SpellCastTimeDBCID { get { return _SpellCastTimeDBCID; } }
        protected int _CastTimeInMS = 0;
        public int CastTimeInMS
        {
            get { return _CastTimeInMS; }
            set
            {
                if (SpellCastTimeDBCIDsByCastTime.ContainsKey(value) == false)
                    SpellCastTimeDBCIDsByCastTime.Add(value, SpellCastTimesDBC.GenerateDBCID());
                _SpellCastTimeDBCID = SpellCastTimeDBCIDsByCastTime[value];
            }
        }
        protected int _SpellRangeDBCID = 1; // First row, self only (no range)
        public int SpellRangeDBCID { get { return _SpellRangeDBCID; } }
        protected int _SpellRange = 0;
        public int SpellRange
        {
            get { return _SpellRange; }
            set
            {
                if (SpellRangeDBCIDsBySpellRange.ContainsKey(value) == false)
                    SpellRangeDBCIDsBySpellRange.Add(value, SpellRangeDBC.GenerateDBCID());
                _SpellRangeDBCID = SpellRangeDBCIDsBySpellRange[value];
                _SpellRange = value;
            }
        }
        protected int _SpellRadiusDBCID = 0;
        public int SpellRadiusDBCID { get { return _SpellRadiusDBCID; } }
        protected int _SpellRadius = 0;
        public int SpellRadius
        {
            get { return _SpellRadius; }
            set
            {
                if (SpellRadiusDBCIDsBySpellRadius.ContainsKey(value) == false)
                    SpellRadiusDBCIDsBySpellRadius.Add(value, SpellRadiusDBC.GenerateDBCID());
                _SpellRadiusDBCID = SpellRadiusDBCIDsBySpellRadius[value];
                _SpellRadius = value;
            }
        }
        public SpellDuration AuraDuration = new SpellDuration();
        public int SpellGroupStackingID = -1;
        public int SpellGroupStackingRule = 0;
        public UInt32 RecoveryTimeInMS = 0; // Note that this may be zero for a player but not a creature.  See Configuration.SPELL_RECOVERY_TIME_MINIMUM_IN_MS
        public UInt32 SpellVisualID1 = 0;
        public UInt32 SpellVisualID2 = 0;
        public bool PlayerLearnableByClassTrainer = false; // Needed?
        public int MinimumPlayerLearnLevel = -1;
        public bool HasEffectBaseFormulaUsingSpellLevel = false;
        public int RequiredAreaIDs = -1;
        public UInt32 SchoolMask = 1;
        public UInt32 DispelType = 0;
        public UInt32 RequiredTotemID1 = 0;
        public UInt32 RequiredTotemID2 = 0;
        public UInt32 SpellFocusID = 0;
        public bool AllowCastInCombat = true;
        public List<Reagent> Reagents = new List<Reagent>();
        public int SkillLine = 0;
        public List<SpellEffectEQ> EQSpellEffects = new List<SpellEffectEQ>();
        public List<SpellEffectWOW> WOWSpellEffects = new List<SpellEffectWOW>();
        public UInt32 ManaCost = 0;
        public SpellEQTargetType EQTargetType = SpellEQTargetType.Single;
        public UInt32 TargetCreatureType = 0; // No specific creature type
        public bool CastOnCorpse = false;
        public Dictionary<ClassType, SpellLearnScrollProperties> LearnScrollPropertiesByClassType = new Dictionary<ClassType, SpellLearnScrollProperties>();
        public int HighestDirectHealAmountInSpellEffect = 0; // Used in spell priority calculations
        private string TargetDescriptionTextFragment = string.Empty;
        public bool BreakEffectOnNonAutoDirectDamage = false;
        public bool NoPartialImmunity = false;
        public UInt32 DefenseType = 0; // 0 None, 1 Magic, 2 Melee, 3 Ranged
        public UInt32 PreventionType = 0; // 0 None, 1 Silence, 2 Pacify, 4 No Actions
        public int WeaponSpellItemEnchantmentDBCID = 0;
        public int WeaponItemEnchantProcSpellID = 0;
        public string WeaponItemEnchantSpellName = string.Empty;
        public UInt32 ProcChance = 101;

        public static Dictionary<int, SpellTemplate> GetSpellTemplatesByEQID()
        {
            lock (SpellTemplateLock)
            {
                if (SpellTemplatesByEQID.Count == 0)
                {
                    Logger.WriteError("GetSPellTemplatesByEQID called before LoadSpellTemplates");
                    return new Dictionary<int, SpellTemplate>();
                }
                return SpellTemplatesByEQID;
            }
        }

        public static void LoadSpellTemplates(SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID)
        {
            // Load the spell templates
            string spellTemplatesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpellTemplates.csv");
            Logger.WriteDebug(string.Concat("Loading spell templates via file '", spellTemplatesFile, "'"));
            List<Dictionary<string, string>> spellTemplateRows = FileTool.ReadAllRowsFromFileWithHeader(spellTemplatesFile, "|");
            foreach (Dictionary<string, string> columns in spellTemplateRows)
            {
                // Skip invalid
                if (columns["enabled"] == "0")
                    continue;

                // Load the row
                SpellTemplate newSpellTemplate = new SpellTemplate();
                newSpellTemplate.EQSpellID = int.Parse(columns["eq_id"]);
                newSpellTemplate.WOWSpellID = int.Parse(columns["wow_id"]);
                newSpellTemplate.WOWSpellIDWorn = int.Parse(columns["wow_worn_id"]);
                newSpellTemplate.Name = columns["name"];
                newSpellTemplate.SpellRange = Convert.ToInt32(float.Parse(columns["range"]) * Configuration.SPELLS_RANGE_MULTIPLIER);
                newSpellTemplate.SpellRadius = Convert.ToInt32(float.Parse(columns["aoerange"]) * Configuration.SPELLS_RANGE_MULTIPLIER);
                newSpellTemplate.RecoveryTimeInMS = UInt32.Parse(columns["cast_recovery_time"]);
                newSpellTemplate.Category = 0; // Temp / TODO: Figure out how/what to set here
                newSpellTemplate.CastTimeInMS = int.Parse(columns["cast_time"]);
                // TODO: FacingCasterFlags
                for (int i = 1; i <= 12; i++)
                    PopulateEQSpellEffect(ref newSpellTemplate, i, columns);
                // Skip if there isn't an effect
                if (newSpellTemplate.EQSpellEffects.Count == 0)
                    continue;
                PopulateAllClassLearnScrollProperties(ref newSpellTemplate, columns);
                newSpellTemplate.ManaCost = Convert.ToUInt32(columns["mana"]);

                // Buff duration (if any)
                int buffDurationInTicks = Convert.ToInt32(columns["buffduration"]);
                int buffDurationFormula = Convert.ToInt32(columns["buffdurationformula"]);
                if (buffDurationFormula != 0)
                    newSpellTemplate.AuraDuration.CalculateAndSetAuraDuration(newSpellTemplate.MinimumPlayerLearnLevel, buffDurationFormula, buffDurationInTicks);

                // Icon
                int spellIconID = int.Parse(columns["icon"]);
                if (spellIconID >= 2500)
                    newSpellTemplate.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(spellIconID - 2500);

                // Targets
                int eqTargetTypeID = int.Parse(columns["targettype"]);
                bool isDetrimental = int.Parse(columns["goodEffect"]) == 0 ? true : false; // "2" should be non-detrimental group only (not caster).  Ignoring that for now.
                List<SpellWOWTargetType> targets = CalculateTargets(ref newSpellTemplate, eqTargetTypeID, isDetrimental, newSpellTemplate.EQSpellEffects, newSpellTemplate.SpellRange, newSpellTemplate.SpellRadius);

                // Visual
                int spellVisualEffectIndex = int.Parse(columns["SpellVisualEffectIndex"]);
                if (spellVisualEffectIndex >= 0 && spellVisualEffectIndex < 52)
                    newSpellTemplate.SpellVisualID1 = Convert.ToUInt32(SpellVisual.GetSpellVisual(spellVisualEffectIndex, !isDetrimental).SpellVisualDBCID);

                // School class
                int resistType = int.Parse(columns["resisttype"]);
                newSpellTemplate.SchoolMask = GetSchoolMaskForResistType(resistType);
                newSpellTemplate.DispelType = GetDispelTypeForResistType(resistType, isDetrimental, newSpellTemplate.AuraDuration.MaxDurationInMS > 0);

                // Set defensive properties
                newSpellTemplate.DefenseType = 1; // Magic
                newSpellTemplate.PreventionType = 1; // Silence

                // Convert the spell effects
                ConvertEQSpellEffectsIntoWOWEffects(ref newSpellTemplate, newSpellTemplate.SchoolMask, newSpellTemplate.AuraDuration.MaxDurationInMS > 0, 
                    newSpellTemplate.CastTimeInMS, targets, newSpellTemplate.SpellRadiusDBCID, itemTemplatesByEQDBID);

                // If there is no wow effect, skip it
                if (newSpellTemplate.WOWSpellEffects.Count == 0)
                    continue;

                // Set the spell and aura descriptions
                SetMainAndAuraDescriptions(ref newSpellTemplate);

                // Stacking rules
                SetAuraStackRule(ref newSpellTemplate, int.Parse(columns["spell_category"]));

                // Add it
                SpellTemplatesByEQID.Add(newSpellTemplate.EQSpellID, newSpellTemplate);
            }
        }        

        public static void GenerateItemEnchantSpellIfNotCreated(string itemName, int procSpellEQID, int enchantSpellWOWID, out SpellTemplate? enchantSpellTemplate)
        {
            lock (SpellTemplateLock)
            {
                enchantSpellTemplate = null;

                // Don't do anything if it already exists
                foreach (SpellTemplate spellTemplate in SpellTemplatesByEQID.Values)
                {
                    if (spellTemplate.WOWSpellID == enchantSpellWOWID)
                        return;
                }
                if (SpellTemplatesByEQID.ContainsKey(procSpellEQID) == false)
                {
                    Logger.WriteDebug("Failed to create an item enchantement since triggered eq spell ID of ", procSpellEQID.ToString() ," did not exist");
                    return;
                }
                SpellTemplate procSpellTemplate = SpellTemplatesByEQID[procSpellEQID];

                // Generate a description
                StringBuilder descriptionSB = new StringBuilder();
                descriptionSB.Append("Coats a weapon with poison that lasts for ");
                descriptionSB.Append(GetTimeTextFromSeconds(Configuration.SPELL_ENCHANT_ROGUE_POISON_ENCHANT_DURATION_ON_WEAPON_TIME_IN_SECONDS));
                descriptionSB.Append(" with each strike having a ");
                descriptionSB.Append(Configuration.SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE);
                descriptionSB.Append("% chance of applying the following: ");
                descriptionSB.Append(procSpellTemplate.Description);

                // Generate an enchant ID
                int enchantID = SpellItemEnchantmentDBC.GenerateUniqueID();

                // Create the new spell
                SpellTemplate enchantSpell = new SpellTemplate();
                enchantSpell.Name = itemName;
                enchantSpell.WOWSpellID = enchantSpellWOWID;
                enchantSpell.Description = descriptionSB.ToString();
                enchantSpell.WeaponSpellItemEnchantmentDBCID = enchantID;
                enchantSpell.WeaponItemEnchantProcSpellID = procSpellTemplate.WOWSpellID;
                enchantSpell.WeaponItemEnchantSpellName = itemName;
                enchantSpell.WOWSpellEffects.Add(new SpellEffectWOW(SpellWOWEffectType.EnchantItemTemporary, 0, 0, 0, 1, 0, enchantID, 0));
                enchantSpell.ProcChance = Convert.ToUInt32(Configuration.SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE);
                enchantSpell.SpellIconID = SpellIconDBC.GetDBCIDForSpellIconID(procSpellTemplate.SpellIconID);
                enchantSpell.SpellVisualID1 = Convert.ToUInt32(Configuration.SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_APPLYING_VISUAL_ID);
                enchantSpell.CastTimeInMS = Configuration.SPELL_ENCHANT_ROGUE_POISON_APPLY_TIME_IN_MS;

                enchantSpellTemplate = enchantSpell;
            }
        }

        private static void PopulateAllClassLearnScrollProperties(ref SpellTemplate spellTemplate, Dictionary<string, string> rowColumns)
        {
            // In EQ, a scroll can be learned by multiple classes at different levels
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Warrior, "warrior", "bard");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Paladin, "paladin");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Hunter, "ranger", "beastlord");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Rogue, "monk", "rogue");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Priest, "cleric");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.DeathKnight, "shadowknight");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Shaman, "shaman");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Mage, "magician", "wizard");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Warlock, "necromancer", "enchanter");
            PopulateClassLearnScrollProperties(ref spellTemplate, rowColumns, ClassType.Druid, "druid");
        }

        private static UInt32 GetSchoolMaskForResistType(int eqResistType)
        {
            switch (eqResistType)
            {
                case 1: return 64; // EQ Magic => WOW Arcane
                case 2: return 4; // EQ Fire => WOW Fire
                case 3: return 16; // EQ Cold => WOW Frost
                case 4: return 8; // EQ Poison => WOW Nature
                case 5: return 32; // EQ Disease => WOW Shadow
                default: return 1; // Physical by default
            }
        }

        public static UInt32 GetDispelTypeForResistType(int eqResistType, bool isDetrimental, bool hasSpellDuration)
        {
            // TODO: Honor 'nodispel'?
            if (hasSpellDuration == false)
                return 0;
            if (isDetrimental == false)
                return 1;
            switch (eqResistType)
            {
                case 1: return 1; // EQ Magic => MAGIC
                case 2: return 1; // EQ Fire => MAGIC
                case 3: return 1; // EQ Cold => MAGIC
                case 4: return 4; // EQ Poison => POISON
                case 5: return 3; // EQ Disease => DISEASE
                default: return 0; // None
            }
        }

        private static void PopulateClassLearnScrollProperties(ref SpellTemplate spellTemplate, Dictionary<string, string> rowColumns, ClassType wowClassType, params string[] eqClassNames)
        {
            SpellLearnScrollProperties spellLearnScrollProperties = new SpellLearnScrollProperties();
            
            // Use the lowest learn level id properties
            foreach(string className in eqClassNames)
            {
                int curClassLearnlevel = int.Parse(rowColumns[string.Concat(className, "_learn_level")]);
                if (curClassLearnlevel != -1 && curClassLearnlevel <= 100 && (curClassLearnlevel < spellLearnScrollProperties.LearnLevel || spellLearnScrollProperties.LearnLevel == -1))
                {
                    spellLearnScrollProperties.LearnLevel = curClassLearnlevel;
                    spellLearnScrollProperties.WOWItemTemplateID = int.Parse(rowColumns[string.Concat(className, "_learn_wowitemid")]);
                }
            }

            // Only save it if a valid one was found
            if (spellLearnScrollProperties.LearnLevel > -1)
            {
                spellTemplate.LearnScrollPropertiesByClassType[wowClassType] = spellLearnScrollProperties;

                // Also save it as the lowest level possible to learn for future formulas
                if (spellTemplate.MinimumPlayerLearnLevel <= 0 || spellTemplate.MinimumPlayerLearnLevel > spellLearnScrollProperties.LearnLevel)
                    spellTemplate.MinimumPlayerLearnLevel = spellLearnScrollProperties.LearnLevel;
            }
        }

        private static List<SpellWOWTargetType> CalculateTargets(ref SpellTemplate spellTemplate, int eqTargetTypeID, bool isDetrimental, List<SpellEffectEQ> eqSpellEffects, int range, int spellRadius)
        {
            List<SpellWOWTargetType> spellWOWTargetTypes = new List<SpellWOWTargetType>();

            // Capture the EQ Target type
            if (Enum.IsDefined(typeof(SpellEQTargetType), eqTargetTypeID) == false)
            {
                Logger.WriteError("SpellTemplate with EQID ", spellTemplate.EQSpellID.ToString(), " has unknown target type of ", eqTargetTypeID.ToString());
                spellWOWTargetTypes.Add(SpellWOWTargetType.Self);
                return spellWOWTargetTypes;
            }
            else
                spellTemplate.EQTargetType = (SpellEQTargetType)eqTargetTypeID;

            // Some spell effects allow targeting both friendly and enemy
            bool canTargetBothEnemyAndFriendly = false;
            foreach (SpellEffectEQ effect in eqSpellEffects)
            {
                if (effect.EQEffectType == SpellEQEffectType.CancelMagic)
                    canTargetBothEnemyAndFriendly = true;
            }

            // Map the EQ target type to WOW
            switch (spellTemplate.EQTargetType)
            {
                case SpellEQTargetType.LineOfSight:
                    {
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single visible enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetFriendly);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single visible friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.GroupV1:
                case SpellEQTargetType.GroupV2:
                    {
                        spellWOWTargetTypes.Add(SpellWOWTargetType.CasterParty);
                        if (spellRadius > 0)
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets the whole party within ", spellRadius, " yards around the caster");
                        else
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets the whole party.");
                    }
                    break;
                case SpellEQTargetType.PointBlankAreaOfEffect:
                case SpellEQTargetType.AreaOfEffectUndead: // TODO, may not be needed.  See "Words of the Undead King"
                    {
                        string targetTypeDescriptionFragment = "";
                        if (spellTemplate.EQTargetType == SpellEQTargetType.AreaOfEffectUndead)
                        {
                            spellTemplate.TargetCreatureType = 32; // Undead, 0x0020
                            targetTypeDescriptionFragment = "undead ";
                        }

                        if (isDetrimental == true)
                        {
                            // Referenced from Blast Wave
                            spellWOWTargetTypes.Add(SpellWOWTargetType.AreaAroundCaster);
                            spellWOWTargetTypes.Add(SpellWOWTargetType.AreaAroundCasterTargetingEnemies);
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets ", targetTypeDescriptionFragment, "enemies within ", spellRadius, " yards around the caster");
                        }
                        if (isDetrimental == false)
                        {
                            // Referenced from Circle of healing
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetAny);
                            spellWOWTargetTypes.Add(SpellWOWTargetType.AreaAroundTargetAlly);
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets ", targetTypeDescriptionFragment, "friendly units within ", spellRadius, " yards around the caster");
                        }

                    }
                    break;
                case SpellEQTargetType.Single:
                    {
                        if (canTargetBothEnemyAndFriendly == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetEnemy);
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetFriendly);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single enemy or friendly unit";
                        }
                        else if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetFriendly);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.Self:
                    {
                        spellWOWTargetTypes.Add(SpellWOWTargetType.Self);
                        spellTemplate.TargetDescriptionTextFragment = "Targets self";
                    }
                    break;
                case SpellEQTargetType.TargetedAreaOfEffect:
                case SpellEQTargetType.TargetedAreaOfEffectLifeTap:
                    {
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetEnemy);
                            spellWOWTargetTypes.Add(SpellWOWTargetType.AreaAroundTargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets an enemy and other enemies within ", spellRadius, " yards around the target");
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetFriendly);
                            spellWOWTargetTypes.Add(SpellWOWTargetType.AreaAroundTargetAlly);
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets a friendly unit and other friendly units within ", spellRadius, " yards around the target");
                        }
                    }
                    break;
                case SpellEQTargetType.Animal:
                    {
                        spellTemplate.TargetCreatureType = 1; // Beast, 0x0001
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single beast enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetAny); // "lull" put into this for now
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single beast friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.Undead:
                    {
                        spellTemplate.TargetCreatureType = 32; // Undead, 0x0020
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single undead enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetAny); // "lull" and heal undead put into this for now
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single undead friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.Summoned:
                    {
                        spellTemplate.TargetCreatureType = 8; // Elemental, 0x0008
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single elemental enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetAny);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single elemental friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.AreaOfEffectSummoned:
                    {
                        spellTemplate.TargetCreatureType = 8; // Elemental, 0x0008
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetEnemy);
                            spellWOWTargetTypes.Add(SpellWOWTargetType.AreaAroundTargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets an elemental enemy and other elemental enemies within ", spellRadius, " yards around the target");
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetAny);
                            spellWOWTargetTypes.Add(SpellWOWTargetType.AreaAroundTargetAlly);
                            spellTemplate.TargetDescriptionTextFragment = string.Concat("Targets an elemental friendly unit and other elemental friendly units within ", spellRadius, " yards around the target");
                        }
                    } break;
                case SpellEQTargetType.LifeTap:
                    {
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetFriendly);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.Pet:
                    {
                        spellWOWTargetTypes.Add(SpellWOWTargetType.Pet);
                        spellTemplate.TargetDescriptionTextFragment = "Targets your pet";
                    }
                    break;
                case SpellEQTargetType.Corpse:
                    {
                        spellWOWTargetTypes.Add(SpellWOWTargetType.None);
                        spellTemplate.TargetDescriptionTextFragment = "Targets a friendly corpse";
                        spellTemplate.CastOnCorpse = true;
                    }
                    break;
                case SpellEQTargetType.Plant:
                    {
                        // Using "elemental" for now
                        spellTemplate.TargetCreatureType = 8; // Elemental, 0x0008
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single elemental enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetAny); // "lull" and heal undead put into this for now
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single elemental friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.UberGiants:
                    {
                        spellTemplate.TargetCreatureType = 16; // Giant, 0x0010
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single giant enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetAny); // "lull" and heal put into this for now
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single giant friendly unit";
                        }
                    }
                    break;
                case SpellEQTargetType.UberDragons:
                    {
                        spellTemplate.TargetCreatureType = 2; // Dragonkin, 0x0002
                        if (isDetrimental == true)
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetEnemy);
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single dragonkin enemy";
                        }
                        else
                        {
                            spellWOWTargetTypes.Add(SpellWOWTargetType.TargetAny); // "lull" and heal put into this for now
                            spellTemplate.TargetDescriptionTextFragment = "Targets a single dragonkin friendly unit";
                        }
                    }
                    break;
                default:
                    {
                        Logger.WriteError("Unable to map eq target type ", spellTemplate.EQTargetType.ToString(), " to WOW target type");
                        spellWOWTargetTypes.Add(SpellWOWTargetType.Self);
                        spellTemplate.TargetDescriptionTextFragment = "Targets self";
                    }
                    break;
            }

            return spellWOWTargetTypes;
        }

        private static void PopulateEQSpellEffect(ref SpellTemplate spellTemplate, int slotID, Dictionary<string, string> rowColumns)
        {
            // Skip non and unmapped effects
            int effectIDRaw = int.Parse(rowColumns[string.Concat("effectid", slotID)]);          
            if (effectIDRaw == 254)
                return;
            if (Enum.IsDefined(typeof(SpellEQEffectType), effectIDRaw) == false)
            {
                Logger.WriteDebug(string.Concat("Skipping population of SpellEffect with EQID of ", spellTemplate.EQSpellID, " as the type ID ", effectIDRaw, " is not mapped"));
                return;
            }

            // Fill in the effect details
            SpellEffectEQ curEffect = new SpellEffectEQ();
            curEffect.EQEffectType = (SpellEQEffectType)effectIDRaw;
            if (slotID < 12)
            {
                int formulaRaw = int.Parse(rowColumns[string.Concat("formula", slotID)]);
                if (Enum.IsDefined(typeof(SpellEQBaseValueFormulaType), formulaRaw) == true)
                    curEffect.EQBaseValueFormulaType = (SpellEQBaseValueFormulaType)formulaRaw;
                else switch (formulaRaw)
                {
                    case 101: curEffect.EQBaseValueFormulaType = SpellEQBaseValueFormulaType.BaseAddLevelDivideTwo; break;
                    case 115: curEffect.EQBaseValueFormulaType = SpellEQBaseValueFormulaType.BaseAddSixTimesLevelMinusSpellLevel; break;
                    case 116: curEffect.EQBaseValueFormulaType = SpellEQBaseValueFormulaType.BaseAddEightTimesLevelMinusSpellLevel; break;
                    case 121: curEffect.EQBaseValueFormulaType = SpellEQBaseValueFormulaType.BaseAddLevelDivideThree; break;
                    default: curEffect.EQBaseValueFormulaType = SpellEQBaseValueFormulaType.UnknownUseBaseOrMaxWhicheverHigher; break;
                }
                curEffect.EQFormulaTypeValue = formulaRaw;
            }
            curEffect.EQBaseValue = int.Parse(rowColumns[string.Concat("effect_base_value", slotID)]);
            if (slotID < 4)
                curEffect.EQLimitValue = int.Parse(rowColumns[string.Concat("effect_limit_value", slotID)]);
            if (slotID < 11)
                curEffect.EQMaxValue = int.Parse(rowColumns[string.Concat("max", slotID)]);

            // Some spells have an effect formula based on the spell level, so they must have their "spell level" set for formulas
            if ((int)curEffect.EQBaseValueFormulaType >= 111 || (int)curEffect.EQBaseValueFormulaType <= 118)
                spellTemplate.HasEffectBaseFormulaUsingSpellLevel = true;

            // Add it
            spellTemplate.EQSpellEffects.Add(curEffect);
        }

        private static void ConvertEQSpellEffectsIntoWOWEffects(ref SpellTemplate spellTemplate, UInt32 schoolMask, bool hasSpellDuration, 
            int spellCastTimeInMS, List<SpellWOWTargetType> targets, int spellRadiusIndex, SortedDictionary<int, ItemTemplate> itemTemplatesByEQDBID)
        {
            // Process all spell effects
            foreach (SpellEffectEQ eqEffect in spellTemplate.EQSpellEffects)
            {
                List<SpellEffectWOW> newSpellEffects = new List<SpellEffectWOW>();
                switch (eqEffect.EQEffectType)
                {
                    case SpellEQEffectType.CurrentHitPoints: // Fallthrough
                    case SpellEQEffectType.CurrentHitPointsOnce:
                    case SpellEQEffectType.HealOverTime:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            string elementalSchoolName = string.Empty;
                            switch (schoolMask)
                            {
                                case 4: elementalSchoolName = "fire"; break;
                                case 8: elementalSchoolName = "nature"; break;
                                case 16: elementalSchoolName = "frost"; break;
                                case 32: elementalSchoolName = "shadow"; break;
                                case 64: elementalSchoolName = "arcane"; break;
                                default: break;
                            }

                            int preFormulaEffectAmount = Math.Abs(eqEffect.EQBaseValue);
                            if (hasSpellDuration == false || eqEffect.EQEffectType == SpellEQEffectType.CurrentHitPointsOnce)
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.None;
                                if (eqEffect.EQBaseValue > 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HealDirectHPS", SpellEffectWOWConversionScaleType.CastTime);
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.Heal;
                                    newSpellEffectWOW.ActionDescription = string.Concat("heal for ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                    spellTemplate.HighestDirectHealAmountInSpellEffect = Math.Max(spellTemplate.HighestDirectHealAmountInSpellEffect, newSpellEffectWOW.CalcEffectHighLevelValue);
                                }
                                else
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageDirectDPS", SpellEffectWOWConversionScaleType.CastTime);
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.SchoolDamage;
                                    if (elementalSchoolName.Length > 0)
                                        newSpellEffectWOW.ActionDescription = string.Concat("strike for ", newSpellEffectWOW.GetFormattedEffectActionString(false), " ", elementalSchoolName, " damage");
                                    else
                                        newSpellEffectWOW.ActionDescription = string.Concat("strike for ", newSpellEffectWOW.GetFormattedEffectActionString(false), " damage");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            }
                            else
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraPeriod = Convert.ToUInt32(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW) * 1000;
                                if (eqEffect.EQBaseValue > 0)
                                {
                                    newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HealOverTimeHPS", SpellEffectWOWConversionScaleType.Periodic);
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PeriodicHeal;
                                    newSpellEffectWOW.ActionDescription = string.Concat("regenerate ", newSpellEffectWOW.GetFormattedEffectActionString(false), " health per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                    newSpellEffectWOW.AuraDescription = string.Concat("regenerating", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " health per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                }
                                else
                                {
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PeriodicDamage;
                                    if (elementalSchoolName.Length > 0)
                                    {
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageOverTimeDPS", SpellEffectWOWConversionScaleType.Periodic);
                                        newSpellEffectWOW.ActionDescription = string.Concat("inflict ", newSpellEffectWOW.GetFormattedEffectActionString(false), " ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                        newSpellEffectWOW.AuraDescription = string.Concat("suffering", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " ", elementalSchoolName, " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                    }
                                    else
                                    {
                                        newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "DamageOverTimeDPS", SpellEffectWOWConversionScaleType.Periodic);
                                        newSpellEffectWOW.ActionDescription = string.Concat("inflict ", newSpellEffectWOW.GetFormattedEffectActionString(false), " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                        newSpellEffectWOW.AuraDescription = string.Concat("suffering", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " damage per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                    }
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            }
                        } break;
                    case SpellEQEffectType.CurrentMana:
                    case SpellEQEffectType.CurrentManaOnce:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;

                            int preFormulaEffectAmount = Math.Abs(eqEffect.EQBaseValue);
                            if (hasSpellDuration == false || eqEffect.EQEffectType == SpellEQEffectType.CurrentManaOnce)
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.None;
                                newSpellEffectWOW.EffectMiscValueA = 0; // Power Type = Mana
                                if (eqEffect.EQBaseValue > 0)
                                {
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.Energize;
                                    newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ManaUpDirect", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("restore ", newSpellEffectWOW.GetFormattedEffectActionString(false), " mana");
                                    spellTemplate.HighestDirectHealAmountInSpellEffect = Math.Max(spellTemplate.HighestDirectHealAmountInSpellEffect, newSpellEffectWOW.CalcEffectHighLevelValue);
                                }
                                else
                                {
                                    newSpellEffectWOW.EffectType = SpellWOWEffectType.PowerBurn;
                                    newSpellEffectWOW.EffectMultipleValue = 0;
                                    newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ManaDownDirect", SpellEffectWOWConversionScaleType.None);
                                    newSpellEffectWOW.ActionDescription = string.Concat("reduce ", newSpellEffectWOW.GetFormattedEffectActionString(false), " mana");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            }
                            else
                            {
                                SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectMiscValueA = 0; // Power Type = Mana
                                newSpellEffectWOW.EffectAuraPeriod = Convert.ToUInt32(Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW) * 1000;
                                if (eqEffect.EQBaseValue > 0)
                                {
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PeriodicEnergize;
                                    newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ManaUpOverTimeMPS", SpellEffectWOWConversionScaleType.Periodic);
                                    newSpellEffectWOW.ActionDescription = string.Concat("recover ", newSpellEffectWOW.GetFormattedEffectActionString(false), " mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                    newSpellEffectWOW.AuraDescription = string.Concat("recovering", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                }
                                else
                                {
                                    newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.PowerBurn;
                                    newSpellEffectWOW.EffectMultipleValue = 0;
                                    newSpellEffectWOW.SetEffectAmountValues(preFormulaEffectAmount, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ManaDownOvertimeMPS", SpellEffectWOWConversionScaleType.Periodic);
                                    newSpellEffectWOW.ActionDescription = string.Concat("reduce ", newSpellEffectWOW.GetFormattedEffectActionString(false), " mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                    newSpellEffectWOW.AuraDescription = string.Concat("reducing", newSpellEffectWOW.GetFormattedEffectAuraString(false, " ", ""), " mana per ", Configuration.SPELL_PERIODIC_SECONDS_PER_TICK_WOW, " seconds");
                                }
                                newSpellEffects.Add(newSpellEffectWOW);
                            }
                        }
                        break;
                    case SpellEQEffectType.ArmorClass:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                            newSpellEffectWOW.EffectMiscValueA = 1; // Armor
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArmorClassBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase armor by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("armor increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArmorClassDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease armor by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("armor decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.Attack:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModAttackPower;
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "AttackBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase attack power by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("attack power increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "AttackDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease attack power by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("attack power decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);

                            // Add a second for ranged attack power
                            SpellEffectWOW newSpellEffectWOW2 = newSpellEffectWOW.Clone();
                            newSpellEffectWOW2.ActionDescription = string.Empty;
                            newSpellEffectWOW2.AuraDescription = string.Empty;
                            newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.ModRangedAttackPower;
                            newSpellEffects.Add(newSpellEffectWOW2);
                        } break;
                    case SpellEQEffectType.MovementSpeed:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModIncreaseSpeed;
                                newSpellEffectWOW.ActionDescription = string.Concat("increase non-mounted movement speed by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                newSpellEffectWOW.AuraDescription = string.Concat("non-mounted movement speed increased", newSpellEffectWOW.GetFormattedEffectAuraString(true, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModDecreaseSpeed;
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease movement speed by ",newSpellEffectWOW.GetFormattedEffectActionString(true));
                                newSpellEffectWOW.AuraDescription = string.Concat("movement speed decreased", newSpellEffectWOW.GetFormattedEffectAuraString(true, " by ", ""));
                                newSpellEffectWOW.EffectMechanic = SpellMechanicType.Snared;
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.TotalHP:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModMaximumHealth;
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "MaxHPBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase maximum health by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("maximum health increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease maximum health by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("maximum health decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.Strength:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                            newSpellEffectWOW.EffectMiscValueA = 0; // Strength
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "StrengthBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase maximum strength by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("maximum strength increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "StrengthDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease maximum strength by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("maximum strength decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.Dexterity: // Fallthrough
                    case SpellEQEffectType.Agility:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            // EQ Dexterity and EQ Agility are both mapped WOW agility, so use the higher of the two and reuse if one exists
                            bool ignoreAsAglEffectExistsAndIsStronger = false;
                            foreach (SpellEffectWOW wowEffect in spellTemplate.WOWSpellEffects)
                            {
                                if (wowEffect.EffectAuraType == SpellWOWAuraType.ModStat && wowEffect.EffectMiscValueA == 1)
                                {
                                    if (wowEffect.EffectBasePoints > 0 && wowEffect.EffectBasePoints >= eqEffect.EQBaseValue)
                                        ignoreAsAglEffectExistsAndIsStronger = true;
                                    if (wowEffect.EffectBasePoints < 0 && wowEffect.EffectBasePoints <= eqEffect.EQBaseValue)
                                        ignoreAsAglEffectExistsAndIsStronger = true;
                                }
                            }
                            if (ignoreAsAglEffectExistsAndIsStronger == true)
                                continue;

                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                            newSpellEffectWOW.EffectMiscValueA = 1; // Agility
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "AgilityBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase agility by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("agility increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "AgilityDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease agility by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("agility decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.Stamina:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                            newSpellEffectWOW.EffectMiscValueA = 2; // Stamina
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "StaminaBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase stamina by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("stamina increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "StaminaDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease stamina by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("stamina decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.Intelligence:
                        {
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                            newSpellEffectWOW.EffectMiscValueA = 3; // Intellect
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "IntellectBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase intellect by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("intellect increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "IntellectDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease intellect by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("intellect decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.Wisdom:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStat;
                            newSpellEffectWOW.EffectMiscValueA = 4; // Spirit
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "SpiritBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase spirit by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("spirit increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "SpiritDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease spirit by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("spirit decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.Charisma:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModHitChance;
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HitPctBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase hit chance by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                newSpellEffectWOW.AuraDescription = string.Concat("hit chance increased", newSpellEffectWOW.GetFormattedEffectAuraString(true, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "HitPctDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease hit chance by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                newSpellEffectWOW.AuraDescription = string.Concat("hit chance decreased", newSpellEffectWOW.GetFormattedEffectAuraString(true, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.AttackSpeed:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModMeleeHaste;

                            // Baseline for attack speed is 100, so above that is increase and below that is decrease
                            newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue - 100, eqEffect.EQMaxValue - 100, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "", SpellEffectWOWConversionScaleType.None);
                            if (newSpellEffectWOW.EffectBasePoints >= 0)
                            {   
                                newSpellEffectWOW.ActionDescription = string.Concat("increase attack speed by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                newSpellEffectWOW.AuraDescription = string.Concat("attack speed increased", newSpellEffectWOW.GetFormattedEffectAuraString(true, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease attack speed by ", newSpellEffectWOW.GetFormattedEffectActionString(true));
                                newSpellEffectWOW.AuraDescription = string.Concat("attack speed decreased", newSpellEffectWOW.GetFormattedEffectAuraString(true, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);

                            // Add a second for ranged attack speed
                            SpellEffectWOW newSpellEffectWOW2 = newSpellEffectWOW.Clone();
                            newSpellEffectWOW2.ActionDescription = string.Empty;
                            newSpellEffectWOW2.AuraDescription = string.Empty;
                            newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.ModRangedHaste;
                            newSpellEffects.Add(newSpellEffectWOW2);
                        } break;
                    case SpellEQEffectType.InvisibilityUnstable:
                    case SpellEQEffectType.Invisibility:
                        {
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModInvisibility;
                            newSpellEffectWOW.ActionDescription = string.Concat("grants invisibility");
                            newSpellEffectWOW.AuraDescription = string.Concat("shrouded by invisibility");
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.SeeInvisibility:
                        {
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModInvisibilityDetect;
                            newSpellEffectWOW.EffectBasePoints = 1000;
                            newSpellEffectWOW.ActionDescription = string.Concat("grants ability to see invisibility");
                            newSpellEffectWOW.AuraDescription = string.Concat("able to see through invisibility");
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.WaterBreathing:
                        {
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.WaterBreathing;
                            newSpellEffectWOW.ActionDescription = string.Concat("grants ability to breath underwater");
                            newSpellEffectWOW.AuraDescription = string.Concat("able to breath underwater");
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    // Not happy with this
                    //case SpellEQEffectType.Blind:
                    //    {
                    //        SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                    //        newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                    //        newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModAttackerMeleeHitChance;
                    //        newSpellEffectWOW.EffectBasePoints = -200;
                    //        newSpellEffectWOW.ActionDescription = string.Concat("cause blindness which makes all hits miss and stops movement");
                    //        newSpellEffectWOW.AuraDescription = string.Concat("unable to land hits or move due to blindness");
                    //        newSpellEffects.Add(newSpellEffectWOW);

                    //        // Add a second for stopping movement
                    //        SpellEffectWOW newSpellEffectWOW2 = newSpellEffectWOW.Clone();
                    //        newSpellEffectWOW2.ActionDescription = string.Empty;
                    //        newSpellEffectWOW2.AuraDescription = string.Empty;
                    //        newSpellEffectWOW2.EffectBasePoints = 0;
                    //        newSpellEffectWOW2.EffectAuraType = SpellWOWAuraType.ModRoot;
                    //        newSpellEffects.Add(newSpellEffectWOW2);
                    //    } break;
                    case SpellEQEffectType.Stun:
                        {
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModStun;
                            newSpellEffectWOW.ActionDescription = string.Concat("stuns");
                            newSpellEffectWOW.AuraDescription = string.Concat("stunned");
                            spellTemplate.AuraDuration.SetFixedDuration(Math.Max(eqEffect.EQBaseValue, 500));
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.Fear:
                        {
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModFear;
                            newSpellEffectWOW.ActionDescription = string.Concat("run away in fear");
                            newSpellEffectWOW.AuraDescription = string.Concat("running in fear");
                            spellTemplate.BreakEffectOnNonAutoDirectDamage = true;
                            spellTemplate.NoPartialImmunity = true;
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.Root:
                        {
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModRoot;
                            newSpellEffectWOW.ActionDescription = string.Concat("roots in place stops movement by rooting in place");
                            newSpellEffectWOW.AuraDescription = string.Concat("rooted");
                            spellTemplate.BreakEffectOnNonAutoDirectDamage = true;
                            spellTemplate.NoPartialImmunity = true;
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.CancelMagic:
                        {
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.Dispel;
                            newSpellEffectWOW.EffectMiscValueA = 1; // 1 = Magic
                            newSpellEffectWOW.EffectDieSides = 1;
                            int numOfOtherDispels = 0;
                            foreach (SpellEffectWOW wowEffect in spellTemplate.WOWSpellEffects)
                            {
                                if (wowEffect.EffectType == SpellWOWEffectType.Dispel && wowEffect.EffectMiscValueA == 1) 
                                {
                                    wowEffect.AuraDescription = string.Empty;
                                    wowEffect.ActionDescription = string.Empty;
                                    numOfOtherDispels++;
                                }
                            }
                            int totalDispelCount = numOfOtherDispels + 1;
                            newSpellEffectWOW.ActionDescription = string.Concat("dispels ", totalDispelCount, " beneficial (enemy) or detrimental (friendly) magic effect");
                            if (totalDispelCount > 1)
                                newSpellEffectWOW.ActionDescription = string.Concat(newSpellEffectWOW.ActionDescription, "s");
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.DiseaseCounter:
                        {
                            // Positive values are associated with other disease effects and 'counters' don't exist in WoW, so just ignore them
                            if (eqEffect.EQBaseValue >= 0)
                                continue;

                            // Calculate a potency
                            int totalDispels = 1;
                            if (eqEffect.EQBaseValue < -1 && eqEffect.EQBaseValue > -10)
                                totalDispels = 2;
                            else if (eqEffect.EQBaseValue <= -10)
                                totalDispels = 3;

                            // Update the true count by counting other ones
                            int numOfOtherDispels = 0;
                            foreach (SpellEffectWOW wowEffect in spellTemplate.WOWSpellEffects)
                            {
                                if (wowEffect.EffectType == SpellWOWEffectType.Dispel && wowEffect.EffectMiscValueA == 3) // 3 = Disease
                                {
                                    wowEffect.AuraDescription = string.Empty;
                                    wowEffect.ActionDescription = string.Empty;
                                    numOfOtherDispels++;
                                }
                            }
                            int totalDispelCount = totalDispels + numOfOtherDispels;

                            // Create one for each count
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.Dispel;
                            newSpellEffectWOW.EffectMiscValueA = 3; // 3 = Disease
                            newSpellEffectWOW.EffectDieSides = 1;
                            for (int i = 1; i < totalDispels; i++)
                            {
                                SpellEffectWOW newSpellEffectWOWAdditonal = newSpellEffectWOW.Clone();
                                newSpellEffectWOWAdditonal.ActionDescription = string.Empty;
                                newSpellEffectWOWAdditonal.AuraDescription = string.Empty;
                                newSpellEffects.Add(newSpellEffectWOWAdditonal);
                            }

                            // Set the in the display
                            newSpellEffectWOW.ActionDescription = string.Concat("cures ", totalDispelCount, " disease");
                            if (totalDispels > 1)
                                newSpellEffectWOW.ActionDescription = string.Concat(newSpellEffectWOW.ActionDescription, "s");
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.PoisonCounter:
                        {
                            // Positive values are associated with other poison effects and 'counters' don't exist in WoW, so just ignore them
                            if (eqEffect.EQBaseValue >= 0)
                                continue;

                            // Calculate a potency
                            int totalDispels = 1;
                            if (eqEffect.EQBaseValue < -1 && eqEffect.EQBaseValue > -10)
                                totalDispels = 2;
                            else if (eqEffect.EQBaseValue <= -10)
                                totalDispels = 3;

                            // Update the true count by counting other ones
                            int numOfOtherDispels = 0;
                            foreach (SpellEffectWOW wowEffect in spellTemplate.WOWSpellEffects)
                            {
                                if (wowEffect.EffectType == SpellWOWEffectType.Dispel && wowEffect.EffectMiscValueA == 4) // 4 = Poison
                                {
                                    wowEffect.AuraDescription = string.Empty;
                                    wowEffect.ActionDescription = string.Empty;
                                    numOfOtherDispels++;
                                }
                            }
                            int totalDispelCount = totalDispels + numOfOtherDispels;

                            // Create one for each count
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.Dispel;
                            newSpellEffectWOW.EffectMiscValueA = 4; // 4 = Poison
                            newSpellEffectWOW.EffectDieSides = 1;
                            for (int i = 1; i < totalDispels; i++)
                            {
                                SpellEffectWOW newSpellEffectWOWAdditonal = newSpellEffectWOW.Clone();
                                newSpellEffectWOWAdditonal.ActionDescription = string.Empty;
                                newSpellEffectWOWAdditonal.AuraDescription = string.Empty;
                                newSpellEffects.Add(newSpellEffectWOWAdditonal);
                            }

                            // Set the in the display
                            newSpellEffectWOW.ActionDescription = string.Concat("cures ", totalDispelCount, " poison");
                            if (totalDispels > 1)
                                newSpellEffectWOW.ActionDescription = string.Concat(newSpellEffectWOW.ActionDescription, "s");
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.SummonItems:
                        {
                            if (itemTemplatesByEQDBID.ContainsKey(eqEffect.EQBaseValue) == false)
                            {
                                Logger.WriteWarning("Failed to summon items with eq id ", eqEffect.EQBaseValue.ToString() , " for eq spell id ", spellTemplate.EQSpellID.ToString(), " as it was not found in the item list");
                                continue;
                            }

                            int itemCount = Math.Max(eqEffect.EQMaxValue, 1);
                            SpellEffectWOW spellEffectWOW = new SpellEffectWOW();
                            spellEffectWOW.EffectType = SpellWOWEffectType.CreateItem;
                            spellEffectWOW.EffectBasePoints = itemCount;
                            spellEffectWOW.EffectItemType = Convert.ToUInt32(itemTemplatesByEQDBID[eqEffect.EQBaseValue].WOWEntryID);
                            itemTemplatesByEQDBID[eqEffect.EQBaseValue].IsCreatedBySpell = true;
                            string itemName = itemTemplatesByEQDBID[eqEffect.EQBaseValue].Name;
                            if (itemCount == 1)
                                spellEffectWOW.ActionDescription = string.Concat("Conjure ", itemCount, " ", itemName, ".\n\nConjured items disappear if logged out for more than 15 minutes.");
                            else
                                spellEffectWOW.ActionDescription = string.Concat("Conjure ", itemCount, " ", itemName, "s.\n\nConjured items disappear if logged out for more than 15 minutes.");
                            newSpellEffects.Add(spellEffectWOW);
                        } break;
                    case SpellEQEffectType.ResistFire:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                            newSpellEffectWOW.EffectMiscValueA = 4; // Fire
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "FireResistanceBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase fire resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("fire resistance increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "FireResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease fire resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("fire resistance decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.ResistCold:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                            newSpellEffectWOW.EffectMiscValueA = 16; // Frost
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "FrostResistanceBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase frost resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("frost resistance increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "FrostResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease frost resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("frost resistance decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.ResistPoison:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                            newSpellEffectWOW.EffectMiscValueA = 8; // Nature
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "NatureResistanceBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase nature resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("nature resistance increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "NatureResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease nature resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("nature resistance decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.ResistDisease:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                            newSpellEffectWOW.EffectMiscValueA = 32; // Shadow
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ShadowResistanceBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase shadow resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("shadow resistance increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ShadowResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease shadow resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("shadow resistance decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.ResistMagic:
                        {
                            if (eqEffect.EQBaseValue == 0)
                                continue;
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.ModResistance;
                            newSpellEffectWOW.EffectMiscValueA = 64; // Arcane
                            if (eqEffect.EQBaseValue >= 0)
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArcaneResistanceBuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("increase arcane resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("arcane resistance increased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            }
                            else
                            {
                                newSpellEffectWOW.SetEffectAmountValues(eqEffect.EQBaseValue, eqEffect.EQMaxValue, spellTemplate.MinimumPlayerLearnLevel, eqEffect.EQBaseValueFormulaType, spellCastTimeInMS, "ArcaneResistanceDebuff", SpellEffectWOWConversionScaleType.None);
                                newSpellEffectWOW.ActionDescription = string.Concat("decrease arcane resistance by ", newSpellEffectWOW.GetFormattedEffectActionString(false));
                                newSpellEffectWOW.AuraDescription = string.Concat("arcane resistance decreased", newSpellEffectWOW.GetFormattedEffectAuraString(false, " by ", ""));
                            } newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.Revive:
                        {
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.ResurrectNew;
                            int amountRestored = Math.Max(Configuration.SPELL_EFFECT_REVIVE_EXPPCT_TO_HPMP_MULTIPLIER * eqEffect.EQBaseValue, 1);
                            newSpellEffectWOW.EffectBasePoints = amountRestored;
                            newSpellEffectWOW.ActionDescription = string.Concat("brings a dead player back to life with ", amountRestored, " health and ", amountRestored, " mana");
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.Gate:
                        {
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            if (Configuration.SPELLS_GATE_TETHER_ENABLED == true)
                            {
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.ApplyAura;
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.Dummy;
                                newSpellEffectWOW.ActionDescription = "opens a magical portal that returns you to your bind point in norrath, and you will have 30 minutes where you can return to your gate point after casting it";
                                newSpellEffectWOW.AuraDescription = "you are tethered to the location where you gated and may return there if you click it off before the buff wears off, but it will fail in combat";
                                newSpellEffectWOW.EffectMiscValueA = 3;
                                spellTemplate.AuraDuration.SetFixedDuration(1800000); // 30 minutes
                            }
                            else
                            {
                                newSpellEffectWOW.EffectType = SpellWOWEffectType.Dummy;
                                newSpellEffectWOW.ActionDescription = "opens a magical portal that returns you to your bind point in norrath";
                                newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.None;
                                newSpellEffectWOW.EffectMiscValueA = 3;
                            }
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    case SpellEQEffectType.BindAffinity:
                        {
                            SpellEffectWOW newSpellEffectWOW = new SpellEffectWOW();
                            newSpellEffectWOW.EffectType = SpellWOWEffectType.Dummy;
                            newSpellEffectWOW.EffectAuraType = SpellWOWAuraType.Dummy;
                            newSpellEffectWOW.EffectMiscValueA = 2;
                            newSpellEffectWOW.ActionDescription = string.Concat("binds the soul of the target to their current location, which only works in norrath");
                            newSpellEffects.Add(newSpellEffectWOW);
                        } break;
                    default:
                        {
                            Logger.WriteError("Unhandled SpellTemplate EQEffectType of ", eqEffect.EQEffectType.ToString(), " for eq spell id ", spellTemplate.EQSpellID.ToString());
                            continue;
                        }
                }

                // Add the target types and radius
                foreach (SpellEffectWOW newSpellEffect in newSpellEffects)
                {
                    if (targets.Count == 0)
                    {
                        Logger.WriteError("Too few targets for spell effect");
                        continue;
                    }
                    if (targets.Count > 2)
                    {
                        Logger.WriteError("Too many targets for spell effect");
                        continue;
                    }
                    newSpellEffect.ImplicitTargetA = targets[0];
                    if (targets.Count == 2)
                        newSpellEffect.ImplicitTargetB = targets[1];
                    newSpellEffect.EffectRadiusIndex = Convert.ToUInt32(spellRadiusIndex);
                    spellTemplate.WOWSpellEffects.Add(newSpellEffect);
                }

            }

            // TODO: Collapse multi-stat effects into 1 where possible

            // Sort them so the aura effects are last
            spellTemplate.WOWSpellEffects.Sort();
        }

        private static void SetAuraStackRule(ref SpellTemplate spellTemplate, int eqSpellCategory)
        {
            // This is exactly how EQ does spell stacking, but it seems like an appropriate approximation that works well for wow
            if (eqSpellCategory < 0) // NPC = -99, AA Procs = -1
                return;
            if (eqSpellCategory > 250) // AA Abilities = 999
                return;

            // Damage detrimental effects should stack from multiple sources / spells
            if (eqSpellCategory == 7 || // Damage over time (magic)
                eqSpellCategory == 8 || // Damage over time (undead)
                eqSpellCategory == 9 || // Damage over time (life taps)
                eqSpellCategory == 129 || // Damage over time (fire)
                eqSpellCategory == 130 || // Damage over time (ice)
                eqSpellCategory == 131 || // Damage over time (poison)
                eqSpellCategory == 132) // Damage over time (disease)
                spellTemplate.SpellGroupStackingRule = 2; // SPELL_GROUP_STACK_FLAG_NOT_SAME_CASTER
            else
                spellTemplate.SpellGroupStackingRule |= 8; // SPELL_GROUP_STACK_FLAG_NEVER_STACK

            // Calculate the category
            int groupStackingID = Configuration.SQL_SPELL_GROUP_ID_START + eqSpellCategory;
            if (SpellGroupStackRuleByGroup.ContainsKey(groupStackingID) == false)
                SpellGroupStackRuleByGroup[groupStackingID] = spellTemplate.SpellGroupStackingRule;
            spellTemplate.SpellGroupStackingID = groupStackingID;
        }

        private static void SetMainAndAuraDescriptions(ref SpellTemplate spellTemplate)
        {
            StringBuilder descriptionSB = new StringBuilder();
            StringBuilder auraSB = new StringBuilder();
            bool descriptionTextHasBeenAddedToAction = false;
            bool descriptionTextHasBeenAddedToAura = false;
            for (int i = 0; i < spellTemplate.WOWSpellEffects.Count; i++)
            {
                SpellEffectWOW spellEffectWOW = spellTemplate.WOWSpellEffects[i];
                if (spellEffectWOW.ActionDescription.Length > 0 )
                {
                    if (descriptionTextHasBeenAddedToAction == true)
                        descriptionSB.Append(", ");
                    descriptionSB.Append(spellTemplate.WOWSpellEffects[i].ActionDescription);
                    descriptionTextHasBeenAddedToAction = true;
                }
                if (spellEffectWOW.AuraDescription.Length > 0)
                {
                    if (descriptionTextHasBeenAddedToAura == true)
                        auraSB.Append(", ");
                    auraSB.Append(spellTemplate.WOWSpellEffects[i].AuraDescription);
                    descriptionTextHasBeenAddedToAura = true;
                }
            }

            // Store and control capitalization
            descriptionSB.Append('.');
            auraSB.Append('.');
            spellTemplate.Description = descriptionSB.ToString();
            if (spellTemplate.Description.Length > 0)
                spellTemplate.Description = string.Concat(char.ToUpper(spellTemplate.Description[0]), spellTemplate.Description.Substring(1));
            spellTemplate.AuraDescription = auraSB.ToString();
            if (spellTemplate.AuraDescription.Length > 0)
                spellTemplate.AuraDescription = string.Concat(char.ToUpper(spellTemplate.AuraDescription[0]), spellTemplate.AuraDescription.Substring(1));

            // Add target information to spell
            spellTemplate.Description = string.Concat(spellTemplate.Description, " ", spellTemplate.TargetDescriptionTextFragment, ".");

            // Add the time duration
            spellTemplate.Description = string.Concat(spellTemplate.Description, GetTimeDurationStringFromMSWithLeadingSpace(spellTemplate.AuraDuration.MaxDurationInMS, spellTemplate.AuraDuration.GetTimeText()));

            // Add any additional fragments to descriptions
            if (spellTemplate.BreakEffectOnNonAutoDirectDamage == true)
                spellTemplate.Description = string.Concat(spellTemplate.Description, " May break on direct damage.");

            // Capitalize Norrath
            spellTemplate.Description = spellTemplate.Description.Replace("norrath", "Norrath");
        }

        private static string GetTimeDurationStringFromMSWithLeadingSpace(int durationInMS, string timeFragment)
        {
            // Skip no duration
            if (durationInMS < 1000)
                return string.Empty;

            // Get the time string
            return string.Concat(" Lasts ", timeFragment, ".");
        }

        private static string GetTimeTextFromSeconds(int inputSeconds)
        {
            StringBuilder timeSB = new StringBuilder();
            int hours = inputSeconds / 3600;
            if (hours > 0)
            {
                timeSB.Append(hours);
                timeSB.Append(" hour");
                if (hours != 1)
                    timeSB.Append("s");
            }
            int minutes = (inputSeconds % 3600) / 60;
            if (minutes > 0)
            {
                if (hours > 0)
                    timeSB.Append(" ");
                timeSB.Append(minutes);
                timeSB.Append(" minute");
                if (minutes != 1)
                    timeSB.Append("s");
            }
            int seconds = (inputSeconds % 60);
            if (seconds > 0)
            {
                if (hours > 0 || minutes > 0)
                    timeSB.Append(" ");
                timeSB.Append(seconds);
                timeSB.Append(" second");
                if (seconds != 1)
                    timeSB.Append("s");
            }
            return timeSB.ToString();
        }

        public List<List<SpellEffectWOW>> GetWOWEffectsInBlocksOfThree()
        {
            List<List<SpellEffectWOW>> returnList = new List<List<SpellEffectWOW>> ();
           
            // Blank out if no spell effects
            if (WOWSpellEffects.Count == 0)
            {
                List<SpellEffectWOW> curBlockSpellEffects = new List<SpellEffectWOW>();
                for (int i = 0; i < 3; i++)
                    curBlockSpellEffects.Add(new SpellEffectWOW());
                returnList.Add(curBlockSpellEffects);
                return returnList;
            }

            // Pre-group by 'max' levels so that spell value calculations work properly
            Dictionary<int, List<SpellEffectWOW>> spellEffectsByMaxLevel = new Dictionary<int, List<SpellEffectWOW>>();
            foreach (SpellEffectWOW spellEffect in WOWSpellEffects)
            {
                if (spellEffectsByMaxLevel.ContainsKey(spellEffect.CalcEffectHighLevel) == false)
                    spellEffectsByMaxLevel.Add(spellEffect.CalcEffectHighLevel, new List<SpellEffectWOW>());
                spellEffectsByMaxLevel[spellEffect.CalcEffectHighLevel].Add(spellEffect);
            }

            // Otherwise, group in batches of 3 adding blanks to pad the space
            // Group these by maximum level of related effects
            foreach (var curEffectList in spellEffectsByMaxLevel.Values)
            {
                int numOfExtractedEffects = 0;
                while (numOfExtractedEffects < curEffectList.Count)
                {
                    List<SpellEffectWOW> curBlockSpellEffects = new List<SpellEffectWOW>();
                    for (int i = 0; i < 3; i++)
                    {
                        if (numOfExtractedEffects >= curEffectList.Count)
                            curBlockSpellEffects.Add(new SpellEffectWOW());
                        else
                            curBlockSpellEffects.Add(curEffectList[numOfExtractedEffects]);
                        numOfExtractedEffects += 1;
                    }
                    returnList.Add(curBlockSpellEffects);
                }
            }
            return returnList;
        }
    }
}
