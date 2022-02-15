using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.Harmony
{
    [HarmonyPatch(typeof(RegionFile), "Get")]
    [HarmonyPatch(new Type[] { typeof(string), typeof(int), typeof(int)  })]
    public class PatchGetRegionFile
    {
        static bool Prefix(RegionFile __result, string dir, int rX, int rZ, ref string ___EXT, ref byte[] ___FileHeaderMagicBytes)
        {
            string str = string.Format("{0}/r.{1}.{2}.{3}", dir, rX, rZ, ___EXT);
            if (!File.Exists(str))
            {
                File.Create(str).Close();
                __result = new RegionFileV2(str, rX, rZ, null, 1);
            }
            FileStream _fileStream = File.Open(str, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            byte[] buffer = new byte[3];
            _fileStream.Read(buffer, 0, 3);
            for (int index = 0; index < 3; ++index)
            {
                if (buffer[index] != ___FileHeaderMagicBytes[index])
                {
                    _fileStream.Close();
                    File.Delete(str);
                    MSNUtils.LogError($"Corrupt region file found! Region has been removed: r.{rX}.{rZ}.7rg");
                    MSNUtils.Log($"Creating new RegionFileV2: r.{rX}.{rZ}.7rg");
                    File.Create(str).Close();
                    __result = new RegionFileV2(str, rX, rZ, null, 1);
                }
            }
            int _version = (byte)_fileStream.ReadByte();
            __result = _version < 1 ? new RegionFileV1(str, rX, rZ, _fileStream, _version) : (RegionFile)new RegionFileV2(str, rX, rZ, _fileStream, _version);
            return false;
        }
    }
}