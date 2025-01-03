﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

using EQWOWConverter.ObjectModels.Properties;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Creatures
{
    internal class CreatureRace
    {
        private static Dictionary<int, CreatureRace> CreatureRacesByRaceID = new Dictionary<int, CreatureRace>();

        public int ID;
        public string Name = string.Empty;
        public string MaleSkeletonName = string.Empty;
        public string MaleAnimationSupplementName = string.Empty;
        public string FemaleSkeletonName = string.Empty;
        public string FemaleAnimationSupplementName = string.Empty;
        public string NeutralSkeletonName = string.Empty;
        public string NeutralAnimationSupplementName = string.Empty;
        public float BoundaryRadius;
        public float BoundaryHeight;
        public float LiftMaleAndNeutral = 0;
        public float LiftFemale = 0;
        public float ModelScale = 1;
        public float Height = 1;
        public float SpawnSizeMod = 0.2f;

        public string GetAnimationSupplementNameForGender(CreatureGenderType genderType)
        {
            switch (genderType)
            {
                case CreatureGenderType.Male: return MaleAnimationSupplementName;
                case CreatureGenderType.Female: return FemaleAnimationSupplementName;
                case CreatureGenderType.Neutral: return NeutralAnimationSupplementName;
                default: return string.Empty;
            }
        }

        public string GetSkeletonNameForGender(CreatureGenderType genderType)
        {
            if (genderType == CreatureGenderType.Male && MaleSkeletonName != string.Empty)
                return MaleSkeletonName;
            if (genderType == CreatureGenderType.Female && FemaleSkeletonName != string.Empty)
                return FemaleSkeletonName;
            if (genderType == CreatureGenderType.Neutral && NeutralSkeletonName != string.Empty)
                return NeutralSkeletonName;
            if (NeutralSkeletonName != string.Empty)
                return NeutralSkeletonName;
            if (MaleSkeletonName != string.Empty)
                return MaleSkeletonName;
            else
                return FemaleSkeletonName;
        }

        public bool HasSkeletonName()
        {
            if (MaleSkeletonName != string.Empty)
                return true;
            if (FemaleSkeletonName != string.Empty)
                return true;
            if (NeutralSkeletonName != string.Empty)
                return true;
            return false;
        }

        public float GetLiftHeightForGender(CreatureGenderType genderType)
        {
            switch (genderType)
            {
                case CreatureGenderType.Female: return LiftFemale;
                default: return LiftMaleAndNeutral;
            }
        }

        public static Dictionary<int, CreatureRace> GetAllCreatureRacesByID()
        {
            if (CreatureRacesByRaceID.Count == 0)
                PopulateCreatureRaceList();
            return CreatureRacesByRaceID;
        }

        private static void PopulateCreatureRaceList()
        {
            CreatureRacesByRaceID.Clear();

            // Load in base race data
            string raceDataFileName = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureRaces.csv");
            Logger.WriteDetail("Populating CreatureRace list via file '" + raceDataFileName + "'");
            string inputData = File.ReadAllText(raceDataFileName);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("CreatureRace list via file '" + raceDataFileName + "' did not have enough lines");
                return;
            }

            // Load all of the data
            bool headerRow = true;
            foreach(string row in inputRows)
            {
                // Handle first row
                if (headerRow == true)
                {
                    headerRow = false;
                    continue;
                }

                // Skip blank rows
                if (row.Trim().Length == 0)
                    continue;

                // Load the row
                string[] rowBlocks = row.Split("|");
                CreatureRace newCreatureRace = new CreatureRace();
                newCreatureRace.ID = int.Parse(rowBlocks[0]);
                newCreatureRace.Name = rowBlocks[1].Trim();
                newCreatureRace.MaleSkeletonName = rowBlocks[2].Trim();
                newCreatureRace.FemaleSkeletonName = rowBlocks[3].Trim();
                newCreatureRace.NeutralSkeletonName = rowBlocks[4].Trim();
                newCreatureRace.LiftMaleAndNeutral = float.Parse(rowBlocks[5]);
                newCreatureRace.LiftFemale = float.Parse(rowBlocks[6]);
                newCreatureRace.ModelScale = float.Parse(rowBlocks[7]);
                newCreatureRace.Height = float.Parse(rowBlocks[8]);
                newCreatureRace.SpawnSizeMod = float.Parse(rowBlocks[9]);
                newCreatureRace.BoundaryRadius = float.Parse(rowBlocks[10]);
                newCreatureRace.BoundaryHeight = float.Parse(rowBlocks[11]);
                if (rowBlocks.Length > 11)
                    newCreatureRace.MaleAnimationSupplementName = rowBlocks[12].Trim();
                if (rowBlocks.Length > 12)
                    newCreatureRace.FemaleAnimationSupplementName = rowBlocks[13].Trim();
                if (rowBlocks.Length > 13)
                    newCreatureRace.NeutralAnimationSupplementName = rowBlocks[14].Trim();

                if (CreatureRacesByRaceID.ContainsKey(newCreatureRace.ID))
                {
                    Logger.WriteError("CreatureRace list via file '" + raceDataFileName + "' has an duplicate row with id '" + newCreatureRace.ID + "'");
                    continue;
                }
                CreatureRacesByRaceID.Add(newCreatureRace.ID, newCreatureRace);
            }
        }
    }
}
