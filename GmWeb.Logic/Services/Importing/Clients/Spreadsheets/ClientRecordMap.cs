using GmWeb.Logic.Utility.Csv;
using GmWeb.Logic.Utility.Extensions.Expressions;
using GmWeb.Logic.Utility.Extensions.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;

namespace GmWeb.Logic.Services.Importing.Clients.Spreadsheets;

public class ClientRecordMap : CsvRecordMap<ClientRecord, ClientRecordMap>
{
    public ClientRecordMap()
    {
        AutoMap();
    }
}
