using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Web.Common.Models.Shared;

namespace GmWeb.Web.Profile.Models.Waitlist
{
    public abstract class EditableFlowModelBase : IEditableViewModel
    {
        public string Guid { get; set; } = GmWeb.Logic.Utility.Extensions.ModelExtensions.GenerateGuid();
        public virtual string EditorTitle => null;
        public virtual string EditorNameField => "Name";
        public virtual bool IsNameMultiline => false;
    }
}