using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace GmWeb.Web.Profile.Models.Services
{
    public class ProcessStepModalViewModel : GmWeb.Web.Common.Models.BasePageViewModel
    {
        [DisplayName("Process type:")]
        public string ProcessTypeValue { get; set; }

        [DisplayName("Description:")]
        public string Description { get; set; }

        public bool IsChecked { get; set; }

        [DisplayName("Date document recorded:")]        
        public DateTime DateRecorded { get; set; }

        [DisplayName("Document comments:")]
        public string DocumentComments { get; set; }

        [DisplayName("Document loaded:")]
        public string DocumentPath { get; set; }

        [DisplayName("Document:")]
        public string AGUploadAgencyDocumentation { get; set; }
        public string ErrorStep { get; set; }
    }
}