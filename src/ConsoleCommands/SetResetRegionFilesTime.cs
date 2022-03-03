using MSNTools.PersistentData;
using System;
using System.Collections.Generic;

namespace MSNTools.ConsoleCommands
{
    public class SetResetRegionFilesTime : MSNConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count < 6)
                {
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Wrong number of arguments, expected 6, found {_params.Count}.");
                }
                else
                {
                    int year = int.Parse(_params[0]);
                    int month = int.Parse(_params[1]);
                    int day = int.Parse(_params[2]);
                    int hour = int.Parse(_params[3]);
                    int minute = int.Parse(_params[4]);
                    int second = int.Parse(_params[5]);

                    DateTime nextResetRegionFilesTime = new DateTime(year, month, day, hour, minute, second);
                    PersistentContainer.Instance.TimeRegionFiles = nextResetRegionFilesTime;
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output(nextResetRegionFilesTime.ToString());
                    PersistentContainer.DataChange = true;
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "srrft" };

        public override string GetDescription() => "srrft year month day hour minute";
    }
}
