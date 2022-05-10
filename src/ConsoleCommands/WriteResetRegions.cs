using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MSNTools.ConsoleCommands
{
    public class WriteResetRegions : MSNConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                List<string> regions = PersistentData.PersistentContainer.Instance.RegionsReset;

                using (StreamWriter sw = new StreamWriter(Config.ConfigPath +"/RR.txt", false, Encoding.UTF8))
                {
                    foreach (var region in regions)
                    {
                        sw.WriteLine(region);
                    }
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "writerr" };

        public override string GetDescription() => "Write reset regions on a text file.";

        public override string GetHelp() => GetDescription() + "\nUsage:\n   writerr\nCan be used to write reset regions to a text file.";
    }
}