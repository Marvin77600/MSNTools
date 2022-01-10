﻿using System.Collections.Generic;
using System.Xml;

public static class MSNLocalization
{
    public static Dictionary<string, string> frenchLocalizations;
    public static Dictionary<string, string> englishLocalizations;

    public static void Init()
    {
        LoadXML();
    }

    private static void LoadXML()
    {
        frenchLocalizations = new Dictionary<string, string>();
        englishLocalizations = new Dictionary<string, string>();
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(ModManager.GetMod("MSNTools").Path + "/MSNLocalizations.xml");

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

    public enum Language
    {
        French,
        English
    }
}