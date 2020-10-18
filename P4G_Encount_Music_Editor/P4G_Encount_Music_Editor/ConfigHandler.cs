﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace P4G_Encount_Music_Editor
{
    class ConfigHandler
    {
        private string currentDir = null;
        private string patchFilePath = null;
        private string randomSetsFilePath = null;

        public ConfigHandler()
        {
            currentDir = Directory.GetCurrentDirectory();
            patchFilePath = $@"{currentDir}\original\BGME_Config.patch";
            randomSetsFilePath = $@"{currentDir}\presets\RandomSets.bgme";
        }

        public void RebuildPatch()
        {
            string newPatchFile = $@"{currentDir}\BGME_Config.patch";
            try
            {
                byte[] patchBytes = File.ReadAllBytes(patchFilePath);
                int startOffset = 134;

                string[] randomSetsLines = File.ReadAllLines(randomSetsFilePath);

                foreach (string line in randomSetsLines)
                {
                    // skip pointless lines
                    if (line.StartsWith('/') || line.StartsWith('#') || line.Length < 1)
                        continue;

                    // split line to parts
                    string[] lineParts = line.Split('=');

                    // parse rand set index
                    int randSetIndex = Int32.Parse(lineParts[0]);
                    // parse min and max song indexes
                    ushort minIndex = UInt16.Parse(lineParts[1].Split(',')[0]);
                    ushort maxIndex = UInt16.Parse(lineParts[1].Split(',')[1]);

                    // convert min and max to bytes
                    byte[] minBytes = BitConverter.GetBytes(minIndex);
                    byte[] maxBytes = BitConverter.GetBytes(maxIndex);

                    // display
                    Console.WriteLine($"Random Set: {randSetIndex}");
                    Console.WriteLine($"Min Index: {minIndex} Bytes: {BitConverter.ToString(minBytes)}");
                    Console.WriteLine($"Max Index: {maxIndex} Bytes: {BitConverter.ToString(maxBytes)}");

                    // copy min and max bytes to main patch bytes array
                    Array.Copy(minBytes, 0, patchBytes, startOffset + 4 * randSetIndex, minBytes.Length);
                    Array.Copy(maxBytes, 0, patchBytes, startOffset + 2 + 4 * randSetIndex, maxBytes.Length);
                    Console.WriteLine("Copied min and max bytes!");
                }

                File.WriteAllBytes($"{newPatchFile}", patchBytes);
                Console.WriteLine($"New BGME_Config Patch Created! File: {newPatchFile}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Problem rebuilding BGME_Config patch!");
            }
        }
    }
}