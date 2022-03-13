using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;
using MSNTools.ChatCommands;
using MSNTools.Discord;
using MSNTools.PersistentData;
using UnityEngine;

namespace MSNTools
{
    public class Config
    {
        public static string ModName = "MSNTools";
        public const string ModVersion = "1.0.32";
        public static string GameVersion = $"{Constants.cVersionMajor}.{Constants.cVersionMinor} (b{Constants.cVersionBuild})";
        public static string ModPrefix = $"[{ModName.ToUpper()}]", Server_Response_Name = $"{ModName}", Chat_Response_Color = "[00FF00]";
        public static string ConfigPath = $"{GamePrefs.GetString(EnumGamePrefs.SaveGameFolder)}/{ModName}";
        public static string ConfigFile = $"{ModName}Config.xml";
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
                    {"Discord_Footer_Image_Url", new List<string> { "Value" } },
                    {"BloodMoon_Alerts_Image_Url", new List<string> { "Value" } },
                    {"Server_Language", new List<string> { "Value" } }
                }
            },
            { "Tools", new Dictionary<string, List <string>>
                {
                    {"Alerts_Webhook", new List<string> { "Enable", "Webhook_Url", "Color" } },
                    {"Anti_Collapse", new List<string> { "Enable", "Min_Entities_Detected"} },
                    {"Bank", new List<string> { "Enable", "Gain_Every_Hours", "Donator_Gain_Every_Hours", "Hour", "Devise_Name", "Max"} },
                    {"BloodMoon_Alerts_Webhook", new List<string> { "Enable", "Webhook_Url", "Hour"} },
                    {"Chat_Commands", new List<string> { "Enable", "Prefix"} },
                    {"Chat_Webhook", new List<string> { "Enable", "Webhook_Url" } },
                    {"Godmode_Detector", new List<string> { "Enable", "Admin_Level" } },
                    {"High_Ping_Kicker", new List<string> { "Enable", "Max_Ping", "Flags" } },
                    {"Inventory_Check", new List<string> { "Enable", "Admin_Level", "Check_Storage", "Exceptions_Items" } },
                    {"Player_Infos_Webhook", new List<string> { "Enable", "Webhook_Url", "Connected_Color", "Disconnected_Color" } },
                    {"Player_Invulnerability_At_Trader", new List<string> { "Enable" } },
                    {"Reset_Regions", new List<string> { "Enable", "Day", "Hour", "Buff_Reset_Zones" } },
                    {"Sanctions_Webhook", new List<string> { "Enable", "Webhook_Url", "Color" } },
                    {"Server_Infos_Webhook", new List<string> { "Enable", "Webhook_Url", "Connected_Color", "Disconnected_Color" } },
                    {"Spectator_Detector", new List<string> { "Enable", "Admin_Level" } },
                    {"TP_Command", new List<string> { "Enable", "TP_Cost", "TP_Max_Count" } },
                    {"Vote_Command", new List<string>{ "Enable", "Gain_Per_Vote", "API_Server_Token" } }
                }
            }
        };

        public static void Load()
        {
            MSNUtils.Log($"Checking for save directory {ConfigPath}");
            if (!Directory.Exists(ConfigPath))
            {
                Directory.CreateDirectory(ConfigPath);
                MSNUtils.Log($"Created directory {ConfigPath}");
            }
            LoadXml();
            InitFileWatcher();
        }

        public static void LoadXml()
        {
            MSNUtils.Log("---------------------------------------------------------------");
            MSNUtils.Log($"Verifying configuration file & Saving new entries");
            MSNUtils.Log("---------------------------------------------------------------");
            if (!File.Exists(ConfigFilePath))
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
                Log.Error($"Failed loading {ConfigFilePath}: {e.Message}");
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
                        MSNUtils.LogWarning($"Unexpected XML node found in '{childNode.Name}' section: {subChild.OuterXml}");
                        continue;
                    }
                    XmlElement _line = (XmlElement)subChild;
                    if (!_line.HasAttribute("Name"))
                    {
                        MSNUtils.LogWarning($"Ignoring {subChild.Name} entry in {ConfigFile} because of missing 'Name' attribute: {subChild.OuterXml}");
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
                                MSNUtils.LogWarning($"Ignoring {nameAttribute} entry in {ConfigFile} because of missing '{paramName}' attribute: {subChild.OuterXml}");
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
                                            MSNUtils.Log($"Detected updated version of {ModName}");
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
                                                    MSNUtils.Log($"Created backup of xml files for version {attribute}");
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
                                    else if (nameAttribute == "BloodMoon_Alerts_Image_Url")
                                    {
                                        DiscordWebhookSender.BloodMoonAlertImageUrl = attribute;
                                    }
                                    else if (nameAttribute == "Server_Language")
                                    {
                                        PersistentContainer.Instance.ServerLanguage = TryParseLanguage(attribute);
                                        PersistentContainer.DataChange = true;
                                    }
                                }
                                if (childNode.Name == "Tools")
                                {
                                    switch (nameAttribute)
                                    {
                                        case "Alerts_Webhook":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out DiscordWebhookSender.AlertsEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Webhook_Url")
                                            {
                                                if (!TryParseUrl(attribute, out DiscordWebhookSender.AlertsWekHookUrl, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Color")
                                            {
                                                if (!TryParseColor(attribute, out DiscordWebhookSender.AlertsColor, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                        case "Anti_Collapse":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out AntiCollapse.IsEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Min_Entities_Detected")
                                            {
                                                if (!TryParseInt(attribute, out AntiCollapse.MinEntitiesDetected, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                        case "Bank":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out Bank.IsEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Devise_Name")
                                            {
                                                if (!TryParseString(attribute, out Bank.DeviseName, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Donator_Gain_Every_Hours")
                                            {
                                                if (!TryParseInt(attribute, out Bank.DonatorGainEveryHours, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Hour")
                                            {
                                                if (!TryParseInt(attribute, out Bank.Hours, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Gain_Every_Hours")
                                            {
                                                if (!TryParseInt(attribute, out Bank.GainEveryHours, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Max")
                                            {
                                                if (!TryParseInt(attribute, out Bank.Max, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                        case "BloodMoon_Alerts_Webhook":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out BloodMoonAlerts.IsEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Webhook_Url")
                                            {
                                                if (!TryParseUrl(attribute, out DiscordWebhookSender.BloodMoonWebHookUrl, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Hour")
                                            {
                                                if (!TryParseInt(attribute, out BloodMoonAlerts.Hour, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                        case "Chat_Webhook":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out DiscordWebhookSender.ChatEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Webhook_Url")
                                            {
                                                if (!TryParseUrl(attribute, out DiscordWebhookSender.ChatWebHookUrl, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                        case "Chat_Commands":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out ChatCommandsHook.ChatCommandsEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Prefix")
                                            {
                                                if (!TryParseString(attribute, out ChatCommandsHook.ChatCommandsPrefix, nameAttribute, paramName, subChild))
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
                                            else if (paramName == "Exceptions_Items")
                                            {
                                                if (!TryParseList(attribute, out InventoryChecks.Exceptions_Items, nameAttribute, paramName, subChild))
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
                                        case "Player_Invulnerability_At_Trader":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out PlayerInvulnerabilityAtTrader.IsEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                        case "Reset_Regions":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out ResetRegions.IsEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Day")
                                            {
                                                if (!TryParseInt(attribute, out ResetRegions.Day, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Hour")
                                            {
                                                if (!TryParseInt(attribute, out ResetRegions.Hour, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Buff_Reset_Zones")
                                            {
                                                if (!TryParseString(attribute, out Zones.BuffResetZone, nameAttribute, paramName, subChild))
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
                                        case "TP_Command":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out ChatCommandTP.IsEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "TP_Cost")
                                            {
                                                if (!TryParseInt(attribute, out ChatCommandTP.TPCost, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "TP_Max_Count")
                                            {
                                                if (!TryParseInt(attribute, out ChatCommandTP.TPMaxCount, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            break;
                                        case "Vote_Command":
                                            if (paramName == "Enable")
                                            {
                                                if (!TryParseBool(attribute, out ChatCommandVote.IsEnabled, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "Gain_Per_Vote")
                                            {
                                                if (!TryParseInt(attribute, out ChatCommandVote.GainPerVote, nameAttribute, paramName, subChild))
                                                    continue;
                                            }
                                            else if (paramName == "API_Server_Token")
                                            {
                                                if (!TryParseString(attribute, out ChatCommandVote.APIServerToken, nameAttribute, paramName, subChild))
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
                sw.WriteLine($"        <BaseConf Name=\"BloodMoon_Alerts_Image_Url\" Value=\"{DiscordWebhookSender.BloodMoonAlertImageUrl}\" />");
                sw.WriteLine($"        <BaseConf Name=\"Server_Language\" Value=\"{PersistentContainer.Instance.ServerLanguage}\" />");
                sw.WriteLine("    </BaseConfs>");
                sw.WriteLine("    <Tools>");
                sw.WriteLine($"        <Tool Name=\"Alerts_Webhook\" Enable=\"{DiscordWebhookSender.AlertsEnabled}\" Webhook_Url=\"{DiscordWebhookSender.AlertsWekHookUrl}\" Color=\"{DiscordWebhookSender.AlertsColor.r},{DiscordWebhookSender.AlertsColor.g},{DiscordWebhookSender.AlertsColor.b}\" />");
                sw.WriteLine($"        <Tool Name=\"Anti_Collapse\" Enable=\"{AntiCollapse.IsEnabled}\" Min_Entities_Detected=\"{AntiCollapse.MinEntitiesDetected}\" />");
                sw.WriteLine($"        <Tool Name=\"Bank\" Enable=\"{Bank.IsEnabled}\" Hours=\"{Bank.Hours}\" Donator_Gain_Every_Hours=\"{Bank.DonatorGainEveryHours}\" Gain_Every_Hours=\"{Bank.GainEveryHours}\" Devise_Name=\"{Bank.DeviseName}\" Max=\"{Bank.Max}\" />");
                sw.WriteLine($"        <Tool Name=\"BloodMoon_Alerts_Webhook\" Webhook_Url=\"{DiscordWebhookSender.BloodMoonWebHookUrl}\" Enable=\"{BloodMoonAlerts.IsEnabled}\" Hour=\"{BloodMoonAlerts.Hour}\" />");
                sw.WriteLine($"        <Tool Name=\"Chat_Commands\" Enable=\"{ChatCommandsHook.ChatCommandsEnabled}\" Prefix=\"{ChatCommandsHook.ChatCommandsPrefix}\" />");
                sw.WriteLine($"        <Tool Name=\"Chat_Webhook\" Enable=\"{DiscordWebhookSender.ChatEnabled}\" Webhook_Url=\"{DiscordWebhookSender.ChatWebHookUrl}\" />");
                sw.WriteLine($"        <Tool Name=\"Godmode_Detector\" Enable=\"{PlayerChecks.GodEnabled}\" Admin_Level=\"{PlayerChecks.God_Admin_Level}\" />");
                //sw.WriteLine($"        <Tool Name=\"High_Ping_Kicker\" Enable=\"{0}\" Max_Ping=\"{1}\" Flags=\"{2}\" />");
                sw.WriteLine($"        <Tool Name=\"Inventory_Check\" Enable=\"{InventoryChecks.IsEnabled}\" Admin_Level=\"{InventoryChecks.Admin_Level}\" Check_Storage=\"{InventoryChecks.Check_Storage}\" Exceptions_Items=\"{InventoryChecks.Exceptions_Items}\" />");
                sw.WriteLine($"        <Tool Name=\"Player_Infos_Webhook\" Enable=\"{DiscordWebhookSender.PlayerInfosEnabled}\" Webhook_Url=\"{DiscordWebhookSender.PlayerInfosWebHookUrl}\" Connected_Color=\"{DiscordWebhookSender.PlayerConnectedColor.r},{DiscordWebhookSender.PlayerConnectedColor.g},{DiscordWebhookSender.PlayerConnectedColor.b}\" Disconnected_Color=\"{DiscordWebhookSender.PlayerDisconnectedColor.r},{DiscordWebhookSender.PlayerDisconnectedColor.g},{DiscordWebhookSender.PlayerDisconnectedColor.b}\" />");
                sw.WriteLine($"        <Tool Name=\"Player_Invulnerability_At_Trader\" Enable=\"{PlayerInvulnerabilityAtTrader.IsEnabled}\" />");
                sw.WriteLine($"        <Tool Name=\"Reset_Regions\" Enable=\"{ResetRegions.IsEnabled}\" Day=\"{ResetRegions.Day}\" Hour=\"{ResetRegions.Hour}\" Buff_Reset_Zones=\"{Zones.BuffResetZone}\" />");
                sw.WriteLine($"        <Tool Name=\"Server_Infos_Webhook\" Enable=\"{DiscordWebhookSender.ServerInfosEnabled}\" Webhook_Url=\"{DiscordWebhookSender.ServerInfosWebHookUrl}\" Connected_Color=\"{DiscordWebhookSender.ServerOnlineColor.r},{DiscordWebhookSender.ServerOnlineColor.g},{DiscordWebhookSender.ServerOnlineColor.b}\" Disconnected_Color=\"{DiscordWebhookSender.ServerOfflineColor.r},{DiscordWebhookSender.ServerOfflineColor.g},{DiscordWebhookSender.ServerOfflineColor.b}\" />");
                sw.WriteLine($"        <Tool Name=\"Sanctions_Webhook\" Enable=\"{DiscordWebhookSender.SanctionsEnabled}\" Webhook_Url=\"{DiscordWebhookSender.SanctionsWebHookUrl}\" Color=\"{DiscordWebhookSender.SanctionsColor.r},{DiscordWebhookSender.SanctionsColor.g},{DiscordWebhookSender.SanctionsColor.b}\"/>");
                sw.WriteLine($"        <Tool Name=\"Spectator_Detector\" Enable=\"{PlayerChecks.SpectatorEnabled}\" Admin_Level=\"{PlayerChecks.Spectator_Admin_Level}\" />");
                sw.WriteLine($"        <Tool Name=\"TP_Command\" Enable=\"{ChatCommandTP.IsEnabled}\" TP_Cost=\"{ChatCommandTP.TPCost}\" TP_Max_Count=\"{ChatCommandTP.TPMaxCount}\" />");
                sw.WriteLine($"        <Tool Name=\"Vote_Command\" Enable=\"{ChatCommandVote.IsEnabled}\" Gain_Per_Vote=\"{ChatCommandVote.GainPerVote}\" API_Server_Token=\"{ChatCommandVote.APIServerToken}\" />");
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
        }

        public static void UpgradeXml(XmlNodeList _oldRootNodes)
        {
            try
            {
                FileWatcher.EnableRaisingEvents = false;
                if (File.Exists(ConfigFilePath))
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
                MSNUtils.LogError($"Error in Config.UpgradeXml: {e.Message}");
            }
            FileWatcher.EnableRaisingEvents = true;
        }

        static bool TryParseBool(string value, out bool result, string nameAttribute, string paramName, XmlNode node)
        {
            bool parse = bool.TryParse(value, out result);
            if (!parse)
                MSNUtils.LogWarning($"Ignoring {nameAttribute} entry in {ConfigFile} because of invalid (True/False) value for '{paramName}' attribute: {node.OuterXml}");
            return parse;
        }

        static bool TryParseInt(string value, out int result, string nameAttribute, string paramName, XmlNode node)
        {
            bool parse = int.TryParse(value, out result);
            if (!parse)
                MSNUtils.LogWarning($"Ignoring {nameAttribute} entry in {ConfigFile} because of invalid (non-numeric) value for '{paramName}' attribute: {node.OuterXml}");
            return parse;
        }

        static bool TryParseString(string value, out string result, string nameAttribute, string paramName, XmlNode node)
        {
            if (value.Length != 0)
            {
                result = value;
                return true;
            }
            else
            {
                result = string.Empty;
                MSNUtils.LogWarning($"Ignoring {nameAttribute} entry in {ConfigFile} because of invalid string value for '{paramName}' attribute: {node.OuterXml}");
                return false;
            }
        }

        static bool TryParseList(string value, out List<string> result, string nameAttribute, string paramName, XmlNode node)
        {
            if (value.Length != 0)
            {
                string[] array = value.Split(new string[] { ",", " ,", ", ", " , " }, StringSplitOptions.RemoveEmptyEntries);
                List<string> list = new List<string>();
                foreach (string item in array)
                {
                    list.Add(item);
                }
                result = list;
                return true;
            }
            else
            {
                result = new List<string>();
                MSNUtils.LogWarning($"Ignoring {nameAttribute} entry in {ConfigFile} because of invalid string value for '{paramName}' attribute: {node.OuterXml}");
                return false;
            }
        }

        static MSNLocalization.Language TryParseLanguage(string value)
        {
            if (value.EqualsCaseInsensitive("french") || value.EqualsCaseInsensitive("english"))
            {
                if (value.EqualsCaseInsensitive("french"))
                {
                    return MSNLocalization.Language.French;
                }
                else
                {
                    return MSNLocalization.Language.English;
                }
            }
            else
            {
                return MSNLocalization.Language.French;
            }
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
                MSNUtils.LogWarning($"Ignoring {nameAttribute} entry in {ConfigFile} because of invalid url value for '{paramName}' attribute: {node.OuterXml}");
                result = string.Empty;
                return false;
            }
        }

        static bool TryParseColor(string value, out Color32 color, string nameAttribute, string paramName, XmlNode node)
        {
            try
            {
                string pattern = @"^(?:(?:^|,\s?)([01]?\d\d?|2[0-4]\d|25[0-5])){3}$";
                Match m = Regex.Match(value, pattern);
                if (m.Success)
                {
                    string[] rgb = value.Split(',');
                    byte r = byte.Parse(rgb[0]);
                    byte g = byte.Parse(rgb[1]);
                    byte b = byte.Parse(rgb[2]);
                    Color32 _color = new Color32(r, g, b, 0);
                    color = _color;
                    return true;
                }
                else
                {
                    MSNUtils.LogWarning($"Ignoring {nameAttribute} entry in {ConfigFile} because of invalid color value for '{paramName}' attribute: {node.OuterXml}");
                    color = Color.white;
                    return false;
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in Config.TryParseColor: {e.Message}");
            }
            color = Color.white;
            return false;
        }
    }
}