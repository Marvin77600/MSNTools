using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSNTools
{
    public class DiscordBot
    {
        public static Thread DiscordBotThread = null;
        public static Process DiscordBotProcess = null;

        public static void Start()
        {
            Thread thread = new Thread(Bot);
            thread.Start();
            DiscordBotThread = thread;
        }

        static void Bot()
        {
            Process process = new Process();
            process.StartInfo.FileName = @"node";
            process.StartInfo.Arguments = @".\index.js";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.WorkingDirectory = $@"{API.Mod.Path}\7DaysDiscordBot\";
            process.Start();
            DiscordBotProcess = process;
            MSNUtils.Log($"DiscordBot Started");
            
            process.StandardInput.Flush();
            process.StandardInput.Close();
        }

        public static void Shutdown()
        {
            DiscordBotProcess.Dispose();
            DiscordBotThread.Abort();
            DiscordBotProcess = null;
            DiscordBotThread = null;
            MSNUtils.Log($"DiscordBot Shutdown");
        }
    }
}