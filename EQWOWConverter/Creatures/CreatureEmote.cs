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
    internal class CreatureEmote
    {
        private static Dictionary<int, List<CreatureEmote>> EmotesByEQEmoteSetID = new Dictionary<int, List<CreatureEmote>>();
        private static Dictionary<int, List<CreatureEmote>> AmbientEmotesByEQCreatureTemplateID = new Dictionary<int, List<CreatureEmote>>();
        private static readonly object EmoteLock = new object();

        public CreatureEmoteEventType EventType = CreatureEmoteEventType.EnterCombat;
        public CreatureEmoteType EmoteType = CreatureEmoteType.Say;
        public float ChancePct = 100;
        public int Param1 = 0;
        public int Param2 = 0;
        public string Text = string.Empty;

        public static Dictionary<int, List<CreatureEmote>> GetEmotesByEQEmoteSetID()
        {
            lock (EmoteLock)
            {
                if (EmotesByEQEmoteSetID.Count == 0)
                    PopulateEmoteLists();
                return EmotesByEQEmoteSetID;
            }
        }

        public static Dictionary<int, List<CreatureEmote>> GetAmbientEmotesByEQCreatureTemplateID()
        {
            lock (EmoteLock)
            {
                if (EmotesByEQEmoteSetID.Count == 0)
                    PopulateEmoteLists();
                return AmbientEmotesByEQCreatureTemplateID;
            }
        }

        private static void PopulateEmoteLists()
        {
            string emotesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureEmotes.csv");
            Logger.WriteDebug("Populating Creature Emote list via file '", emotesFile, "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(emotesFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                int emoteSetID = int.Parse(columns["emoteid"]);
                int eventID = int.Parse(columns["event"]);
                int typeID = int.Parse(columns["type"]);

                // TAKP fires both 'KilledPC' (5) and 'Killed' (9) when an NPC kills a player, so combine
                if (eventID == 9)
                    eventID = (int)CreatureEmoteEventType.KilledPC;
                if (eventID < 0 || eventID > (int)CreatureEmoteEventType.OnDespawn)
                {
                    Logger.WriteError("CreatureEmote row with emoteid '", emoteSetID.ToString(), "' has unknown event '", eventID.ToString(), "'");
                    continue;
                }
                if (typeID < 0 || typeID > (int)CreatureEmoteType.Proximity)
                {
                    Logger.WriteError("CreatureEmote row with emoteid '", emoteSetID.ToString(), "' has unknown type '", typeID.ToString(), "'");
                    continue;
                }

                CreatureEmote newEmote = new CreatureEmote();
                newEmote.EventType = (CreatureEmoteEventType)eventID;
                newEmote.EmoteType = (CreatureEmoteType)typeID;
                newEmote.Text = ConditionEmoteText(columns["text"]);
                if (newEmote.Text.Length == 0)
                    continue;
                if (EmotesByEQEmoteSetID.ContainsKey(emoteSetID) == false)
                    EmotesByEQEmoteSetID.Add(emoteSetID, new List<CreatureEmote>());
                EmotesByEQEmoteSetID[emoteSetID].Add(newEmote);
            }

            // Ambient emotes (from EQ quest scripts)
            string ambientEmotesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureAmbientEmotes.csv");
            Logger.WriteDebug("Populating Creature Ambient Emote list via file '" + ambientEmotesFile + "'");
            rows = FileTool.ReadAllRowsFromFileWithHeader(ambientEmotesFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip any invalid expansion rows
                int minExpansion = int.Parse(columns["min_expansion"]);
                int maxExpansion = int.Parse(columns["max_expansion"]);
                if (minExpansion != -1 && minExpansion > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;
                if (maxExpansion != -1 && maxExpansion < Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;

                int eqCreatureTemplateID = int.Parse(columns["eq_npc_id"]);
                CreatureEmote newEmote = new CreatureEmote();
                switch (columns["event"].Trim().ToLower())
                {
                    case "timer": newEmote.EventType = CreatureEmoteEventType.RandomTimer; break;
                    case "proximity": newEmote.EventType = CreatureEmoteEventType.Proximity; break;
                    case "spawn": newEmote.EventType = CreatureEmoteEventType.OnSpawn; break;
                    case "death": newEmote.EventType = CreatureEmoteEventType.OnDeath; break;
                    case "combat": newEmote.EventType = CreatureEmoteEventType.EnterCombat; break;
                    default:
                        {
                            Logger.WriteError("CreatureAmbientEmote row with id '", columns["id"], "' has unknown event '", columns["event"], "'");
                            continue;
                        }
                }
                switch (columns["emote_type"].Trim().ToLower())
                {
                    case "say": newEmote.EmoteType = CreatureEmoteType.Say; break;
                    case "emote": newEmote.EmoteType = CreatureEmoteType.Emote; break;
                    case "shout": newEmote.EmoteType = CreatureEmoteType.Shout; break;
                    case "proximity": newEmote.EmoteType = CreatureEmoteType.Proximity; break;
                    default:
                        {
                            Logger.WriteError("CreatureAmbientEmote row with id '", columns["id"], "' has unknown emote_type '", columns["emote_type"], "'");
                            continue;
                        }
                }
                newEmote.ChancePct = float.Parse(columns["chance"]);
                newEmote.Param1 = int.Parse(columns["param1"]);
                newEmote.Param2 = int.Parse(columns["param2"]);

                // Convert proximity radius to WoW units
                if (newEmote.EventType == CreatureEmoteEventType.Proximity)
                    newEmote.Param1 = (int)MathF.Ceiling((float)newEmote.Param1 * Configuration.GENERATE_WORLD_SCALE);

                newEmote.Text = ConditionEmoteText(columns["text"]);
                if (newEmote.Text.Length == 0)
                    continue;
                if (AmbientEmotesByEQCreatureTemplateID.ContainsKey(eqCreatureTemplateID) == false)
                    AmbientEmotesByEQCreatureTemplateID.Add(eqCreatureTemplateID, new List<CreatureEmote>());
                AmbientEmotesByEQCreatureTemplateID[eqCreatureTemplateID].Add(newEmote);
            }
        }

        private static string ConditionEmoteText(string text)
        {
            string conditionedText = text.Trim();
            conditionedText = conditionedText.Replace("\"", "'");
            conditionedText = conditionedText.Replace("\\", "");
            conditionedText = conditionedText.Replace("$mname", "$MN");
            conditionedText = conditionedText.Replace("$mracep", "$MRP");
            conditionedText = conditionedText.Replace("$mrace", "$MR");
            conditionedText = conditionedText.Replace("$mclass", "$MC");
            conditionedText = conditionedText.Replace("$racep", "$RP");
            conditionedText = conditionedText.Replace("$name", "$N");
            conditionedText = conditionedText.Replace("$race", "$R");
            conditionedText = conditionedText.Replace("$class", "$C");
            return conditionedText;
        }
    }
}
