using System;
using System.Collections.Generic;
using System.Xml;

namespace MSNTools.ConsoleCommands
{
    public class EditConfig : MSNConsoleCmdAbstract
    {
        public override int DefaultPermissionLevel => 0;

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 3)
                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"Wrong number of arguments, expected 3, found {_params.Count}.");

                else
                {
                    string toolName = _params[0];
                    string paramName = _params[1];
                    string valueToReplace = _params[2];

                    XmlDocument configFile = new XmlDocument();
                    configFile.Load(Config.ConfigFilePath);

                    foreach (XmlNode item in configFile.DocumentElement.ChildNodes)
                    {
                        foreach (XmlNode item1 in item)
                        {
                            if (item1.Attributes["Name"].Value == toolName)
                            {
                                item1.Attributes[paramName].Value = valueToReplace;
                                configFile.Save(Config.ConfigFilePath);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        public override string[] GetCommands() => new string[] { "editconfig" };

        public override string GetDescription() => "...";
    }
}