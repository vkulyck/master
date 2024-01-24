using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Deltas;

public class DeltaOptions
{
    /// <summary>
    /// Number of seconds to map a diff before giving up (0 for infinity).
    /// </summary>
    public float DiffTimeout { get; set; } = 1.0f;

    /// <summary>
    /// Cost of an empty edit operation in terms of edit characters.
    /// </summary>
    public short DiffEditCost { get; set; } = 4;

    /// <summary>
    /// At what point is no match declared (0.0 = perfection, 1.0 = very loose).
    /// </summary>
    public double MatchThreshold { get; set; } = 0.5D;

    /// <summary>
    /// How far to search for a match(0 = exact location, 1000+ = broad match).
    /// A match this many characters away from the expected location will ad
    /// 1.0 to the score(0.0 is a perfect match).
    /// </summary>
    public int MaxMatchDistance { get; set; } = 1000;

    /// <summary>
    /// When deleting a large block of text(over ~64 characters), how close
    /// do the contents have to be to match the expected contents. (0.0 =
    /// perfection, 1.0 = very loose).  Note that Match_Threshold controls
    /// how closely the end points of a delete need to match.
    /// </summary>
    public float PatchDeleteThreshold { get; set; } = 0.5f;

    /// <summary>
    /// Chunk size for context length. 
    /// </summary>
    public short PatchMargin { get; set; } = 4;

    /// <summary>
    /// The maximum bits allowed for a match.
    /// </summary>
    public short MaxMatchBits { get; set; } = 8 * sizeof(int);
}
