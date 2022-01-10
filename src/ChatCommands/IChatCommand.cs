using System.Collections.Generic;

namespace MSNTools.ChatCommands
{
    public interface IChatCommand
    {
        string[] GetCommands();

        void Execute(List<string> _params, ClientInfo _clientInfo);
    }
}
