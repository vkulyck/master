using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Services.Deltas;

namespace GmWeb.Logic.Services.Datasets.Languages;

public class LanguageOptions : ServiceOptions
{
    public List<string[]> PrimaryLanguages { get; set; } = new List<string[]>();
    public List<string[]> ExtendedLanguages { get; set; } = new List<string[]>();
    public List<string> ChineseLanguages { get; set; } = new List<string>();
}
