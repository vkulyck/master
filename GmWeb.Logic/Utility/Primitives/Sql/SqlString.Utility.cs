using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Primitives;

public partial record struct SqlString
{

    /// <summary>
    /// Wrap the current command in a function call.
    /// </summary>
    /// <param name="name">The function name.</param>
    /// <param name="args">Optional arguments to be passed to the function after the current instance expression.</param>
    /// <returns>A SQL consisting of a call to the supplied function, having the current instance SQL as the first 
    /// argument and the provided <paramref name="args"/> list as the subsequent function arguments.</returns>
    private SqlString Wrap(string name, IEnumerable<string> args)
        => this.Wrap(name, args.ToArray());
    private SqlString Wrap(string name, params string[] args)
    {
        var func = name.ToUpper();
        if (args.Length > 0)
        {
            var argList = string.Join(", ", args);
            var result = $"{func}({this.SQL}, {argList})";
            return result;
        }
        else
        {
            var result = $"{func}({this.SQL})";
            return result;
        }
    }


    /// <summary>
    /// Surround the provided argument with single quotes to create a SQL (N)VARCHAR string.
    /// </summary>
    /// <param name="str">The (N)VARCHAR value to be quoted.</param>
    /// <returns>The quoted (N)VARCHAR string.</returns>
    private static string Q(string str) => $"'{str}'";
}
