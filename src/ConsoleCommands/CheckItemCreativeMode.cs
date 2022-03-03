using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.ConsoleCommands
{
    public class CheckItemCreativeMode : MSNConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 1)
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Wrong number of arguments, expected 1, found {_params.Count}.");
                else
                {
                    string itemName = _params[0];
                    EnumCreativeMode enumCreativeMode = ItemClass.GetItem(itemName).ItemClass.CreativeMode;
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Item {itemName} is {enumCreativeMode} mode");
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "checkitemcreativemode", "cicm" };

        public override string GetDescription() => "Check the creative mode of an item.";

        public override string GetHelp() => GetDescription() + "\nUsage:\n   checkitemcreativemode <itemname>";
    }
}