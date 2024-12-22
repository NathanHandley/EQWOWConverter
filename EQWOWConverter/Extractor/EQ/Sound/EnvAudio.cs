using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using EalTools;

namespace LanternExtractor.EQ.Sound
{
    public class EnvAudio
    {
        public static EnvAudio Instance { get; } = new EnvAudio();

        public EalData Data { get; private set; }

        private bool _loaded;
        private string _ealFilePath;
        private EalFile _ealFile;
        private Dictionary<string, int> _sourceLevels;

        static EnvAudio()
        {
        }

        private EnvAudio()
        {
        }

        public bool Load(string ealFilePath)
        {
            if (_ealFilePath == ealFilePath)
            {
                return _loaded;
            }

            if (ealFilePath == null || !File.Exists(ealFilePath))
            {
                return false;
            }

            _ealFilePath = ealFilePath;

            // Allow other threads to open the same file for reading
            _ealFile = new EalFile(new FileStream(
                _ealFilePath, FileMode.Open, FileAccess.Read, FileShare.Read));

            if (!_ealFile.Initialize())
            {
                return false;
            }

            Data = _ealFile.Data;

            if (Data == null)
            {
                return false;
            }

            _sourceLevels = Data.SourceModels.ToDictionary(
                s => Path.GetFileNameWithoutExtension(s.Name).ToLower(),
                s => s.SourceAttributes.EaxAttributes.DirectPathLevel
            );

            _loaded = true;

            return _loaded;
        }

        private int GetVolumeEq(string soundFile)
        {
            var volume = 0;
            _sourceLevels?.TryGetValue(soundFile, out volume);
            return volume;
        }

        public float GetVolumeLinear(string soundFile)
        {
            var volumeEq = GetVolumeEq(soundFile);
            return GetVolumeLinear(volumeEq);
        }

        public float GetVolumeLinear(int directAudioLevel)
        {
            var linear = MathF.Pow(10.0f, directAudioLevel / 2000.0f);
            return Math.Clamp(linear, 0f, 1f);
        }
    }
}
