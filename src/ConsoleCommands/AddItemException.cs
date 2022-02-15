using System;
using System.Collections.Generic;
using MSNTools.PersistentData;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace MSNTools.ConsoleCommands
{
    public class ItemException : ConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                var actualExceptions = InventoryChecks.Exceptions_Items;
                if (_params.Count == 2)
                {
                    if (_params[0] == "add")
                    {
                        AddItemExceptionMethod(_params[1]);
                    }
                    else if (_params[0] == "remove" || _params[0] == "del")
                    {
                        RemoveItemExceptionMethod(_params[1]);
                    }
                }
                else
                {
                    if (_params.Count == 1 && _params[0] == "list")
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output(GetListExceptionsMethod());
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError("Execute " + e.Message);
            }
        }

        string GetListExceptionsMethod()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(Config.ConfigFilePath);
                foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes[2])
                {
                    if (xmlNode.Attributes[0].Value == "Inventory_Check")
                    {
                        foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                        {
                            if (xmlAttribute.Name == "Exceptions_Items")
                            {
                                string[] vs = xmlAttribute.Value.Split(',');

                                StringBuilder stringBuilder = new StringBuilder();
                                foreach (string v in vs)
                                {
                                    if (v.Length > 0)
                                        stringBuilder.Append($"{v}\n");
                                }
                                stringBuilder.ToString().TrimEnd('\n');
                                return stringBuilder.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in ItemException.GetListExceptionsMethod: {e.Message}");
            }
            return string.Empty;
        }

        void AddItemExceptionMethod(string item)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(Config.ConfigFilePath);
                foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes[2])
                {
                    if (xmlNode.Attributes[0].Value == "Inventory_Check")
                    {
                        foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                        {
                            if (xmlAttribute.Name == "Exceptions_Items")
                            {
                                string[] vs = xmlAttribute.Value.Split(',');
                                if (!vs.Contains(item))
                                {
                                    xmlAttribute.Value += $",{item}";
                                    xmlDocument.Save(Config.ConfigFilePath);
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{item} successfully added to the exceptions list.");
                                }
                                else
                                {
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{item} already exist in the exceptions list.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in ItemException.AddItemExceptionMethod: {e.Message}");
            }
        }

        void RemoveItemExceptionMethod(string item)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(Config.ConfigFilePath);
                foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes[2])
                {
                    if (xmlNode.Attributes[0].Value == "Inventory_Check")
                    {
                        foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                        {
                            if (xmlAttribute.Name == "Exceptions_Items")
                            {
                                string[] vs = xmlAttribute.Value.Split(',');
                                if (vs.Contains(item))
                                {
                                    StringBuilder stringBuilder = new StringBuilder();
                                    foreach (string v in vs)
                                    {
                                        if (v != item)
                                            stringBuilder.Append($"{v},");
                                    }
                                    stringBuilder.ToString().TrimEnd(',');
                                    xmlAttribute.Value = stringBuilder.ToString();
                                    xmlDocument.Save(Config.ConfigFilePath);
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{item} successfully removed to the exceptions list.");
                                }
                                else
                                {
                                    SingletonMonoBehaviour<SdtdConsole>.Instance.Output($"{item} doesn't exist in the exceptions list.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in ItemException.RemoveItemExceptionMethod: {e.Message}");
            }
        }

        public override string[] GetCommands() => new string[] { "additemexception", "aie" };

        public override string GetDescription() => "...";
    }
}
