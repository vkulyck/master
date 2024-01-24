using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Data.Models;
using GmWeb.Web.Profile.Models.Shared;

namespace GmWeb.Web.Profile.Models.Waitlist
{
    public class ClientQuery : EditableFlowModelBase
    {
        public string VariableName { get; set; }
        public override string EditorNameField => "VariableName";
        public override string EditorTitle => "Client Query";
        public int DataSourceID { get; set; }
        public ClientResponse ClientResponse { get; set; }
        public string RequestText { get; set; }
        public string LookupTableName { get; set; }
    }
}