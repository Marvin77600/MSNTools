using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.ConsoleCommands
{
    public class ReadResetRegions : MSNConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (File.Exists(Config.ConfigPath + "/RR.txt"))
                {
                    string[] regions = File.ReadAllLines(Config.ConfigPath + "/RR.txt");
                    PersistentData.PersistentContainer.Instance.RegionsReset = regions.ToList();
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Reset regions list have been updated");
                }
                else
                {
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output("File RR.txt doesn't exist");
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "readrr" };

        public override string GetDescription() => "Read reset regions from a text file and save these on PersistentData.";

        public override string GetHelp() => GetDescription() + "\nUsage:\n   readrr\nCan be used to read reset regions from a text file and save these on PersistentData.";
    }
}
