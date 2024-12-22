using System.Collections.Generic;
using System.IO;
using LanternExtractor.Infrastructure;

namespace LanternExtractor.EQ.Sound
{
    /// <summary>
    /// EffSndBnk files are plain text files ending with "_sndbnk" which list the sounds in each zone.
    /// The indices of these sounds are used by the sound entries to determine which sound should play.
    /// </summary>
    public class EffSndBnk
    {
        private List<string> _emitSounds = new List<string>();
        private List<string> _loopSounds = new List<string>();
        private string _soundFilePath;
        
        public EffSndBnk(string soundFilePath)
        {
            _soundFilePath = soundFilePath;
        }
        
        public void Initialize()
        {
            if (!File.Exists(_soundFilePath))
            {
                return;
            }
            
            List<string> currentList = null;

            string fileText = File.ReadAllText(_soundFilePath);
            List<string> parsedLines = TextParser.ParseTextByNewline(fileText);

            if (parsedLines == null || parsedLines.Count == 0)
            {
                return;
            }

            foreach (var line in parsedLines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                switch (line)
                {
                    case SoundConstants.Emit:
                        currentList = _emitSounds;
                        continue;
                    case SoundConstants.Loop:
                        currentList = _loopSounds;
                        continue;
                    default:
                        currentList?.Add(line);
                        break;
                }
            }
        }

        private string GetValueFromList(int index, ref List<string> list)
        {
            if (index < 0 || index >= list.Count)
            {
                return SoundConstants.Unknown;
            }

            return list[index];
        }

        public string GetEmitSound(int index)
        {
            return GetValueFromList(index, ref _emitSounds);
        }

        public string GetLoopSound(int index)
        {
            return GetValueFromList(index, ref _loopSounds);
        }
    }
}