using System;
using System.Collections.Generic;
using System.Linq;
using Algenta.Globalization.LanguageTags;

namespace GmWeb.Web.RHI.Utility
{
    public static class WebControlData
    {
        public static Dictionary<int, string> GetBinaryData()
        {
            return new Dictionary<int, string>()
            {
                {1,"Yes"},
                {2,"No"}
            };
        }
        public static Dictionary<int, string> GetJobCategories()
        {
            return new Dictionary<int, string>()
            {
                {1,"Senior Management/Leadership (CEO, Executive Director, Senior/Executive VP, Chief level positions, etc)"},
                {2,"Upper management (Director, VP, Duty chief, etc)"},
                {3,"Middle management"},
                {4,"Lower management"},
                {5,"Leader/foreman"},
                {6,"Contributor/team member"}
            };
        }
        public static Dictionary<int, string> GetCurrentWorkStatus()
        {
            return new Dictionary<int, string>()
            {
                {1,"Working full time"},
                {2,"Working part time"},
                {3,"Full-time homemaker"},
                {4,"Retired"},
                {5,"On disability"},
                {6,"On paid leave"},
                {7,"On unpaid leave"},
                {8,"Unemployed or laid off and looking for work"},
                {9,"Unemployed and not looking for work"},
                {10,"Other. Please specify"}
            };
        }
        public static Dictionary<int, string> GetCurrentOccupation()
        {
            return new Dictionary<int, string>()
            {
                {1,"Police officer"},
                {2,"Fire fighter"},
                {3,"Paramedic"},
                {4,"Emergency Medical Technician (EMT)"},
                {5,"Correctional officer"},
                {6,"Other. Please specify"}
            };
        }
        public static Dictionary<int, string> GetInsuranceCoverage()
        {
            return new Dictionary<int, string>()
            {
                {1,"Uninsured"},
                {2,"Insured but paying out of pocket"},
                {3,"Employer health insurance"},
                {4,"Private-paid health insurance"},
                {5,"Military health insurance"},
                {6,"Medi-Cal"},
                {7,"Medicare"},
                {8,"Health saving account (HSA)"},
                {9,"Other. Please specify"},
            };
        }
        public static Dictionary<int, string> GetMartialStatus()
        {
            return new Dictionary<int, string>()
            {
                {1,"Married"},
                {2,"Living with a partner"},
                {3,"Widowed"},
                {4,"Divorced"},
                {5,"Separated"},
                {6,"Never married"}
            };
        }
        public static Dictionary<int, string> GetSexualOrientation()
        {
            return new Dictionary<int, string>()
            {
                {1,"Straight"},
                {2,"Lesbian"},
                {3,"Gay"},
                {4,"Bisexual"},
                {5,"Assexual"},
                {6,"Questioning"},
                {7,"I rather not say"},
                {8,"Other - Please specify"}
            };
        }
        public static Dictionary<int, string> GetRacialList()
        { 
            return new Dictionary<int, string>()
            {
                {   1   ,"	Asian non-Hispanic or Latino	"},
                {   2   ,"	White and Hispanic or Latino	"},
                {   3   ,"	Asian non-Hispanic or Latino	"},
                {   4   ,"	Asian non-Hispanic or Latino	"},
                {   5   ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   6   ,"	White and Hispanic or Latino	"},
                {   7   ,"	White and Hispanic or Latino	"},
                {   8   ,"	Asian non-Hispanic or Latino	"},
                {   9   ,"	Asian non-Hispanic or Latino	"},
                {   10  ,"	Asian non-Hispanic or Latino	"},
                {   11  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   12  ,"	White and Hispanic or Latino	"},
                {   13  ,"	Asian non-Hispanic or Latino	"},
                {   14  ,"	White and Hispanic or Latino	"},
                {   15  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   16  ,"	White and Hispanic or Latino	"},
                {   17  ,"	White and Hispanic or Latino	"},
                {   18  ,"	White and Hispanic or Latino	"},
                {   19  ,"	Asian non-Hispanic or Latino	"},
                {   20  ,"	White and Hispanic or Latino	"},
                {   21  ,"	White and Hispanic or Latino	"},
                {   22  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   23  ,"	Asian non-Hispanic or Latino	"},
                {   24  ,"	White and Hispanic or Latino	"},
                {   25  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   26  ,"	White and Hispanic or Latino	"},
                {   27  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   28  ,"	White and Hispanic or Latino	"},
                {   29  ,"	White and Hispanic or Latino	"},
                {   30  ,"	White and Hispanic or Latino	"},
                {   31  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   32  ,"	Asian non-Hispanic or Latino	"},
                {   33  ,"	Asian non-Hispanic or Latino	"},
                {   34  ,"	Asian non-Hispanic or Latino	"},
                {   35  ,"	Asian non-Hispanic or Latino	"},
                {   36  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   37  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   38  ,"	Asian non-Hispanic or Latino	"},
                {   39  ,"	Asian non-Hispanic or Latino	"},
                {   40  ,"	Asian non-Hispanic or Latino	"},
                {   41  ,"	Asian non-Hispanic or Latino	"},
                {   42  ,"	Asian non-Hispanic or Latino	"},
                {   43  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   44  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   45  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   46  ,"	White and Hispanic or Latino	"},
                {   47  ,"	White and Hispanic or Latino	"},
                {   48  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   49  ,"	Asian non-Hispanic or Latino	"},
                {   50  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   51  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   52  ,"	Asian non-Hispanic or Latino	"},
                {   53  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   54  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   55  ,"	White and Hispanic or Latino	"},
                {   56  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   57  ,"	Asian non-Hispanic or Latino	"},
                {   58  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   59  ,"	White and Hispanic or Latino	"},
                {   60  ,"	White and Hispanic or Latino	"},
                {   61  ,"	White and Hispanic or Latino	"},
                {   62  ,"	White and Hispanic or Latino	"},
                {   63  ,"	White and Hispanic or Latino	"},
                {   64  ,"	White and Hispanic or Latino	"},
                {   65  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   66  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   67  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   68  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   69  ,"	Asian non-Hispanic or Latino	"},
                {   70  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   71  ,"	Asian non-Hispanic or Latino	"},
                {   72  ,"	Asian non-Hispanic or Latino	"},
                {   73  ,"	Asian non-Hispanic or Latino	"},
                {   74  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   75  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   76  ,"	Asian non-Hispanic or Latino	"},
                {   77  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   78  ,"	White and Hispanic or Latino	"},
                {   79  ,"	Asian non-Hispanic or Latino	"},
                {   80  ,"	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	"},
                {   81  ,"	White and Hispanic or Latino	"},
                {   82  ,"	Asian non-Hispanic or Latino	"},
                {   83  ,"	White non-Hispanic or Latino	"},
                {   84  ,"	Asian and Hispanic or Latino	"},
                {   85  ,"	Black or African America and Hispanic or Latino	"},
                {   86  ,"	Black or African America non-Hispanic or Latino	"},
                {   87  ,"	American Indian and Alaska Native and Hispanic or Latino	"},
                {   88  ,"	Native Hawaiian and Other Pacific Islander and Hispanic or Latino	"},
                {   89  ,"	American Indian and Alaska Native non-Hispanic or Latino	"},
                {   90  ,"	Other	"},

            };
        }
        public static Dictionary<int, string> GetEthnicityList()
        { 
            return new Dictionary<int, string>()
            {
                {   1   ,"	Afghan	"},
                {   2   ,"	Argentinan	"},
                {   3   ,"	Bangladeshi	"},
                {   4   ,"	Bhutanese	"},
                {   5   ,"	Bismarch Arch. Islander	"},
                {   6   ,"	Bolivian	"},
                {   7   ,"	Brazilian	"},
                {   8   ,"	Bruneian	"},
                {   9   ,"	Burmese	"},
                {   10  ,"	Cambodian	"},
                {   11  ,"	Caroline Islander	"},
                {   12  ,"	Chilean	"},
                {   13  ,"	Chinese	"},
                {   14  ,"	Colombian	"},
                {   15  ,"	Cook Islanders	"},
                {   16  ,"	Costa Rican	"},
                {   17  ,"	Cuban	"},
                {   18  ,"	Dominican	"},
                {   19  ,"	East Timorese	"},
                {   20  ,"	Ecuadorean	"},
                {   21  ,"	El Salvadorian	"},
                {   22  ,"	Fijian	"},
                {   23  ,"	Fillipino	"},
                {   24  ,"	French Guiana 	"},
                {   25  ,"	French Polynesian	"},
                {   26  ,"	Guadeloupean	"},
                {   27  ,"	Guamanian/Chamorro	"},
                {   28  ,"	Guatemalan	"},
                {   29  ,"	Haitian	"},
                {   30  ,"	Honduran	"},
                {   31  ,"	I-Kiribati	"},
                {   32  ,"	Indian	"},
                {   33  ,"	Indonesian	"},
                {   34  ,"	Japanese	"},
                {   35  ,"	Kazakhstani	"},
                {   36  ,"	Kermadec Islander	"},
                {   37  ,"	Kiribatian	"},
                {   38  ,"	Korean	"},
                {   39  ,"	Kyrgyz/Kyrgyzstani/Kirghiz	"},
                {   40  ,"	Laotian	"},
                {   41  ,"	Malaysian	"},
                {   42  ,"	Maldivian	"},
                {   43  ,"	Mangarevan	"},
                {   44  ,"	Maori	"},
                {   45  ,"	Marshallese	"},
                {   46  ,"	Martiniquean	"},
                {   47  ,"	Mexican	"},
                {   48  ,"	Micronesian	"},
                {   49  ,"	Mongolian	"},
                {   50  ,"	Native Hawaiian	"},
                {   51  ,"	Nauruan	"},
                {   52  ,"	Nepalese	"},
                {   53  ,"	New Caledonian	"},
                {   54  ,"	New Guinean	"},
                {   55  ,"	Nicaraguan	"},
                {   56  ,"	Niueans	"},
                {   57  ,"	Pakistani	"},
                {   58  ,"	Palauan	"},
                {   59  ,"	Panamanian	"},
                {   60  ,"	Paraguayan	"},
                {   61  ,"	Peruvian	"},
                {   62  ,"	Puerto Rican	"},
                {   63  ,"	Saint Barthélemy 	"},
                {   64  ,"	Saint Martin 	"},
                {   65  ,"	Saipanese	"},
                {   66  ,"	Samoan	"},
                {   67  ,"	Samoan	"},
                {   68  ,"	Santa Cruz Islander	"},
                {   69  ,"	Singaporean	"},
                {   70  ,"	Solomon Islander	"},
                {   71  ,"	Sri Lankan	"},
                {   72  ,"	Tajik/Tadzhik	"},
                {   73  ,"	Thai	"},
                {   74  ,"	Tokelauans	"},
                {   75  ,"	Tongan	"},
                {   76  ,"	Turkmen	"},
                {   77  ,"	Tuvaluan	"},
                {   78  ,"	Uruguayan	"},
                {   79  ,"	Uzbek/Uzbekistani	"},
                {   80  ,"	Vanuatuan	"},
                {   81  ,"	Venezuelan	"},
                {   82  ,"	Vietnamese	"},
                {   83  ,"	Other/Not Listed	"}

            };
        }
        public static Dictionary<int, string> GetReligionList()
        { 
            return new Dictionary<int, string>()
            {
                {1,"Atheist"},
                {2,"Christian/Eastern Orthodox"},
                {3,"Christian/Catholic"},
                {4,"Christian/Proto-Protestant"},
                {5,"Christian/Lutheran"},
                {6,"Christian/Presbyterianism"},
                {7,"Christian/Anglican"},
                {8,"Christian/Anabaptist"},
                {9,"Christian/Methodist"},
                {10,"Christian/Seventh day Adventist"},
                {11,"Christian/Quaker"},
                {12,"Christian/Plymouth Brethren"},
                {13,"Christian/Irvingist"},
                {14,"Christian/Pentecostal"},
                {15,"Christian/Evangelical"},
                {16,"Christian/Eastern Protestant"},
                {17,"Christian/Mormonism"},
                {18,"Islam"},
                {19,"Buddhism"},
                {20,"Hinduism"},
                {21,"Judaism"}
            };
        }
        public static Dictionary<int, string> GetIntList()
        { 
            return new Dictionary<int, string>()
            {
                { 1, "1"},
                { 2, "2"},
                { 3, "3"},
                { 4, "4"},
                { 5, "5"},
                { 6, "6"},
                { 7, "7"},
                { 8, "8"},
                { 9, "9"},
                { 10, "10"},
                { 11, "11"},
                { 12, "12"},
                { 13, "13"},
                { 14, "14"},
                { 15, "15"},
                { 16, "16"},
                { 17, "17"},
                { 18, "18"},
                { 19, "19"},
                { 20, "20"}
            };
        }
        public static Dictionary<int, string> GetEducationList()
        { 
            return new Dictionary<int, string>()
            {
                { 1, "5 years K-5"},
                { 2, "12 years K-6 to High School dipolma/GED"},
                { 3, "14 years Associate's degree/certificate"},
                { 4, "16 years Bachelor's degree"},
                { 5, "18 years Master's degree"},
                { 6, "19+ year Ph.D. or advanced professional degree"},
                { 7, "Postdoctoral"}
            };
        }
        public static Dictionary<int, string> GetIncomeList()
        {
            return new Dictionary<int, string>()
            {
                { 1, "$0 - $15,000"},
                { 2, "$15,001 - $25,000"},
                { 3, "$25,001 - $35,000"},
                { 4, "$35,001 - $45,000"},
                { 5, "$45,001 - $60,000"},
                { 7, "$60,001 - $75,000"},
                { 8, "$75,001 - $90,000"},
                { 9, "$90,001 - $110,000"},
                { 10, "$110,001 - $150,000"},
                { 11, "$150,001 - $200,000"},
                { 12, "$200,001 - $250,000"},
                { 13, "$250,001 +"},
                { 14, "Prefer not to answer"}
            };
        }
        public static Dictionary<int, string> GetMedicalData()
        {
            return new Dictionary<int, string>()
            {
                { 1, "State"},
                { 2, "Insurance Provider"},
                { 3, "Member Number"},
                { 4, "Coverage renewal date"},
                { 5, "Plan/Plan Name"},
                { 7, "Member ID Number"},
                { 8, "Group ID"},
                { 9, "Primary Care Provier"}
            };
        }

        public static Dictionary<int, string> GetMedicareData()
        {
            return new Dictionary<int, string>()
            {
                { 1, "Medicare Part A"},
                { 2, "Medicare Part B"},
                { 3, "Medicare Parts A & B"},
                { 4, "Medicare Advantage plan / Medicare Part C / MA Plan"},
                { 5, "Medicare Advantage plan / Medicare Part C / MA Plan WITH Part D"},
                { 7, "Medicare Part D"},
                { 8, "Medicare Supplement (Medigap)"}
            };
        }

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
        public static Dictionary<string,string> GetLangData()
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
