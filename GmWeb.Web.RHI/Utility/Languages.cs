using System;
using System.Collections.Generic;
using System.Linq;
using Algenta.Globalization.LanguageTags;

namespace GmWeb.Web.RHI.Utility
{
    public static class Languages
    {
        ////https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        private static readonly List<string> _langCodes = new List<string>()
        {
            "aa","af","am","an","as","bm","ba","eu","be","bs","br","bg","ca","ch","ce","ny","cv","kw","co","hr","cs","da","dv","dz","ee","fo","fj","fi","gl","ka","gu","ht","ha","hz",
            "ho","hu","ga","ig","is","jv","kl","kn","ks","kk","ki","rw","ky","kj","lb","lg","li","ln","lo","lt","gv","mk","ml","mt","mh","nv","ng","oc","os","pa","rm","sd",
            "se","sm","sg","gd","sn","si","sk","so","st","su","sv","ta","te","tg","th","ti","tk","tn","tr","ts","tt","ug","uk","ur","ve","wa","cy","wo","xh","yo","zu","ab","av","bn","fy","km",
            "rn","lu","mi","mr","my","na","nd","sl","nr","bo","ss","to","ae","la","sa","pi","cu","vo","io","ie","ia","eo","tw","id","nb","nn","nl","el","hy","bi","ay","cr","et","gn","ik","iu","kr",
            "kv","kg","ku","lv","mg","mn","ne","om","ps","qu","sc","sw","uz","za","az","fa","ff","or","oj","no","sq","ms","ak","yi","he","ty","ii","ro","sr"
        };

        private static readonly List<string> _primarylangCodes = new List<string>()
        {
            "en","es", "zh", "fr","tl","vi","ko","de", "ar","ru","it","pt","hi", "pl", "ja"
        };
        private static Dictionary<string,string> _codeLanguage = new();
        private static Dictionary<string, string> _addCodeLanguage = new();

        private static readonly List<string> _chiniseDialect = new List<string>()
        {
            "Mandarin", "Cantonese", "Min", "Gan", "Hakka", "Xiang", "Wu", "Jin"
        };
        public static Dictionary<string, string> GetAddLangData(string languageID)
        {
            _addCodeLanguage.Clear();
            //if (_addCodeLanguage.Count == 0)
            //{
                foreach (var langCode in _langCodes)
                {
                    _addCodeLanguage.Add(langCode, TagRegistry.GetLanguage(langCode).Descriptions.First());
                }

            //}
            if (languageID == "Other")
                return _addCodeLanguage;

            if (languageID == "Chinese")
            {
                _addCodeLanguage.Clear();
                foreach (var chiniseCode in _chiniseDialect)
                {
                    _addCodeLanguage.Add(chiniseCode, chiniseCode);
                }
            }
            return _addCodeLanguage;
        }
        public static Dictionary<string,string> GetData()
        {
            if (_codeLanguage.Count == 0)
            {
                foreach (var langCode in _primarylangCodes)
                {
                    _codeLanguage.Add(langCode, TagRegistry.GetLanguage(langCode).Descriptions.First());
                }
                _codeLanguage.Add("Other", "Other");

            }
            return _codeLanguage;
        }
    }
}
