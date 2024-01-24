using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Profile.Models.Shared
{
    public abstract class BaseEntityViewModel : GmWeb.Logic.Interfaces.IDataRowModel
    {
        public abstract void CopyRowData(DataRow row);
    }
}