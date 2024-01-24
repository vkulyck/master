using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Interfaces;

namespace GmWeb.Web.Common.Models.Shared
{
    public interface IEditableViewModel : IViewModel
    {
        string EditorNameField { get; }
        string EditorTitle { get; }
        bool IsNameMultiline { get; }
    }
}
