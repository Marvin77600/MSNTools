using MSNTools.Discord;
using MSNTools.PersistentData;
using System;
using System.Collections.Generic;

namespace MSNTools.ChatCommands
{
    public class ChatCommandsHook
    {
        public static string ChatCommandsPrefix = "/";
        public static bool ChatCommandsEnabled = false;
        private static readonly List<IChatCommand> chatCommands = new List<IChatCommand>();
        private static readonly Dictionary<string, IChatCommand> m_CommandsAllVariants = new CaseInsensitiveStringDictionary<IChatCommand>();
        private static readonly List<string> staticTokenizedCommandList = new List<string>(16);

        private static void RegisterChatCommand(SortedList<string, IChatCommand> _commandsList, string _className, IChatCommand _command)
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

        public static void RegisterChatCommands()
        {
            try
            {
                MSNUtils.Log("Register Chat Commands");
                SortedList<string, IChatCommand> chatCommandsList = new SortedList<string, IChatCommand>();
                ReflectionHelpers.FindTypesImplementingBase(typeof(IChatCommand), _type =>
                {
                    IChatCommand _command = ReflectionHelpers.Instantiate<IChatCommand>(_type);
                    RegisterChatCommand(chatCommandsList, _type.Name, _command);
                });
                foreach (IChatCommand chatCommand in chatCommandsList.Values)
                {
                    chatCommands.Add(chatCommand);
                    MSNUtils.Log($"{chatCommand.GetCommands()[0]}");
                    for (int index = 0; index < chatCommand.GetCommands().Length; ++index)
                    {
                        string command = chatCommand.GetCommands()[index];
                        if (!string.IsNullOrEmpty(command))
                        {
                            IChatCommand consoleCommand2;
                            if (m_CommandsAllVariants.TryGetValue(command, out consoleCommand2))
                                MSNUtils.LogWarning("Command with alias \"" + command + "\" already registered from " + consoleCommand2.GetType().Name + ", not registering for class " + chatCommand.GetType().Name);
                            else
                                m_CommandsAllVariants.Add(command, chatCommand);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MSNUtils.LogError($"Error registering chat commands");
                MSNUtils.LogError($"{ex}");
            }
        }

        private static IChatCommand GetCommand(string _command, bool _alreadyTokenized = false)
        {
            if (!_alreadyTokenized)
            {
                int length = _command.IndexOf(' ');
                if (length >= 0)
                    _command = _command.Substring(0, length);
            }
            IChatCommand consoleCommand;
            return m_CommandsAllVariants.TryGetValue(_command, out consoleCommand) ? consoleCommand : null;
        }

        private static void ExecuteCommand(string _command, ClientInfo _clientInfo)
        {
            if (string.IsNullOrEmpty(_command))
                return;
            List<string> stringList = tokenizeCommand(_command);
            if (stringList != null)
            {
                if (stringList[0] == string.Empty)
                    return;
                IChatCommand command = GetCommand(stringList[0].Substring(1), true);
                if (command != null)
                {
                    if (GameManager.Instance.World == null)
                    {
                        MSNUtils.LogError("*** ERROR: Command '" + stringList[0] + "' can only be executed when a game is started.");
                    }
                    try
                    {
                        string str = $"{stringList[0]} ";
                        foreach (string s in stringList.GetRange(1, stringList.Count - 1))
                            str += s + " ";
                        str.TrimEnd(' ');
                        if (DiscordWebhookSender.ServerInfosEnabled)
                            DiscordWebhookSender.SendChatCommand(_clientInfo, str);
                        command.Execute(stringList.GetRange(1, stringList.Count - 1), _clientInfo);
                    }
                    catch (Exception ex)
                    {
                        MSNUtils.LogError("*** ERROR: Executing command '" + stringList[0] + "' failed: " + ex.Message);
                        MSNUtils.LogError($"{ex}");
                    }
                }
                else
                    MSNUtils.LogError("*** ERROR: unknown command '" + stringList[0] + "'");
            }
        }

        private static List<string> tokenizeCommand(string _command)
        {
            List<string> tokenizedCommandList = staticTokenizedCommandList;
            tokenizedCommandList.Clear();
            bool flag = false;
            int startIndex = 0;
            for (int index = 0; index < _command.Length; ++index)
            {
                if (!flag)
                {
                    if (_command[index] == '"')
                    {
                        if (index - startIndex > 0)
                            tokenizedCommandList.Add(_command.Substring(startIndex, index - startIndex));
                        startIndex = index + 1;
                        flag = true;
                    }
                    else if (_command[index] == ' ' || _command[index] == '\t')
                    {
                        if (index - startIndex > 0)
                            tokenizedCommandList.Add(_command.Substring(startIndex, index - startIndex));
                        startIndex = index + 1;
                    }
                }
                else if (_command[index] == '"')
                {
                    if (index + 1 < _command.Length && _command[index + 1] == '"')
                    {
                        ++index;
                    }
                    else
                    {
                        string str = _command.Substring(startIndex, index - startIndex).Replace("\"\"", "\"");
                        tokenizedCommandList.Add(str);
                        startIndex = index + 1;
                        flag = false;
                    }
                }
            }
            if (flag)
            {
                Log.Error("*** ERROR: Quotation started at position " + startIndex.ToString() + " was not closed");
                return null;
            }
            if (startIndex < _command.Length)
                tokenizedCommandList.Add(_command.Substring(startIndex, _command.Length - startIndex));
            return tokenizedCommandList;
        }

        public static void Exec(ClientInfo clientInfo, string command)
        {
            ExecuteCommand(command, clientInfo);
        }

        public static void ChatMessage(ClientInfo _cInfo, string _message, int _senderId, string _name, EChatType _type, List<int> _recipientEntityIds)
        {
            try
            {
                if (_message.Contains("U+") || _name.Contains("U+"))
                {
                    return;
                }
                if (_type == EChatType.Whisper)
                {
                    _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(_type, _senderId, _message, _name, false, null));
                }
                else
                {
                    if (_cInfo != null)
                    {
                        if (_recipientEntityIds != null)
                        {
                            if (_recipientEntityIds.Count == 0)
                            {
                                List<EntityPlayer> players = PersistentOperations.PlayerList();
                                for (int i = 0; i < players.Count; i++)
                                {
                                    _recipientEntityIds.Add(players[i].entityId);
                                }
                            }
                        }
                        else
                        {
                            List<int> recipients = new List<int>();
                            List<EntityPlayer> players = PersistentOperations.PlayerList();
                            for (int i = 0; i < players.Count; i++)
                            {
                                recipients.Add(players[i].entityId);
                            }
                            _recipientEntityIds = recipients;
                        }
                    }
                    MSNLocalization.Language language = PersistentContainer.Instance.Players[_cInfo.PlatformId.ToString()].Language;

                    GameManager.Instance.ChatMessageServer(_cInfo, _type, -1, _message, _name, false, _recipientEntityIds);
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in ChatCommandsHook.ChatMessage: {e.Message}");
            }
        }
    }
}