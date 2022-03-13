using System;
using System.Collections.Generic;
using MSNTools.PersistentData;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.ConsoleCommands
{
    public class LoadPersistentContainer : ConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                PersistentContainer.Instance.Load();
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "loadpersistentcontainer" };

        public override string GetDescription() => "...";
    }
}