using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GmWeb.Logic.Services.Deltas;

public class Delta
{
    protected List<Diff> Diffs { get; } = new List<Diff>();
    public Delta() { }
    public Delta(IEnumerable<Diff> diffs)
    {
        this.Diffs.AddRange(diffs);
    }

    /// <summary>
    /// Crush the diff into an encoded string which describes the operations
    /// required to transform text1 into text2.
    /// E.g. =3\t-2\t+ing  -> Keep 3 chars, delete 2 chars, insert 'ing'.
    /// Operations are tab-separated.  Inserted text is escaped using %xx
    /// notation.
    /// </summary>
    public string Serialize()
    {
        StringBuilder text = new StringBuilder();
        foreach (var aDiff in this.Diffs)
        {
            switch (aDiff.Operation)
            {
                case Operation.Insert:
                    text.Append("+").Append(DeltaService.encodeURI(aDiff.Text)).Append("\t");
                    break;
                case Operation.Delete:
                    text.Append("-").Append(aDiff.Text.Length).Append("\t");
                    break;
                case Operation.Equal:
                    text.Append("=").Append(aDiff.Text.Length).Append("\t");
                    break;
            }
        }
        string serialized = text.ToString();
        if (serialized.Length != 0)
        {
            // Strip off trailing tab character.
            serialized = serialized.Substring(0, serialized.Length - 1);
        }
        return serialized;
    }

    /// <summary>
    /// Given the original text1, and an encoded string which describes the
    /// operations required to transform text1 into text2, compute the full diff.
    /// @param text1 Source string for the diff.
    /// @param delta Delta text.
    /// @return Array of Diff objects or null if invalid.
    /// @throws ArgumentException If invalid input.
    /// </summary>
    public static Delta Deserialize(string text1, string serialized)
    {
        var diffs = new List<Diff>();
        int pointer = 0;  // Cursor in text1
        string[] tokens = serialized.Split(new string[] { "\t" },
            StringSplitOptions.None);
        foreach (string token in tokens)
        {
            if (token.Length == 0)
            {
                // Blank tokens are ok (from a trailing \t).
                continue;
            }
            // Each token begins with a one character parameter which specifies the
            // operation of this token (delete, insert, equality).
            string param = token.Substring(1);
            switch (token[0])
            {
                case '+':
                    // decode would change all "+" to " "
                    param = param.Replace("+", "%2b");

                    param = HttpUtility.UrlDecode(param);
                    //} catch (UnsupportedEncodingException e) {
                    //  // Not likely on modern system.
                    //  throw new Error("This system does not support UTF-8.", e);
                    //} catch (IllegalArgumentException e) {
                    //  // Malformed URI sequence.
                    //  throw new IllegalArgumentException(
                    //      "Illegal escape in diff_fromDelta: " + param, e);
                    //}
                    diffs.Add(new Diff(Operation.Insert, param));
                    break;
                case '-':
                // Fall through.
                case '=':
                    int n;
                    try
                    {
                        n = Convert.ToInt32(param);
                    }
                    catch (FormatException e)
                    {
                        throw new ArgumentException(
                            "Invalid number in diff_fromDelta: " + param, e);
                    }
                    if (n < 0)
                    {
                        throw new ArgumentException(
                            "Negative number in diff_fromDelta: " + param);
                    }
                    string text;
                    try
                    {
                        text = text1.Substring(pointer, n);
                        pointer += n;
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        throw new ArgumentException("Delta length (" + pointer
                            + ") larger than source text length (" + text1.Length
                            + ").", e);
                    }
                    if (token[0] == '=')
                    {
                        diffs.Add(new Diff(Operation.Equal, text));
                    }
                    else
                    {
                        diffs.Add(new Diff(Operation.Delete, text));
                    }
                    break;
                default:
                    // Anything else is an error.
                    throw new ArgumentException(
                        "Invalid diff operation in diff_fromDelta: " + token[0]);
            }
        }
        if (pointer != text1.Length)
        {
            throw new ArgumentException("Delta length (" + pointer
                + ") smaller than source text length (" + text1.Length + ").");
        }
        return new Delta(diffs);
    }
}
