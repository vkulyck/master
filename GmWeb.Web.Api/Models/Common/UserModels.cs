using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GmWeb.Logic.Utility.Extensions.Collections;
using GenderType = GmWeb.Logic.Enums.Gender;
using PrimaryLanguages = GmWeb.Logic.Services.Datasets.Languages.PrimaryLanguages;
using UserRoleType = GmWeb.Logic.Enums.UserRole;
using IPerson = GmWeb.Logic.Interfaces.IPerson;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;
using GmWeb.Web.Common.Models.Carma;

namespace GmWeb.Web.Api.Models.Common
{
    public class UserDetailsDTO : UserDTO
    {
        public List<ActivityDTO> Activities { get; set; }
        public List<NoteDTO> Notes { get; set; }
    }
}
