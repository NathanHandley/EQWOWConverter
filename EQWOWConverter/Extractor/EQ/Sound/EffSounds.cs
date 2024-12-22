using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Sound
{
    /// <summary>
    /// The EffSound file is binary binary ending with "_sounds.eff" extension.
    /// It contains no header, just an array of entries each consisting of 84 bytes.
    /// Each entry describes an instance of a sound or music in the world.
    /// </summary>
    public class EffSounds
    {
        public static int EntryLengthInBytes = 84;

        /// <summary>
        /// The sound bank referenced for sound names
        /// </summary>
        private readonly EffSndBnk _soundBank;

        private readonly EnvAudio _envAudio;
        private readonly string _soundFilePath;
        private readonly List<AudioInstance> _audioInstances = new List<AudioInstance>();

        public EffSounds(string soundFilePath, EffSndBnk soundBank, EnvAudio envAudio)
        {
            _soundFilePath = soundFilePath;
            _soundBank = soundBank;
            _envAudio = envAudio;
        }

        public void Initialize(ILogger logger)
        {
            if (_soundBank == null || !File.Exists(_soundFilePath))
            {
                return;
            }

            var file = File.Open(_soundFilePath, FileMode.Open);
            var reader = new BinaryReader(file);
            int fileLength = (int)reader.BaseStream.Length;

            if (fileLength % EntryLengthInBytes != 0)
            {
                logger.LogError($"Invalid .eff file - size must be multiple of {EntryLengthInBytes}");
                return;
            }

            int entryCount = fileLength / EntryLengthInBytes;

            for (int i = 0; i < entryCount; ++i)
            {
                var basePosition = EntryLengthInBytes * i;
                reader.BaseStream.Position = basePosition + 16;
                float posX = reader.ReadSingle();
                float posY = reader.ReadSingle();
                float posZ = reader.ReadSingle();
                float radius = reader.ReadSingle();

                reader.BaseStream.Position = basePosition + 56;

                var typeByte = reader.ReadByte();

                if (!Enum.IsDefined(typeof(AudioType), typeByte))
                {
                    logger.LogError($"Unable to parse sound type: {typeByte}");
                    continue;
                }

                var type = (AudioType)typeByte;
                reader.BaseStream.Position = basePosition + 48;
                int soundId1 = reader.ReadInt32();
                string sound1 = GetSoundName(soundId1);

                if (type == AudioType.Music)
                {
                    int soundId2 = reader.ReadInt32();
                    reader.BaseStream.Position = basePosition + 60;
                    int loopCountDay = reader.ReadInt32();
                    int loopCountNight = reader.ReadInt32();
                    int fadeOutMs = reader.ReadInt32();
                    var musicInstance = new MusicInstance(type, posX, posY, posZ, radius, soundId1, soundId2,
                        loopCountDay, loopCountNight, fadeOutMs);
                    _audioInstances.Add(musicInstance);
                }
                else if (type == AudioType.Sound2d)
                {
                    int soundId2 = reader.ReadInt32();
                    string sound2 = GetSoundName(soundId2);
                    reader.BaseStream.Position = basePosition + 32;
                    int cooldown1 = reader.ReadInt32();
                    int cooldown2 = reader.ReadInt32();
                    int cooldownRandom = reader.ReadInt32();
                    reader.BaseStream.Position = basePosition + 60;
                    int volume1Raw = reader.ReadInt32();
                    float volume1 = GetEalSoundVolume(sound1, volume1Raw);
                    int volume2Raw = reader.ReadInt32();
                    float volume2 = GetEalSoundVolume(sound2, volume2Raw);
                    var soundInstance = new SoundInstance2d(type, posX, posY, posZ, radius, volume1, sound1, cooldown1,
                        sound2, cooldown2, cooldownRandom, volume2);
                    _audioInstances.Add(soundInstance);
                }
                else
                {
                    reader.BaseStream.Position = basePosition + 32;
                    int cooldown1 = reader.ReadInt32();
                    reader.BaseStream.Position = basePosition + 40;
                    int cooldownRandom = reader.ReadInt32();
                    reader.BaseStream.Position = basePosition + 60;
                    int volumeRaw = reader.ReadInt32();
                    float volume = GetEalSoundVolume(sound1, volumeRaw);
                    reader.BaseStream.Position = basePosition + 72;
                    int multiplier = reader.ReadInt32();
                    var soundInstance = new SoundInstance3d(type, posX, posY, posZ, radius, volume, sound1,
                        cooldown1, cooldownRandom, multiplier);
                    _audioInstances.Add(soundInstance);
                }
            }
        }

        private string GetSoundName(int soundId)
        {
            var emissionType = GetEmissionType(soundId);
            switch (emissionType)
            {
                case EmissionType.None:
                    return string.Empty;
                case EmissionType.Emit:
                    return _soundBank.GetEmitSound(soundId - 1);
                case EmissionType.Loop:
                    return _soundBank.GetLoopSound(soundId - 162);
                case EmissionType.Internal:
                    return ClientSounds.GetClientSound(soundId);
                default:
                    return SoundConstants.Unknown;
            }
        }

        private float GetEalSoundVolume(string soundName, int volumeRaw)
        {
            if (volumeRaw > 0)
            {
                volumeRaw = -volumeRaw;
            }

            return volumeRaw == 0 ? _envAudio.GetVolumeLinear(soundName) : _envAudio.GetVolumeLinear(volumeRaw);
        }

        private EmissionType GetEmissionType(int soundId)
        {
            if (soundId <= 0)
            {
                return EmissionType.None;
            }

            if (soundId < 32)
            {
                return EmissionType.Emit;
            }

            return soundId < 162 ? EmissionType.Internal : EmissionType.Loop;
        }

        public void ExportSoundData(string zoneName, string rootFolder)
        {
            var sound2dExport = new StringBuilder();
            var sound3dExport = new StringBuilder();
            var musicExport = new StringBuilder();

            foreach (var entry in _audioInstances)
            {
                if (entry.AudioType == AudioType.Music)
                {
                    if (!(entry is MusicInstance music))
                    {
                        continue;
                    }

                    musicExport.AppendLine(string.Join(",", music.PosX, music.PosZ, music.PosY, music.Radius,
                        music.TrackIndexDay, music.TrackIndexNight, music.LoopCountDay, music.LoopCountNight,
                        music.FadeOutMs));
                }
                else if (entry.AudioType == AudioType.Sound2d)
                {
                    if (!(entry is SoundInstance2d sound2d))
                    {
                        continue;
                    }

                    sound2dExport.AppendLine(string.Join(",", sound2d.PosX, sound2d.PosZ,
                        sound2d.PosY, sound2d.Radius, sound2d.Sound1, sound2d.Sound2,
                        sound2d.Cooldown1, sound2d.Cooldown2, sound2d.CooldownRandom, sound2d.Volume1, sound2d.Volume2));
                }
                else
                {
                    if (!(entry is SoundInstance3d sound3d))
                    {
                        continue;
                    }

                    sound3dExport.AppendLine(string.Join(",", sound3d.PosX, sound3d.PosZ,
                        sound3d.PosY, sound3d.Radius, sound3d.Sound1, sound3d.Cooldown1, sound3d.CooldownRandom, sound3d.Volume1,
                        sound3d.Multiplier));
                }
            }

            string exportPath = Path.Combine(rootFolder, zoneName, "Zone/");

            if (musicExport.Length > 0)
            {
                StringBuilder exportHeader = new StringBuilder();
                exportHeader.AppendLine(LanternStrings.ExportHeaderTitle + "Music Instances");
                exportHeader.AppendLine(
                    "# Format: PosX, PosY, PosZ, Radius, MusicIndexDay, MusicIndexNight, LoopCountDay, LoopCountNight, FadeOutMs");
                Directory.CreateDirectory(exportPath);
                File.WriteAllText(exportPath + "music_instances.txt", exportHeader.ToString() + musicExport);
            }

            if (sound2dExport.Length > 0)
            {
                StringBuilder exportHeader = new StringBuilder();
                exportHeader.AppendLine(LanternStrings.ExportHeaderTitle + "Sound 2D Instances");
                exportHeader.AppendLine(
                    "# Format: PosX, PosY, PosZ, Radius, SoundNameDay, SoundNameNight, CooldownDay, CooldownNight, CooldownRandom, VolumeDay, VolumeNight");
                Directory.CreateDirectory(exportPath);
                File.WriteAllText(exportPath + "sound2d_instances.txt", exportHeader.ToString() + sound2dExport);
            }

            if (sound3dExport.Length > 0)
            {
                StringBuilder exportHeader = new StringBuilder();
                exportHeader.AppendLine(LanternStrings.ExportHeaderTitle + "Sound 3D Instances");
                exportHeader.AppendLine(
                    "# Format: PosX, PosY, PosZ, Radius, SoundName, Cooldown, CooldownRandom, Volume, Multiplier");
                Directory.CreateDirectory(exportPath);
                File.WriteAllText(exportPath + "sound3d_instances.txt", exportHeader.ToString() + sound3dExport);
            }
        }
    }
}