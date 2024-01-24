using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Deltas;

/// <summary>
/// Class representing one patch operation.
/// </summary>
public record class Patch
{
    public List<Diff> Diffs { get; set; } = new List<Diff>();
    public int BeforeIndex { get; set; }
    public int AfterIndex { get; set; }
    public int BeforeLength { get; set; }
    public int AfterLength { get; set; }

    /// <summary>
    /// Emulate GNU diff's format.
    /// Header: @@ -382,8 +481,9 @@
    /// Indices are printed as 1-based, not 0-based.
    /// @return The GNU diff string.
    /// </summary>
    public override string ToString()
    {
        string coordsBefore, coordsAfter;
        if (this.BeforeLength == 0)
        {
            coordsBefore = this.BeforeIndex + ",0";
        }
        else if (this.BeforeLength == 1)
        {
            coordsBefore = Convert.ToString(this.BeforeIndex + 1);
        }
        else
        {
            coordsBefore = (this.BeforeIndex + 1) + "," + this.BeforeLength;
        }
        if (this.AfterLength == 0)
        {
            coordsAfter = this.AfterIndex + ",0";
        }
        else if (this.AfterLength == 1)
        {
            coordsAfter = Convert.ToString(this.AfterIndex + 1);
        }
        else
        {
            coordsAfter = (this.AfterIndex + 1) + "," + this.AfterLength;
        }
        StringBuilder text = new StringBuilder();
        text.Append("@@ -").Append(coordsBefore).Append(" +").Append(coordsAfter)
            .Append(" @@\n");
        // Escape the body of the patch with %xx notation.
        foreach (Diff aDiff in this.Diffs)
        {
            switch (aDiff.Operation)
            {
                case Operation.Insert:
                    text.Append('+');
                    break;
                case Operation.Delete:
                    text.Append('-');
                    break;
                case Operation.Equal:
                    text.Append(' ');
                    break;
            }

            text.Append(DeltaService.encodeURI(aDiff.Text)).Append("\n");
        }
        return text.ToString();
    }
}

