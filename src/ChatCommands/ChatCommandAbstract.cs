using System.Collections.Generic;

namespace MSNTools.ChatCommands
{
    public abstract class ChatCommandAbstract : IChatCommand
    {
        public abstract string[] GetCommands();

        public abstract string Execute(List<string> _params, ClientInfo _clientInfo);
    }
}