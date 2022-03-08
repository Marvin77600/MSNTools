using System.Collections.Generic;
using System.Xml;
using MSNTools;

public static class MSNLocalization
{
    public static Dictionary<string, string> frenchLocalizations;
    public static Dictionary<string, string> englishLocalizations;

    public static void Init() => LoadXML();

    private static void LoadXML()
    {
        frenchLocalizations = new Dictionary<string, string>();
        englishLocalizations = new Dictionary<string, string>();
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(API.Mod.Path + "/MSNLocalizations.xml");

        foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
        {
            string key = xmlNode.Attributes["key"].Value;
            string french = xmlNode.Attributes["french"].Value;
            string english = xmlNode.Attributes["english"].Value;

            frenchLocalizations.Add(key, french);
            englishLocalizations.Add(key, english);
        }
    }

    public static string Get(string key, Language language)
    {
        if (language == Language.French)
        {
            if (frenchLocalizations.TryGetValue(key, out string french))
                return french;
        }
        if (language == Language.English)
        {
            if (englishLocalizations.TryGetValue(key, out string english))
                return english;
        }
        return key;
    }

    public static string Get(string key, Language language, params object[] parameters)
    {
        int i = parameters.Length;
        if (language == Language.French)
        {
            if (frenchLocalizations.TryGetValue(key, out string french))
            {
                for (int index = 0; index < i; index++)
                {
                    french = french.Replace("{" + index + "}", parameters[index].ToString());
                }
                return french;
            }
        }
        if (language == Language.English)
        {
            if (englishLocalizations.TryGetValue(key, out string english))
            {
                for (int index = 0; index < i; index++)
                {
                    english = english.Replace("{" + index + "}", parameters[index].ToString());
                }
                return english;
            }
        }
        return key;
    }

    public enum Language
    {
        French,
        English
    }
}