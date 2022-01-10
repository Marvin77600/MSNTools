using System.Collections.Generic;

namespace MSNTools.ChatCommands
{
    public abstract class ChatCommandAbstract : IChatCommand
    {
        public abstract string[] GetCommands();

        public abstract void Execute(List<string> _params, ClientInfo _clientInfo);
    }
}