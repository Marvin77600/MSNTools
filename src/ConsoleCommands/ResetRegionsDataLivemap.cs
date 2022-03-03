using AllocsFixes.JSON;
using MSNTools.PersistentData;
using System;
using System.Collections.Generic;

namespace MSNTools.ConsoleCommands
{
    public class ResetRegionsDataLivemap : MSNConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                var list = PersistentContainer.Instance.RegionsReset;
                JSONObject jSONObject = new JSONObject();
                for (int i = 0; i < list.Count; i++)
                {
                    var region = list[i];
                    jSONObject.Add($"{i}", new JSONString(region));
                }
                SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{jSONObject}");
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Exception " + e.Message);
            }
        }
        
        public override string[] GetCommands() => new string[] { "rrdl" };

        public override string GetDescription() => "Get the reset regions list in JSON format.";

        public override string GetHelp() => GetDescription() + "\nUsage:\n   rrdl";
    }
}