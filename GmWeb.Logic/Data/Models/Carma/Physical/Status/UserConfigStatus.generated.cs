


namespace GmWeb.Logic.Data.Models.Carma;


using System;
using System.ComponentModel.DataAnnotations.Schema;
using GmWeb.Logic.Data.Annotations;


[Flags]
public enum UserConfigStatus : byte
{
    None = 0,
    Starred = 1
}

public partial class UserConfig
{
    [SqlDataType(System.Data.SqlDbType.TinyInt)]
    public UserConfigStatus Status { get; set; } = UserConfigStatus.None;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public bool IsStarred
    {
        get => this.GetStatus(UserConfigStatus.Starred);
        set => this.SetStatus(UserConfigStatus.Starred, value);
    }

    private bool GetStatus(UserConfigStatus status) => this.Status.HasFlag(status);
    private void SetStatus(UserConfigStatus status, bool enable)
    {
        if (enable)
            this.Status |= status;
        else
            this.Status &= ~status;
    }
}
