using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using NHunspell;


namespace G.Extensions
{
    public static class Spelling
    {
        public static IEnumerable<string> Suggestions(string input)
        {
            var domain = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = ConfigurationManager.AppSettings["SpellDictionary"];
            var fullPath = domain + configPath;
            if (File.Exists(fullPath + "/en_us.aff") && File.Exists(fullPath + "/en_us.dic"))
            {
                using (Hunspell hunspell = new Hunspell(fullPath + "/en_us.aff", fullPath + "/en_us.dic"))
                {
                    return hunspell.Suggest(input);
                }
            }
            throw new FileNotFoundException("Method: Suggestions " + fullPath);
        }

        public static bool SpellCheck(string input)
        {
            var domain = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = ConfigurationManager.AppSettings["SpellDictionary"];
            var fullPath = domain + configPath;
            if (File.Exists(fullPath + "/en_US.aff") && File.Exists(fullPath + "/en_US.dic"))
            {
                using (Hunspell hunspell = new Hunspell(fullPath + "/en_US.aff", fullPath + "/en_US.dic"))
                {
                    bool result = hunspell.Spell(input);
                    return result;
                }
            }
                throw new FileNotFoundException("Method: SpellCheck " + fullPath);
        }

        public static Dictionary<string, List<string>> ThesaurusEntries(string word)
        {
            var result = new Dictionary<string, List<string>>();
            var _synList = new List<string>();
            var domain = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = ConfigurationManager.AppSettings["SpellDictionary"];
            var fullPath = domain + configPath;
            var thes = new MyThes(fullPath + "/th_en_us_new.dat");
            if (File.Exists(fullPath + "/en_us.aff") && File.Exists(fullPath + "/en_us.dic")) {
                using (Hunspell hunspell = new Hunspell(fullPath + "/en_us.aff", fullPath + "/en_us.dic"))
                {
                    var thesaurus = thes.Lookup(word, hunspell);
                    if (thesaurus != null)
                    {
                        foreach (KeyValuePair<string, List<ThesMeaning>> entry in thesaurus.GetSynonyms())
                        {
                            _synList.Add(entry.Key);
                        }
                    }
                    result.Add("Thesaurus", _synList);
                    return result;
                }
            }
            throw new FileNotFoundException("Method: ThesaurusEntries " + fullPath);

        }
    }
}