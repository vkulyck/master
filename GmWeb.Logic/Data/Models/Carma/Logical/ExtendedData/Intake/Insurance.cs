using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;

public record class InsuranceData
{
    public int? MedicalInsurance { get; set; }
    public string EmployerName { get; set; }
    public string InsuranceProvider { get; set; }
    public string GroupNumber { get; set; }
    public string SubscriberNumber { get; set; }
    public string PrimaryHolder { get; set; }
    public string HolderRelation { get; set; }
    public string MedicalNumber { get; set; }
    public int? MedicalPlan { get; set; }
    public string OtherMedicalCare { get; set; }
}
