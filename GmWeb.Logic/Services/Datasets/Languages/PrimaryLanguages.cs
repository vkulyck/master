using System;
using System.Collections.Generic;
using System.Linq;
using Algenta.Globalization.LanguageTags;

namespace GmWeb.Logic.Services.Datasets.Languages
{
    public static class PrimaryLanguages
    {
        // Full list of codes: https://datahub.io/core/language-codes/r/3.html
        public static LanguageTag Arabic { get; private set; }
        public static LanguageTag Chinese { get; private set; }
        public static LanguageTag English { get; private set; }
        public static LanguageTag French { get; private set; }
        public static LanguageTag German { get; private set; }
        public static LanguageTag Hindi { get; private set; }
        public static LanguageTag Italian { get; private set; }
        public static LanguageTag Japanese { get; private set; }
        public static LanguageTag Korean { get; private set; }
        public static LanguageTag Polish { get; private set; }
        public static LanguageTag Portuguese { get; private set; }
        public static LanguageTag Russian { get; private set; }
        public static LanguageTag Spanish { get; private set; }
        public static LanguageTag Tagalog { get; private set; }
        public static LanguageTag Vietnamese { get; private set; }
        public static string OtherLanguageTag = "other";


        private static readonly List<LanguageTag> _tags = new List<LanguageTag>();
        public static IEnumerable<LanguageTag> AllTags => _tags;
        public static IEnumerable<string> AllCodes => AllTags.Select(x => x.Value);
        static PrimaryLanguages()
        {
            Arabic = init("ar");
            Chinese = init("zh");
            English = init("en");
            French = init("fr");
            German = init("de");
            Hindi = init("hi");
            Italian = init("it");
            Japanese = init("ja");
            Korean = init("kg");
            Polish = init("pl");
            Portuguese = init("pt");
            Russian = init("ru");
            Spanish = init("es");
            Tagalog = init("tl");
            Vietnamese = init("vi");
        }

        private static LanguageTag init(string tag)
        {
            if (!TagRegistry.TryParse(tag, out LanguageTag parsed))
                throw new Exception($"Error initializing language: {tag}");
            _tags.Add(parsed);
            return parsed;
        }
    }
}
