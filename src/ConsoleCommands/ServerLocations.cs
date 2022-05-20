using MSNTools.PersistentData;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSNTools.ConsoleCommands
{
    public class ServerLocations : MSNConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                Dictionary<string, string[]> serverLocations = PersistentContainer.Instance.ServerLocations;
                if (_params[0] == "add")
                {
                    if (serverLocations.ContainsKey(_params[1]))
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Server location {_params[1]} already exists.");
                    }
                    else
                    {
                        string locationName = _params[1];
                        string x = _params[2];
                        string y = _params[3];
                        string z = _params[4];

                        List<string> locations = new List<string>();
                        string[] positions = new string[] { x, y, z };
                        serverLocations.Add(locationName, positions);
                        PersistentContainer.Instance.ServerLocations = serverLocations;
                        PersistentContainer.DataChange = true;

                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Server location {_params[1]} at position ({x}, {y}, {z}) was added.");
                    }
                }
                else if (_params[0] == "remove" || _params[0] == "del")
                {
                    if (!serverLocations.ContainsKey(_params[1]))
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Server location {_params[1]} do not exists.");
                    }
                    else
                    {
                        serverLocations.Remove(_params[1]);
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Server location {_params[1]} was removed.");
                        PersistentContainer.Instance.ServerLocations = serverLocations;
                        PersistentContainer.DataChange = true;
                    }
                }
                else if (_params[0] == "list")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var item in serverLocations)
                        stringBuilder.Append($"{item.Key} : {string.Join(", ", item.Value)}\n");
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output(stringBuilder.ToString());
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "serverlocations" };

        public override string GetDescription() => "Manage the server locations tps list.";

        public override string GetHelp() => GetDescription() + "\nUsage:\n" +
            "   serverlocations add <location_name> <x y z>\n" +
            "   serverlocations remove <location_name>\n" +
            "   serverlocations list";
    }
}