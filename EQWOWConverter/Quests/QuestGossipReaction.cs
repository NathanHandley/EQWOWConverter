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
        public bool MovementIsRun = false;
        public int PathGridID = -1;
        public int PathGridStartNode = -1;
        public int PathGridEndNode = -1;
        public int PathListID = 0;
        public int GameObjectID = -1;
        public int GameObjectEntryID = 0;
        public int GameObjectLifetimeSec = 0;
        public int RequiredQuestID = 0;
        public int RequiredAtGridID = -1;
        public int RequiredAtGridNode = -1;
        public float RequiredNearX = 0;
        public float RequiredNearY = 0;
        public float RequiredNearZ = 0;
        public float RequiredNearDistance = 0;
        public bool FiresOnArrival = false;

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
            HashSet<(string, string, int)> optionsAlreadyWalking = new HashSet<(string, string, int)>();
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
                    case "attacknpc":
                        {
                            reaction.ReactionType = QuestReactionType.AttackNpc;
                            reaction.CreatureEQID = int.Parse(reactionValue1);
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
                    case "spawnobject":
                        {
                            reaction.ReactionType = QuestReactionType.SpawnObject;
                            reaction.CreatureIsSelf = true;
                            reaction.GameObjectID = int.Parse(reactionValue1);
                        }
                        break;
                    case "walkgrid":
                        {
                            reaction.ReactionType = QuestReactionType.WalkGrid;
                            reaction.CreatureIsSelf = true;
                            int gridID;
                            int startNode;
                            int endNode;
                            if (QuestReaction.TryParseWalkGridValue(reactionValue1, out gridID, out startNode, out endNode) == false)
                            {
                                Logger.WriteError(string.Concat("Unreadable walkgrid value of '", reactionValue1, "'"));
                                continue;
                            }
                            reaction.PathGridID = gridID;
                            reaction.PathGridStartNode = startNode;
                            reaction.PathGridEndNode = endNode;
                        }
                        break;
                    case "walkto":
                        {
                            reaction.ReactionType = QuestReactionType.WalkTo;
                            reaction.CreatureIsSelf = true;
                            reaction.MovementIsRun = reactionValue1 == "run";
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

                // An option can be held back until the player has finished a quest, and until the creature is standing on a particular path grid node
                string requiredQuestString = columns["required_quest_wowid"].Trim();
                if (requiredQuestString.Length > 0)
                    reaction.RequiredQuestID = int.Parse(requiredQuestString);
                string requiredNodeString = columns["required_at_grid_node"].Trim();
                if (requiredNodeString.Length > 0)
                {
                    string[] gridAndNode = requiredNodeString.Split(':');
                    int requiredGridID;
                    int requiredNodeNumber;
                    if (gridAndNode.Length != 2 || int.TryParse(gridAndNode[0].Trim(), out requiredGridID) == false || int.TryParse(gridAndNode[1].Trim(), out requiredNodeNumber) == false)
                        Logger.WriteError(string.Concat("Unreadable required_at_grid_node value of '", requiredNodeString, "'"));
                    else
                    {
                        reaction.RequiredAtGridID = requiredGridID;
                        reaction.RequiredAtGridNode = requiredNodeNumber;
                    }
                }

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

                // Rows that follow a walkto for the same menu option are deferred until the creature reaches the destination
                (string, string, int) optionKey = (reaction.ZoneShortName, reaction.CreatureName, reaction.OptionID);
                reaction.FiresOnArrival = optionsAlreadyWalking.Contains(optionKey);
                if (reaction.ReactionType == QuestReactionType.WalkTo || reaction.ReactionType == QuestReactionType.WalkGrid)
                    optionsAlreadyWalking.Add(optionKey);

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
