using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using System.IO;

namespace MSNTools
{
    public class ResetRegions
    {
        public static bool IsEnabled = true;
        public static int Hour = 0;
        public static int Day = 0;

        public static void Exec()
        {
            try
            {
                List<string> regions = PersistentContainer.Instance.RegionsReset;
                DateTime nextResetRegionFilesTime = PersistentContainer.Instance.TimeRegionFiles;
                if (DateTime.Now >= nextResetRegionFilesTime)
                {
                    if (regions.Count > 0)
                    {
                        string regionDir = GameIO.GetSaveGameRegionDir();
                        if (Directory.Exists(regionDir))
                        {
                            string[] files = Directory.GetFiles(regionDir, "*.7rg");
                            if (files != null && files.Length > 0)
                            {
                                foreach (var file in files)
                                {
                                    string fileName = file.Remove(0, file.IndexOf("Region") + 9).Replace(".7rg", "");
                                    if (regions.Contains(fileName))
                                    {
                                        FileInfo fInfo = new FileInfo(file);
                                        if (fInfo != null)
                                        {
                                            fInfo.Delete();
                                            MSNUtils.Log($"Region reset: r.{fileName}.7rg");
                                        }
                                    }
                                }
                                PersistentContainer.Instance.TimeRegionFiles = new DateTime(nextResetRegionFilesTime.Year, nextResetRegionFilesTime.Month, nextResetRegionFilesTime.Day + Day);
                                PersistentContainer.DataChange = true;
                                MSNUtils.Log($"Next Reset Regions : {nextResetRegionFilesTime}");
                            }
                        }
                        else
                        {
                            MSNUtils.LogWarning($"Region directory not found. Unable to delete regions from the reset list");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in ResetRegions.Exec: {e.Message}");
            }
        }
    }
}