using MSNTools.ChatCommands;
using BankBehaviour = MSNTools.Bank;
using MSNTools.PersistentData;
using System;
using System.Collections.Generic;

namespace MSNTools.ConsoleCommands
{
    public class Bank : MSNConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count == 3)
                {
                    int entityID = int.Parse(_params[1]);
                    int value = int.Parse(_params[2]);
                    ClientInfo clientInfo = PersistentOperations.GetClientInfoFromEntityId(entityID);
                    int wallet = PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].PlayerWallet;
                    if (clientInfo != null)
                    {
                        if (_params[0].EqualsCaseInsensitive("set"))
                        {
                            PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].PlayerWallet = value;
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{clientInfo.playerName} have now {value} {BankBehaviour.DeviseName}.");
                        }
                        else if (_params[0].EqualsCaseInsensitive("add"))
                        {
                            PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].PlayerWallet += value;
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{clientInfo.playerName} have now {wallet + value} {BankBehaviour.DeviseName}.");
                        }
                        else if (_params[0].EqualsCaseInsensitive("remove"))
                        {
                            PersistentContainer.Instance.Players[clientInfo.PlatformId.ToString()].PlayerWallet -= value;
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{clientInfo.playerName} have now {wallet - value} {BankBehaviour.DeviseName}.");
                        }
                        PersistentContainer.DataChange = true;
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "bank" };

        public override string GetDescription() => "Manage the bank of a player.";

        public override string GetHelp() => GetDescription() + "\nUsage:\n" +
            "   bank set <entity_id> <value>\n" +
            "   bank add <entity_id> <value>\n" +
            "   bank remove <entity_id> <value>";
    }
}