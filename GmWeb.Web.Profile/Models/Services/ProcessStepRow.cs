using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Profile.Models.Services
{
    public class ProcessStepRow
    {
        public string ActionText { get; set; }
        public string ActionUri { get; set; }
        public string ProcessStepName { get; set; }
        public string ProcessStepDescription { get; set; }
        public string EntityName { get; set; }
        public bool IsCompleted { get; set; }
        public string DocumentName { get; set; }
        public DateTime DocumentRecordDate { get; set; }
        public string Comments { get; set; }
        public int GroupNumber { get; set; }
    }
}