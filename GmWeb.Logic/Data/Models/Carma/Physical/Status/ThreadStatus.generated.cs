


namespace GmWeb.Logic.Data.Models.Carma;


using System;
using System.ComponentModel.DataAnnotations.Schema;
using GmWeb.Logic.Data.Annotations;


[Flags]
public enum ThreadStatus : byte
{
    None = 0,
    Flagged = 1
}

public partial class Thread
{
    [SqlDataType(System.Data.SqlDbType.TinyInt)]
    public ThreadStatus Status { get; set; } = ThreadStatus.None;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public bool IsFlagged
    {
        get => this.GetStatus(ThreadStatus.Flagged);
        set => this.SetStatus(ThreadStatus.Flagged, value);
    }

    private bool GetStatus(ThreadStatus status) => this.Status.HasFlag(status);
    private void SetStatus(ThreadStatus status, bool enable)
    {
        if (enable)
            this.Status |= status;
        else
            this.Status &= ~status;
    }
}
