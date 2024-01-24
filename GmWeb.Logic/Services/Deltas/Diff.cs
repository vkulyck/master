using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Deltas;

/// <summary>
/// Class representing one diff operation.
/// </summary>
public record class Diff
{
    /// <summary>
    /// The text transform operation being performed.
    /// </summary>
    public Operation Operation { get; set; }
    /// <summary>
    /// The text associated with this diff operation.
    /// </summary>
    public string Text { get; set; }

    public Diff() : this(Operation.None, string.Empty) { }
    public Diff(Operation operation, string text)
    {
        // Construct a diff with the specified operation and text.
        this.Operation = operation;
        this.Text = text;
    }

    /// <summary>
    /// Display a human-readable version of this Diff.
    /// @return text version.
    /// </summary>
    public override string ToString()
    {
        string prettyText = this.Text.Replace('\n', '\u00b6');
        return "Diff(" + this.Operation + ",\"" + prettyText + "\")";
    }
}
