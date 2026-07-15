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

namespace EQWOWConverter
{
    internal class WavTool
    {
        private class WavPCMData
        {
            public byte[] FrameData = new byte[0];
            public UInt32 SampleRate;
            public UInt16 ChannelCount;
            public UInt16 BitsPerSample;
            public int BytesPerFrame;
            public int FrameCount;
        }

        // Returns the play length of a PCM wav file in milliseconds, or 0 if it could not be read
        public static int GetWavFileDurationInMS(string filePath)
        {
            WavPCMData? wavData = ReadPCMWavFile(filePath);
            if (wavData == null)
                return 0;
            return Convert.ToInt32((Convert.ToInt64(wavData.FrameCount) * 1000) / wavData.SampleRate);
        }

        // Returns the durations (in milliseconds) of the sequential pieces the wav file divides into, where every piece is at most (about) targetPieceDurationMS long
        // The same cut points are used by WriteWavFilePieces
        public static List<int> CalculateWavFilePieceDurationsInMS(string filePath, int targetPieceDurationMS)
        {
            List<int> pieceDurationsMS = new List<int>();
            WavPCMData? wavData = ReadPCMWavFile(filePath);
            if (wavData == null)
                return pieceDurationsMS;
            List<int> cutFrames = CalculatePieceCutFrames(wavData, targetPieceDurationMS);
            for (int i = 0; i < cutFrames.Count - 1; i++)
                pieceDurationsMS.Add(Convert.ToInt32((Convert.ToInt64(cutFrames[i + 1] - cutFrames[i]) * 1000) / wavData.SampleRate));
            return pieceDurationsMS;
        }

        // Splits a PCM wav file into sequential pieces named '<outputFileNameNoExtBase>Piece<number>.wav', cut at the same points as CalculateWavFilePieceDurationsInMS so the pieces play back-to-back as the original recording
        public static bool WriteWavFilePieces(string inputFilePath, string outputFolderPath, string outputFileNameNoExtBase, int targetPieceDurationMS)
        {
            WavPCMData? wavData = ReadPCMWavFile(inputFilePath);
            if (wavData == null)
                return false;
            List<int> cutFrames = CalculatePieceCutFrames(wavData, targetPieceDurationMS);
            for (int i = 0; i < cutFrames.Count - 1; i++)
            {
                int startFrame = cutFrames[i];
                int frameCount = cutFrames[i + 1] - cutFrames[i];
                byte[] pieceFrameData = new byte[frameCount * wavData.BytesPerFrame];
                Array.Copy(wavData.FrameData, startFrame * wavData.BytesPerFrame, pieceFrameData, 0, pieceFrameData.Length);
                string outputFilePath = Path.Combine(outputFolderPath, string.Concat(outputFileNameNoExtBase, "Piece", (i + 1).ToString(), ".wav"));
                WritePCMWavFile(outputFilePath, pieceFrameData, wavData.SampleRate, wavData.ChannelCount, wavData.BitsPerSample);
            }
            return true;
        }

        // Pieces cut at even divisions would click at the seams if playback timing jitters, so each interior cut point shifts to the nearest zero crossing (where the waveform passes silence) within a small window
        private static List<int> CalculatePieceCutFrames(WavPCMData wavData, int targetPieceDurationMS)
        {
            int targetPieceFrames = Convert.ToInt32((Convert.ToInt64(targetPieceDurationMS) * wavData.SampleRate) / 1000);
            if (targetPieceFrames <= 0)
                targetPieceFrames = 1;
            int pieceCount = (wavData.FrameCount + targetPieceFrames - 1) / targetPieceFrames;
            if (pieceCount < 1)
                pieceCount = 1;
            List<int> cutFrames = new List<int>();
            cutFrames.Add(0);
            int zeroCrossingSearchWindowFrames = Convert.ToInt32(wavData.SampleRate * 40 / 1000); // 40ms window each direction
            for (int i = 1; i < pieceCount; i++)
            {
                int nominalCutFrame = Convert.ToInt32((Convert.ToInt64(wavData.FrameCount) * i) / pieceCount);
                cutFrames.Add(FindNearestZeroCrossingFrame(wavData, nominalCutFrame, zeroCrossingSearchWindowFrames));
            }
            cutFrames.Add(wavData.FrameCount);
            return cutFrames;
        }

        private static int FindNearestZeroCrossingFrame(WavPCMData wavData, int nominalFrame, int searchWindowFrames)
        {
            int bestFrame = nominalFrame;
            int bestAmplitude = int.MaxValue;
            for (int offset = 0; offset <= searchWindowFrames; offset++)
            {
                for (int direction = 0; direction < 2; direction++)
                {
                    if (offset == 0 && direction == 1)
                        continue;
                    int curFrame = direction == 0 ? nominalFrame + offset : nominalFrame - offset;
                    if (curFrame <= 0 || curFrame >= wavData.FrameCount)
                        continue;
                    int curAmplitude = Math.Abs(GetFirstChannelSampleAmplitude(wavData, curFrame));
                    if (curAmplitude < bestAmplitude)
                    {
                        bestAmplitude = curAmplitude;
                        bestFrame = curFrame;
                        if (bestAmplitude == 0)
                            return bestFrame;
                    }
                }
            }
            return bestFrame;
        }

        private static int GetFirstChannelSampleAmplitude(WavPCMData wavData, int frame)
        {
            int sampleIndex = frame * wavData.BytesPerFrame;
            if (wavData.BitsPerSample == 8)
                return wavData.FrameData[sampleIndex] - 128; // 8-bit PCM is unsigned with a 128 center point
            else
                return BitConverter.ToInt16(wavData.FrameData, sampleIndex);
        }

        private static WavPCMData? ReadPCMWavFile(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                Logger.WriteError("Could not read wav file '" + filePath + "' as the file does not exist");
                return null;
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            if (fileBytes.Length < 44 || fileBytes[0] != 'R' || fileBytes[1] != 'I' || fileBytes[2] != 'F' || fileBytes[3] != 'F' ||
                fileBytes[8] != 'W' || fileBytes[9] != 'A' || fileBytes[10] != 'V' || fileBytes[11] != 'E')
            {
                Logger.WriteError("Could not read wav file '" + filePath + "' as it is not a RIFF WAVE file");
                return null;
            }

            // Walk the chunks to find 'fmt ' and 'data'
            UInt16 formatType = 0;
            WavPCMData wavData = new WavPCMData();
            int dataStartIndex = -1;
            int dataLength = 0;
            int byteCursor = 12;
            while (byteCursor + 8 <= fileBytes.Length)
            {
                string chunkID = string.Concat((char)fileBytes[byteCursor], (char)fileBytes[byteCursor + 1], (char)fileBytes[byteCursor + 2], (char)fileBytes[byteCursor + 3]);
                int chunkSize = BitConverter.ToInt32(fileBytes, byteCursor + 4);
                if (chunkID == "fmt ")
                {
                    formatType = BitConverter.ToUInt16(fileBytes, byteCursor + 8);
                    wavData.ChannelCount = BitConverter.ToUInt16(fileBytes, byteCursor + 10);
                    wavData.SampleRate = BitConverter.ToUInt32(fileBytes, byteCursor + 12);
                    wavData.BitsPerSample = BitConverter.ToUInt16(fileBytes, byteCursor + 22);
                }
                else if (chunkID == "data")
                {
                    dataStartIndex = byteCursor + 8;
                    dataLength = Math.Min(chunkSize, fileBytes.Length - dataStartIndex);
                }
                byteCursor += 8 + chunkSize;
                if (chunkSize % 2 == 1)
                    byteCursor++;
            }
            if (formatType != 1 || dataStartIndex == -1 || wavData.ChannelCount == 0 || wavData.SampleRate == 0 ||
                (wavData.BitsPerSample != 8 && wavData.BitsPerSample != 16))
            {
                Logger.WriteError("Could not read wav file '" + filePath + "' as only 8-bit and 16-bit PCM with fmt and data chunks is supported");
                return null;
            }
            wavData.BytesPerFrame = wavData.ChannelCount * (wavData.BitsPerSample / 8);
            wavData.FrameCount = dataLength / wavData.BytesPerFrame;
            wavData.FrameData = new byte[wavData.FrameCount * wavData.BytesPerFrame];
            Array.Copy(fileBytes, dataStartIndex, wavData.FrameData, 0, wavData.FrameData.Length);
            return wavData;
        }

        private static void WritePCMWavFile(string filePath, byte[] frameData, UInt32 sampleRate, UInt16 channelCount, UInt16 bitsPerSample)
        {
            int bytesPerFrame = channelCount * (bitsPerSample / 8);
            List<byte> outputBytes = new List<byte>();
            outputBytes.AddRange(new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' });
            outputBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(36 + frameData.Length)));
            outputBytes.AddRange(new byte[] { (byte)'W', (byte)'A', (byte)'V', (byte)'E' });
            outputBytes.AddRange(new byte[] { (byte)'f', (byte)'m', (byte)'t', (byte)' ' });
            outputBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(16)));
            outputBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(1))); // PCM
            outputBytes.AddRange(BitConverter.GetBytes(channelCount));
            outputBytes.AddRange(BitConverter.GetBytes(sampleRate));
            outputBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(sampleRate * Convert.ToUInt32(bytesPerFrame)))); // Byte rate
            outputBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(bytesPerFrame))); // Block align
            outputBytes.AddRange(BitConverter.GetBytes(bitsPerSample));
            outputBytes.AddRange(new byte[] { (byte)'d', (byte)'a', (byte)'t', (byte)'a' });
            outputBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(frameData.Length)));
            outputBytes.AddRange(frameData);
            File.WriteAllBytes(filePath, outputBytes.ToArray());
        }
    }
}
