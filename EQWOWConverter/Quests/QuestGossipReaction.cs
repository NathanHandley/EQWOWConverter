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

namespace EQWOWConverter.Quests
{
    internal class QuestGossipReaction
    {
        private static List<QuestGossipReaction> GossipReactions = new List<QuestGossipReaction>();
        private static bool IsLoaded = false;

        public string ZoneShortName = string.Empty;
        public string CreatureName = string.Empty;
        public string MenuText = string.Empty;
        public int OptionID = 0;
        public string OptionText = string.Empty;
        public QuestReactionType ReactionType;
        public string ReactionValue = string.Empty;
        public bool UsePlayerX = false;
        public bool UsePlayerY = false;
        public bool UsePlayerZ = false;
        public bool UsePlayerHeading = false;
        public bool UseNpcX = false;
        public bool UseNpcY = false;
        public bool UseNpcZ = false;
        public bool UseNpcHeading = false;
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float EQHeading;
        public float WOWOrientation;
        public float AddedX;
        public float AddedY;
        public bool CreatureIsSelf = false;
        public int CreatureEQID = 0;
        public int DelayInMS = 0;

        public static List<QuestGossipReaction> GetGossipReactions()
        {
            if (IsLoaded == false)
                PopulateGossipReactionList();
            return GossipReactions;
        }

        private static void PopulateGossipReactionList()
        {
            IsLoaded = true;
            string gossipReactionsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "GossipReactions.csv");
            Logger.WriteDebug(string.Concat("Loading gossip reactions via file '", gossipReactionsFile, "'"));
            if (File.Exists(gossipReactionsFile) == false)
            {
                Logger.WriteError(string.Concat("Could not load gossip reactions, file did not exist at '", gossipReactionsFile, "'"));
                return;
            }
            List<Dictionary<string, string>> reactionRows = FileTool.ReadAllRowsFromFileWithHeader(gossipReactionsFile, "|");
            foreach (Dictionary<string, string> columns in reactionRows)
            {
                QuestGossipReaction reaction = new QuestGossipReaction();
                reaction.ZoneShortName = columns["zone_shortname"];
                reaction.CreatureName = columns["creature_name"];
                reaction.MenuText = columns["menu_text"];
                reaction.OptionID = int.Parse(columns["option_id"]);
                reaction.OptionText = columns["option_text"];
                string reactionTypeString = columns["Type"];
                string reactionValue1 = columns["ReactionValue"];
                switch (reactionTypeString)
                {
                    case "attackplayer":
                        {
                            reaction.ReactionType = QuestReactionType.AttackPlayer;
                            reaction.CreatureIsSelf = true;
                        } break;
                    case "despawn":
                        {
                            reaction.ReactionType = QuestReactionType.Despawn;
                            if (reactionValue1 == "self")
                                reaction.CreatureIsSelf = true;
                            else
                                reaction.CreatureEQID = int.Parse(reactionValue1);
                        } break;
                    case "emote":
                        {
                            reaction.ReactionType = QuestReactionType.Emote;
                            reaction.ReactionValue = reactionValue1;
                        } break;
                    case "say":
                        {
                            reaction.ReactionType = QuestReactionType.Say;
                            reaction.ReactionValue = reactionValue1;
                        } break;
                    case "yell":
                        {
                            reaction.ReactionType = QuestReactionType.Yell;
                            reaction.ReactionValue = reactionValue1;
                        } break;
                    case "spawn":
                        {
                            reaction.ReactionType = QuestReactionType.Spawn;
                            reaction.CreatureEQID = int.Parse(reactionValue1);
                            PopulateReactionPositionFromColumns(reaction, columns);
                        } break;
                    case "spawnunique":
                        {
                            reaction.ReactionType = QuestReactionType.SpawnUnique;
                            reaction.CreatureEQID = int.Parse(reactionValue1);
                            PopulateReactionPositionFromColumns(reaction, columns);
                        } break;
                    default:
                        {
                            Logger.WriteError(string.Concat("Unhandled gossip reaction type of '", reactionTypeString, "'"));
                            continue;
                        }
                }

                string delayString = columns["DelayMS"];
                if (delayString.Length > 0)
                    reaction.DelayInMS = int.Parse(delayString);

                // Scale positions for wow world scale
                reaction.PositionX *= Configuration.GENERATE_WORLD_SCALE;
                reaction.PositionY *= Configuration.GENERATE_WORLD_SCALE;
                reaction.PositionZ *= Configuration.GENERATE_WORLD_SCALE;
                reaction.AddedX *= Configuration.GENERATE_WORLD_SCALE;
                reaction.AddedY *= Configuration.GENERATE_WORLD_SCALE;

                // Convert heading
                if (reaction.EQHeading != 0)
                {
                    float modHeading = reaction.EQHeading / (256f / 360f);
                    reaction.WOWOrientation = modHeading * Convert.ToSingle(Math.PI / 180);
                }

                GossipReactions.Add(reaction);
            }
        }

        private static void PopulateReactionPositionFromColumns(QuestGossipReaction reaction, Dictionary<string, string> columns)
        {
            string positionXString = columns["PositionX"];
            if (positionXString == "playerX")
                reaction.UsePlayerX = true;
            else if (positionXString == "npcX")
                reaction.UseNpcX = true;
            else
                reaction.PositionX = ParseTool.ParseFloat(positionXString, 0);
            string positionYString = columns["PositionY"];
            if (positionYString == "playerY")
                reaction.UsePlayerY = true;
            else if (positionYString == "npcY")
                reaction.UseNpcY = true;
            else
                reaction.PositionY = ParseTool.ParseFloat(positionYString, 0);
            string positionZString = columns["PositionZ"];
            if (positionZString == "playerZ")
                reaction.UsePlayerZ = true;
            else if (positionZString == "npcZ")
                reaction.UseNpcZ = true;
            else
                reaction.PositionZ = ParseTool.ParseFloat(positionZString, 0);
            string headingString = columns["Heading"];
            if (headingString == "playerHeading")
                reaction.UsePlayerHeading = true;
            else if (headingString == "npcHeading")
                reaction.UseNpcHeading = true;
            else
                reaction.EQHeading = ParseTool.ParseFloat(headingString, 0);
            string addedXString = columns["AddedX"];
            if (addedXString.Length > 0)
                reaction.AddedX = ParseTool.ParseFloat(addedXString, 0);
            string addedYString = columns["AddedY"];
            if (addedYString.Length > 0)
                reaction.AddedY = ParseTool.ParseFloat(addedYString, 0);
        }
    }
}
