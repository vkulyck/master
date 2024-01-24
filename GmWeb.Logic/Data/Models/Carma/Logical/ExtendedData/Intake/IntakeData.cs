using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;

public record class IntakeData
{
    private static readonly IntakeData _Empty = new IntakeData
    {
        General = new(),
        Ethnicity = new(),
        Nationality = new(),
        Religion = new(),
        Orientation = new(),
        Marital = new(),
        Family = new(),
        Career = new(),
        Insurance = new()
    };
    public static IntakeData Empty => _Empty with { }; // Clone the record
    public GeneralData General { get; set; }
    public EthnicityData Ethnicity { get; set; }
    public NationalityData Nationality { get; set; }
    public ReligionData Religion { get; set; }
    public OrientationData Orientation { get; set; }
    public MaritalData Marital { get; set; }
    public FamilyData Family { get; set; }
    public CareerData Career { get; set; }
    public InsuranceData Insurance { get; set; }
}
