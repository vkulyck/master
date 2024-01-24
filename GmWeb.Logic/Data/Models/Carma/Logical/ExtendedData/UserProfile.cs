using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Data.Models.Carma.ExtendedData;
using GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;
using Newtonsoft.Json;

namespace GmWeb.Logic.Data.Models.Carma.ExtendedData;

public class UserProfile
{
    public Residence Residence { get; set; } = new Residence();
    public List<IntakeData> IntakeHistory { get; set; } = new();
    public IntakeData PendingIntakeData { get; set; }
    [JsonIgnore]
    public IntakeData CurrentIntakeData
    {
        get => this.PendingIntakeData ?? this.IntakeHistory.FirstOrDefault();
        set => this.PendingIntakeData = value;
    }
}
