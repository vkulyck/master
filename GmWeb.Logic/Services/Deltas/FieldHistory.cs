using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Deltas;

public record class HistoryEntry<TEntity>
{
    public DeltaField FieldHistory { get; set; }
    public string FieldValue { get; set; }
    public DateTimeOffset? FieldTimestamp { get; set; }
    public bool IsInserting => this.FieldHistory.IsUnmodified;
    public bool IsUpdating => this.FieldValue != this.FieldHistory.CurrentValue;
}

public abstract record class HistoryProperties
{
    public virtual PropertyInfo FieldDelta { get; }
    public virtual PropertyInfo FieldValue { get; }
    public virtual PropertyInfo FieldTimestamp { get; }

    public HistoryEntry<TEntity> CreateEntry<TEntity>(TEntity entity)
    {
        var entry = new HistoryEntry<TEntity>
        {
            FieldHistory = (DeltaField)this.FieldDelta.GetValue(entity),
            FieldValue = (string)this.FieldValue.GetValue(entity),
            FieldTimestamp = (DateTimeOffset?)this.FieldTimestamp.GetValue(entity)
        };
        return entry;
    }
}
public record class HistoryProperties<TEntity> : HistoryProperties
{
    public override PropertyInfo FieldDelta { get; }
    public override PropertyInfo FieldValue { get; }
    public override PropertyInfo FieldTimestamp { get; }
    public HistoryProperties(PropertyInfo deltaField)
    {
        this.FieldDelta = deltaField;
        this.FieldValue = typeof(TEntity).GetProperty(deltaField.Name.Replace("History", ""), typeof(string));
        this.FieldTimestamp = typeof(TEntity).GetProperty(deltaField.Name.Replace("History", "Modified"), typeof(DateTimeOffset?));
    }
}