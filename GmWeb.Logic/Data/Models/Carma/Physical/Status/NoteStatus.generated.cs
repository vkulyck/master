


namespace GmWeb.Logic.Data.Models.Carma;


using System;
using System.ComponentModel.DataAnnotations.Schema;
using GmWeb.Logic.Data.Annotations;


[Flags]
public enum NoteStatus : byte
{
    None = 0,
    Flagged = 1
}

public partial class Note
{
    [SqlDataType(System.Data.SqlDbType.TinyInt)]
    public NoteStatus Status { get; set; } = NoteStatus.None;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public bool IsFlagged
    {
        get => this.GetStatus(NoteStatus.Flagged);
        set => this.SetStatus(NoteStatus.Flagged, value);
    }

    private bool GetStatus(NoteStatus status) => this.Status.HasFlag(status);
    private void SetStatus(NoteStatus status, bool enable)
    {
        if (enable)
            this.Status |= status;
        else
            this.Status &= ~status;
    }
}
