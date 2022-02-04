using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.ConsoleCommands
{
    public class CmdDiscordBot : ConsoleCmdAbstract
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
                    if (_params[0] == "start")
                    {
                        if (DiscordBot.DiscordBotProcess != null && DiscordBot.DiscordBotThread != null)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{Config.ModPrefix} Discord Bot is already running, you can't start an another !");
                        }
                        else
                        {
                            DiscordBot.Start();
                        }
                    }
                    else if (_params[0] == "stop")
                    {
                        if (DiscordBot.DiscordBotProcess == null && DiscordBot.DiscordBotThread == null)
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{Config.ModPrefix} Discord Bot is not running, you can't stop it !");
                        }
                        else
                        {
                            DiscordBot.Shutdown();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "discordbot", "discord-bot" };

        public override string GetDescription() => "...";
    }
}