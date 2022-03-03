using MSNTools.PersistentData;
using System;
using System.Collections.Generic;

namespace MSNTools.ConsoleCommands
{
    public class GetResetRegionFilesTime : MSNConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                DateTime time = PersistentContainer.Instance.TimeRegionFiles;
                SingletonMonoBehaviour<SdtdConsole>.Instance.Output(time.ToString());
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "grrft" };

        public override string GetDescription() => "Get next reset region time.";

        public override string GetHelp() => GetDescription() + "\nUsage:\n   grrft";
    }
}