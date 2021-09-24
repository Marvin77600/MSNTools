using System;
using SystemColor = System.Drawing.Color;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;
using MSNTools.Discord;

namespace MSNTools
{
    public class Config
    {
        public static string ModName = "MSNTools";
        public const string ModVersion = "1.0.0";
        public const string GameVersion = "19.6.8";
        public static string ModPrefix = $"[{ModName.ToUpper()}]", Server_Response_Name = $"{ModName}", Chat_Response_Color = "[00FF00]";
        public static string ConfigPath = $"{GamePrefs.GetString(EnumGamePrefs.SaveGameFolder)}/{ModName}";
        private static string ConfigFile = $"{ModName}Config.xml";
        public static string ConfigFilePath = $"{ConfigPath}/{ConfigFile}";

        private static FileSystemWatcher FileWatcher = new FileSystemWatcher();

        private static Dictionary<string, Dictionary<string, List<string>>> ConfigFileStruct = new Dictionary<string, Dictionary<string, List<string>>>
        {
            { "Versions", new Dictionary<string, List <string>>
                {
                    {"Mod", new List<string> { "Value" } },
                    {"Game", new List<string> { "Value" } }
                }
            },
            { "BaseConfs", new Dictionary<string, List <string>>
                {
                    {"ModName", new List<string> { "Value" } },
                    {"Server_Response_Name", new List<string> { "Value" } },
                    {"Chat_Response_Color", new List<string> { "Value" } },
                    {"Discord_Footer_Image_Url", new List<string> { "Value" } }
                }
            },
            { "Tools", new Dictionary<string, List <string>>
                {
                    {"Chat_Webhook", new List<string> { "Enable", "Webhook_Url" } },
                    {"Godmode_Detector", new List<string> { "Enable", "Admin_Level" } },
                    {"High_Ping_Kicker", new List<string> { "Enable", "Max_Ping", "Flags" } },
                    {"Inventory_Check", new List<string> { "Enable", "Admin_Level", "Check_Storage" } },
                    {"Player_Infos_Webhook", new List<string> { "Enable", "Webhook_Url", "Connected_Color", "Disconnected_Color" } },
                    {"Sanctions_Webhook", new List<string> { "Enable", "Webhook_Url", "Color" } },
                    {"Server_Infos_Webhook", new List<string> { "Enable", "Webhook_Url", "Connected_Color", "Disconnected_Color" } },
                    {"Spectator_Detector", new List<string> { "Enable", "Admin_Level" } }
                }
            }
        };

        public static void Load()
        {
            Log.Out($"{ModPrefix} Checking for save directory {ConfigPath}");
            if (!Directory.Exists(ConfigPath))
            {
                Directory.CreateDirectory(ConfigPath);
                Log.Out($"{ModPrefix} Created directory {ConfigPath}");
            }
            LoadXml();
            InitFileWatcher();
        }

        public static void LoadXml()
        {
            Log.Out("---------------------------------------------------------------");
            Log.Out($"{ModPrefix} Verifying configuration file & Saving new entries");
            Log.Out("---------------------------------------------------------------");
            if (!Utils.FileExists(ConfigFilePath))
            {
                WriteXml();
            }
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(ConfigFilePath);
            }
            catch (XmlException e)
            {
                Log.Error($"{ModPrefix} Failed loading {ConfigFilePath}: {e.Message}");
                return;
            }
            foreach (XmlNode childNode in xmlDoc.DocumentElement.ChildNodes)
            {
                foreach (XmlNode subChild in childNode.ChildNodes)
                {
                    if (subChild.NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }
                    if (subChild.NodeType != XmlNodeType.Element)
                    {
                        Log.Warning($"{ModPrefix} Unexpected XML node found in '{childNode.Name}' section: {subChild.OuterXml}");
                        continue;
                    }
                    XmlElement _line = (XmlElement)subChild;
                    if (!_line.HasAttribute("Name"))
                    {
                        Log.Warning($"{ModPrefix} Ignoring {subChild.Name} entry in {ConfigFile} because of missing 'Name' attribute: {subChild.OuterXml}");
                        continue;
                    }
                    foreach (string nameAttribute in ConfigFileStruct[childNode.Name].Keys)
                    {
                        if (nameAttribute != _line.GetAttribute("Name"))
                            continue;

                        foreach (string paramName in ConfigFileStruct[childNode.Name][nameAttribute])
                        {
                            if (!_line.HasAttribute(paramName))
                            {
                                Log.Warning($"{ModPrefix} Ignoring {nameAttribute} entry in {ConfigFile} because of missing '{paramName}' attribute: {subChild.OuterXml}");
                                break;
                            }
                            else
                            {
                                string attribute = _line.GetAttribute(paramName);

                                if (childNode.Name == "Versions")
                                {
                                    if (nameAttribute == "Mod")
                                    {
                                        if (attribute != ModVersion)
                                        {
                                            Log.Out($"{ModPrefix} Detected updated version of {ModName}");
                                            string[] _files = Directory.GetFiles(ConfigPath, "*.xml");
                                            if (!Directory.Exists(ConfigPath + "/XMLBackups"))
                                            {
                                                Directory.CreateDirectory(ConfigPath + "/XMLBackups");
                                            }
                                            if (_files != null && _files.Length > 0)
                                            {
                                                if (!Directory.Exists(ConfigPath + "/XMLBackups/" + attribute))
                                                {
                                                    Directory.CreateDirectory(ConfigPath + "/XMLBackups/" + attribute);
                                                    for (int i = 0; i < _files.Length; i++)
                                                    {
                                                        string _fileName = _files[i];
                                                        string _fileNameShort = _fileName.Substring(_fileName.IndexOf($"{ModName}") + ModName.Length);
                                                        File.Copy(_fileName, ConfigPath + "/XMLBackups/" + attribute + _fileNameShort);
                                                        if (_fileNameShort == ConfigFile)
                                                        {
                                                            File.Delete(_fileName);
                                                        }
                                                    }
                                                    WriteXml();
                                                    //Config.UpgradeXml(xmlDoc.DocumentElement.ChildNodes[1].ChildNodes);
                                                    Log.Out($"{ModPrefix} Created backup of xml files for version {attribute}");
                                                }
                                            }
                                        }
                                    }
                                }
                                if (childNode.Name == "BaseConfs")
                                {
                                    if (nameAttribute == "ModName")
                                    {
                                        ModName = attribute;
                                    }
                                    else if (nameAttribute == "Server_Response_Name")
                                    {
                                        Server_Response_Name = attribute;
                                    }
                                    else if (nameAttribute == "Chat_Response_Color")
                                    {
                                        Chat_Response_Color = attribute;
                                    }
                                    else if (nameAttribute == "Discord_Footer_Image_Url")
                                    {
                                        DiscordWebhookSender.FooterImageUrl = attribute;
                                    }
                                }
                                if (childNode.Name == "Tools")
                                {
                                    switch (nameAttribute)
                                    {
                                        case "Chat_Webhook":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out DiscordWebhookSender.ChatEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Webhook_Url")
                                            {
                                                if (DiscordWebhookSender.ChatEnabled)
                                                    if (!TryParseUrl(attribute, out DiscordWebhookSender.ChatWebHookUrl, nameAttribute, paramName, subChild))
                                                        continue;
                                            }
                                            break;
                                        case "Godmode_Detector":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out PlayerChecks.GodEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Admin_Level")
                                            {
                                                if (!TryParseInt(attribute, out PlayerChecks.God_Admin_Level, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                        //case "High_Ping_Kicker":
                                        //    if (paramName == "Enable")
                                        //    {
                                        //        if (!TryParseBool(attribute, out HighPingKickerEnabled, nameAttribute, paramName, subChild))
                                        //            continue;
                                        //    }
                                        //    else if (paramName == "Max_Ping")
                                        //    {
                                        //        if (!TryParseInt(attribute, out HighPingKickerMaxPing, nameAttribute, paramName, subChild))
                                        //            continue;
                                        //    }
                                        //    else if (paramName == "Flags")
                                        //    {
                                        //        if (!TryParseInt(attribute, out HighPingKickerFlags, nameAttribute, paramName, subChild))
                                        //            continue;
                                        //    }
                                        //    break;
                                        case "Inventory_Check":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out InventoryChecks.IsEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Admin_Level")
                                            {
                                                if (!TryParseInt(attribute, out InventoryChecks.Admin_Level, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Check_Storage")
                                            {
                                                if (!TryParseBool(attribute, out InventoryChecks.Check_Storage, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                        case "Player_Infos_Webhook":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out DiscordWebhookSender.PlayerInfosEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Webhook_Url")
                                            {
                                                if (!TryParseUrl(attribute, out DiscordWebhookSender.PlayerInfosWebHookUrl, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Connected_Color")
                                            {
                                                if (!TryParseColor(attribute, out DiscordWebhookSender.PlayerConnectedColor, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Disconnected_Color")
                                            {
                                                if (!TryParseColor(attribute, out DiscordWebhookSender.PlayerDisconnectedColor, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                        case "Sanctions_Webhook":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out DiscordWebhookSender.SanctionsEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Webhook_Url")
                                            {
                                                if (DiscordWebhookSender.SanctionsEnabled)
                                                    if (!TryParseUrl(attribute, out DiscordWebhookSender.SanctionsWebHookUrl, nameAttribute, paramName, subChild))
                                                        continue;
                                            }
                                            else if (paramName == "Color")
                                            {
                                                if (!TryParseColor(attribute, out DiscordWebhookSender.SanctionsColor, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                        case "Server_Infos_Webhook":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out DiscordWebhookSender.ServerInfosEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Webhook_Url")
                                            {
                                                if (!TryParseUrl(attribute, out DiscordWebhookSender.ServerInfosWebHookUrl, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Connected_Color")
                                            {
                                                if (!TryParseColor(attribute, out DiscordWebhookSender.ServerOnlineColor, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Disconnected_Color")
                                            {
                                                if (!TryParseColor(attribute, out DiscordWebhookSender.ServerOfflineColor, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                        case "Spectator_Detector":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out PlayerChecks.SpectatorEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Admin_Level")
                                            {
                                                if (!TryParseInt(attribute, out PlayerChecks.Spectator_Admin_Level, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        public static void WriteXml()
        {
            FileWatcher.EnableRaisingEvents = false;
            using (StreamWriter sw = new StreamWriter(ConfigFilePath, false, Encoding.UTF8))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine($"<{ModName}>");
                sw.WriteLine("    <Versions>");
                sw.WriteLine($"        <Version Name=\"Mod\" Value=\"{ModVersion}\" />");
                sw.WriteLine($"        <Version Name=\"Game\" Value=\"{GameVersion}\" />");
                sw.WriteLine("    </Versions>");
                sw.WriteLine("    <BaseConfs>");
                sw.WriteLine($"        <BaseConf Name=\"ModName\" Value=\"{ModName}\" />");
                sw.WriteLine($"        <BaseConf Name=\"Server_Response_Name\" Value=\"{Server_Response_Name}\" />");
                sw.WriteLine($"        <BaseConf Name=\"Chat_Response_Color\" Value=\"{Chat_Response_Color}\" />");
                sw.WriteLine($"        <BaseConf Name=\"Discord_Footer_Image_Url\" Value=\"{DiscordWebhookSender.FooterImageUrl}\" />");
                sw.WriteLine("    </BaseConfs>");
                sw.WriteLine("    <Tools>");
                sw.WriteLine($"        <Tool Name=\"Chat_Webhook\" Enable=\"{DiscordWebhookSender.ChatEnabled}\" Webhook_Url=\"{DiscordWebhookSender.ChatWebHookUrl}\" />");
                sw.WriteLine($"        <Tool Name=\"Godmode_Detector\" Enable=\"{PlayerChecks.GodEnabled}\" Admin_Level=\"{PlayerChecks.God_Admin_Level}\" />");
                //sw.WriteLine($"        <Tool Name=\"High_Ping_Kicker\" Enable=\"{0}\" Max_Ping=\"{1}\" Flags=\"{2}\" />");
                sw.WriteLine($"        <Tool Name=\"Player_Infos_Webhook\" Enable=\"{DiscordWebhookSender.PlayerInfosEnabled}\" Webhook_Url=\"{DiscordWebhookSender.PlayerInfosWebHookUrl}\" Connected_Color=\"{DiscordWebhookSender.PlayerConnectedColor}\" Disconnected_Color=\"{DiscordWebhookSender.PlayerDisconnectedColor}\" />");
                sw.WriteLine($"        <Tool Name=\"Server_Infos_Webhook\" Enable=\"{DiscordWebhookSender.ServerInfosEnabled}\" Webhook_Url=\"{DiscordWebhookSender.ServerInfosWebHookUrl}\" Connected_Color=\"{DiscordWebhookSender.ServerOnlineColor}\" Disconnected_Color=\"{DiscordWebhookSender.ServerOfflineColor}\" />");
                sw.WriteLine($"        <Tool Name=\"Inventory_Check\" Enable=\"{InventoryChecks.IsEnabled}\" Admin_Level=\"{InventoryChecks.Admin_Level}\" Check_Storage=\"{InventoryChecks.Check_Storage}\" />");
                sw.WriteLine($"        <Tool Name=\"Sanctions_Webhook\" Enable=\"{DiscordWebhookSender.SanctionsEnabled}\" Webhook_Url=\"{DiscordWebhookSender.SanctionsWebHookUrl}\" Color=\"{DiscordWebhookSender.SanctionsColor}\"/>");
                sw.WriteLine($"        <Tool Name=\"Spectator_Detector\" Enable=\"{PlayerChecks.SpectatorEnabled}\" Admin_Level=\"{PlayerChecks.Spectator_Admin_Level}\" />");
                sw.WriteLine("    </Tools>");
                sw.WriteLine($"</{ModName}>");
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            FileWatcher.EnableRaisingEvents = true;
        }

        private static void InitFileWatcher()
        {
            FileWatcher = new FileSystemWatcher(ConfigPath, ConfigFile);
            FileWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
            FileWatcher.Created += new FileSystemEventHandler(OnFileChanged);
            FileWatcher.Deleted += new FileSystemEventHandler(OnFileChanged);
            FileWatcher.EnableRaisingEvents = true;
        }

        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            LoadXml();
            //Mods.Load(false);
            //ActiveTools.Exec(false);
        }

        public static void UpgradeXml(XmlNodeList _oldRootNodes)
        {
            try
            {
                FileWatcher.EnableRaisingEvents = false;
                if (Utils.FileExists(ConfigFilePath))
                {
                    XmlDocument _newXml = new XmlDocument();
                    try
                    {
                        _newXml.Load(ConfigFilePath);
                    }
                    catch (XmlException e)
                    {
                        Log.Error(string.Format("[SERVERTOOLS] Failed loading {0}: {1}", ConfigFilePath, e.Message));
                        return;
                    }
                    XmlNodeList _newRootNodes = _newXml.DocumentElement.ChildNodes[1].ChildNodes;
                    for (int i = 0; i < _newRootNodes.Count; i++)
                    {
                        if (_newRootNodes[i].NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        if (_newRootNodes[i].Attributes.Count > 0 && _newRootNodes[i].Attributes[0].Name == "Name")
                        {
                            for (int j = 0; j < _oldRootNodes.Count; j++)
                            {
                                if (_oldRootNodes[j].NodeType == XmlNodeType.Comment)
                                {
                                    continue;
                                }
                                if (_oldRootNodes[j].Attributes.Count > 0 && _oldRootNodes[j].Attributes[0].Name == "Name" && _oldRootNodes[j].Attributes[0].Value == _newRootNodes[i].Attributes[0].Value)
                                {
                                    for (int k = 1; k < _newRootNodes[i].Attributes.Count; k++)
                                    {
                                        for (int l = 1; l < _oldRootNodes[j].Attributes.Count; l++)
                                        {
                                            if (_newRootNodes[i].Attributes[k].Name == _oldRootNodes[j].Attributes[l].Name)
                                            {
                                                _newRootNodes[i].Attributes[k].Value = _oldRootNodes[j].Attributes[l].Value;
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    _newXml.Save(ConfigFilePath);
                }
            }
            catch (Exception e)
            {
                Log.Out($"{ModPrefix} Error in Config.UpgradeXml: {e.Message}");
            }
            FileWatcher.EnableRaisingEvents = true;
        }

        static bool TryParseBool(string value, out bool result, string nameAttribute, string paramName, XmlNode node)
        {
            bool parse = bool.TryParse(value, out result);
            if (!parse)
                Log.Warning($"{ModPrefix} Ignoring {nameAttribute} entry in {ConfigFile} because of invalid (True/False) value for '{paramName}' attribute: {node.OuterXml}");
            return parse;
        }

        static bool TryParseInt(string value, out int result, string nameAttribute, string paramName, XmlNode node)
        {
            bool parse = int.TryParse(value, out result);
            if (!parse)
                Log.Warning($"{ModPrefix} Ignoring {nameAttribute} entry in {ConfigFile} because of invalid (non-numeric) value for '{paramName}' attribute: {node.OuterXml}");
            return parse;
        }

        static bool TryParseUrl(string value, out string result, string nameAttribute, string paramName, XmlNode node)
        {
            if (value.StartsWith("http://") || value.StartsWith("https://"))
            {
                result = value;
                return true;
            }
            else
            {
                Log.Warning($"{ModPrefix} Ignoring {nameAttribute} entry in {ConfigFile} because of invalid url value for '{paramName}' attribute: {node.OuterXml}");
                result = string.Empty;
                return false;
            }
        }

        static bool TryParseColor(string value, out SystemColor systemColor, string nameAttribute, string paramName, XmlNode node)
        {
            try
            {
                string pattern = @"^(?:(?:^|,\s?)([01]?\d\d?|2[0-4]\d|25[0-5])){3}$";
                Match m = Regex.Match(value, pattern);
                if (m.Success)
                {
                    string[] rgb = value.Split(',');
                    int r = int.Parse(rgb[0]);
                    int g = int.Parse(rgb[1]);
                    int b = int.Parse(rgb[2]);
                    SystemColor color = SystemColor.FromArgb(r, g, b);
                    systemColor = color;
                    return true;
                }
                else
                {
                    Log.Warning($"{ModPrefix} Ignoring {nameAttribute} entry in {ConfigFile} because of invalid color value for '{paramName}' attribute: {node.OuterXml}");
                    systemColor = SystemColor.White;
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Out($"{ModPrefix} Error in Config.TryParseColor: {e.Message}");
            }
            systemColor = SystemColor.White;
            return false;
        }
    }
}