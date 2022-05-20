using MSNTools.PersistentData;
using BankChatCommand = MSNTools.Bank;
using System;
using System.Collections.Generic;

namespace MSNTools.ConsoleCommands
{
    public class GetPlayerMoney : MSNConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 1)
                {
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Wrong number of arguments, expected 1, found {_params.Count}.");
                }
                else
                {
                    string steamID = _params[0];
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{PersistentContainer.Instance.Players[steamID].PlayerWallet};{BankChatCommand.DeviseName}");
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "gpm" };

        public override string GetDescription() => "...";
    }
}