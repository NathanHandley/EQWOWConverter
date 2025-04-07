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

namespace EQWOWConverter
{
    internal class LogCounter
    {
        private int StartCursorLeft = 0;
        private int StartCursorTop = 0;
        private int StartProgress = 0;
        private int CurProgress = 0;
        private int TotalNumber = 0;
        private static readonly object writeLock = new object();

        public LogCounter(string counterMessage, int startProgress = 0, int totalNumber = 0)
        {
            lock (writeLock)
            {
                StartProgress = startProgress;
                TotalNumber = totalNumber;
                CurProgress = StartProgress;
                Logger.WriteInfo(counterMessage, false);
                StartCursorLeft = Console.CursorLeft;
                StartCursorTop = Console.CursorTop;
                Logger.WriteInfo(string.Empty, true, false);
            }
        }

        public void AddToProgress(int numToAdd)
        {
            lock (writeLock)
            {
                CurProgress += numToAdd;
            }
        }

        public void SetProgress(int curProgress)
        {
            lock (writeLock)
            {
                CurProgress = curProgress;
            }
        }

        public void Write(int numToAddToProgress = 0)
        {
            lock (writeLock)
            {
                if (numToAddToProgress != 0)
                    CurProgress += numToAddToProgress;

                string outputString;
                if (TotalNumber != 0)
                    outputString = "(" + CurProgress.ToString() + " of " + TotalNumber.ToString() + ")";
                else
                    outputString = "(" + CurProgress.ToString() + ")";
                Logger.WriteForCounter(outputString, StartCursorLeft, StartCursorTop);
            }
        }
    }
}
