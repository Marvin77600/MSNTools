using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.ConsoleCommands
{
    public class MSNHelp : MSNConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                SortedList<string, MSNConsoleCmdAbstract> chatCommandsList = new SortedList<string, MSNConsoleCmdAbstract>();
                ReflectionHelpers.FindTypesImplementingBase(typeof(MSNConsoleCmdAbstract), _type =>
                {
                    MSNConsoleCmdAbstract _command = ReflectionHelpers.Instantiate<MSNConsoleCmdAbstract>(_type);
                    RegisterMSNCommand(chatCommandsList, _type.Name, _command);
                });
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append($"Command count : {chatCommandsList.Count}\n");
                foreach (var command in chatCommandsList)
                {
                    stringBuilder.Append($"{command.Key} => {command.Value.GetDescription()}\n");
                }
                SingletonMonoBehaviour<SdtdConsole>.Instance.Output(stringBuilder.ToString());
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        private static void RegisterMSNCommand(SortedList<string, MSNConsoleCmdAbstract> _commandsList, string _className, MSNConsoleCmdAbstract _command)
        {
            string[] commands = _command.GetCommands();
            string str = commands[0];
            if (_commandsList.ContainsKey(str))
            {
                MSNUtils.LogWarning("Chat command with name \"" + str + "\" already loaded, not loading from class " + _className);
            }
            else
            {
                _commandsList.Add(str, _command);
            }
        }

        public override string[] GetCommands() => new string[] { "msn-help" };

        public override string GetDescription() => $"Get list of MSNTools commands.";

        public override string GetHelp() => GetDescription() + "\nUsage:\n" +
            $"   msn-help";
    }
}