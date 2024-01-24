using GmWeb.Logic.Data.Annotations;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GenderType = GmWeb.Logic.Enums.Gender;
using PrimaryLanguages = GmWeb.Logic.Services.Datasets.Languages.PrimaryLanguages;
using UserRole = GmWeb.Logic.Enums.UserRole;
using Constants = GmWeb.Common.Identity.IdentityConstants;
using IPerson = GmWeb.Logic.Interfaces.IPerson;
using UserProfile = GmWeb.Logic.Data.Models.Carma.ExtendedData.UserProfile;
using CarmaContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;
using Newtonsoft.Json;
using Nito.AsyncEx;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Models.Carma;

[Table("Users", Schema = "carma")]
public partial class User : BaseDataModel, IPerson, IIntegerKeyModel
{
    #region Keys
    int IIntegerKeyModel.PrimaryKey => this.UserID;
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserID { get; set; }
    [ForeignKey("Agency")]
    public int AgencyID { get; set; } = Constants.DefaultAgencyID;
    [ForeignKey("AgencyID")]
    public virtual Agency Agency { get; set; }
    public Guid? AccountID { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid LookupID { get; set; } = Guid.NewGuid();
    public string Email { get; set; }
    #endregion

    #region Names
    public string Title { get; set; }
    public string FirstName { get; set; }
    [NotMapped]
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    [NotMapped]
    public string Suffix { get; set; }
    public string FullName => (this as IPerson).GetFullName();
    #endregion

    #region Details
    public string Phone { get; set; }
    [SqlDataType(System.Data.SqlDbType.Int)]
    public UserRole UserRole { get; set; } = UserRole.Client;
    [SqlDataType(System.Data.SqlDbType.Int)]
    public GenderType Gender { get; set; } = GenderType.Unspecified;
    public string LanguageCode { get; set; } = PrimaryLanguages.English.Value;
    [JsonColumn]
    public virtual UserProfile Profile { get; set; } = new UserProfile();

    [SqlDataType(System.Data.SqlDbType.Date)]
    public System.DateTime BirthDate { get; set; }
    #endregion

    #region Navigation
    [InverseProperty("Registrant")]
    public virtual ICollection<UserActivity> RegisteredActivities { get; set; }
    [InverseProperty("Registrar")]
    public virtual ICollection<UserActivity> OrganizedActivities { get; set; }
    [InverseProperty("NoteAuthor")]
    public virtual ICollection<Note> ChildNotes { get; set; }
    [InverseProperty("NoteSubject")]
    public virtual ICollection<Note> ParentNotes { get; set; }
    [InverseProperty("ConfigSubject")]
    public virtual ICollection<UserConfig> ParentConfigs { get; set; }
    [InverseProperty("ConfigOwner")]
    public virtual ICollection<UserConfig> ChildConfigs { get; set; }
    #endregion

    #region Viewer
    public void LoadParentConfig(User owner)
    {
        AsyncContext.Run(async () => await this.LoadParentConfigAsync(owner, context: null));
    }
    public async Task LoadParentConfigAsync(User owner, CarmaContext context)
    {
        this.ViewerID = owner.UserID;
        if (!this.ViewerID.HasValue)
            return;
        UserConfig parentConfig = null;
        if (this.ParentConfigs == null && context != null)
        {
            var childEntry = context.Entry(this);
            var configsEntry = childEntry.Collection(x => x.ParentConfigs);
            await configsEntry.LoadAsync();
            parentConfig = configsEntry.CurrentValue.SingleOrDefault(x => x.OwnerID == owner.UserID);
        }
        else if(this.ParentConfigs != null)
            parentConfig = this.ParentConfigs.SingleOrDefault(x => x.OwnerID == this.ViewerID.Value);
        if (parentConfig == null)
        {
            this.IsStarred = false;
            return;
        }
        this.IsStarred = parentConfig?.IsStarred ?? false;
    }
    [NotMapped]
    [JsonIgnore]
    public int? ViewerID { get; set; }
    [NotMapped]
    public bool? IsStarred { get; set; }
    #endregion

    public override string ToString()
    {
        string s = $"{this.UserID}: {this.LastName}, {this.FirstName}";
        if (this.IsStarred == true)
            return $"★ {s}";
        return s;
    }

    public void GenerateLookupID(int seed)
    {
        var random = new Random(seed);
        byte[] bytes = new byte[16];
        random.NextBytes(bytes);
        this.LookupID = new Guid(bytes);
    }
}
