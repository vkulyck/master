using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;

namespace GmWeb.Logic.Utility.Logging;

public record struct ConsoleColors
{
    private static ConsoleColors _None = new ConsoleColors();
    public static ConsoleColors None => _None;
    public ConsoleColor? Foreground { get; set; }
    public ConsoleColor? Background { get; set; }

    public ConsoleColors() : this(default, default) { }
    public ConsoleColors(ConsoleColor? foreground) : this(foreground, default) { }
    public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
    {
        Foreground = foreground;
        Background = background;
    }

    public static implicit operator ConsoleColors((ConsoleColor Foreground, ConsoleColor Background) colors)
    {
        return new ConsoleColors(colors.Foreground, colors.Background);
    }
    public static implicit operator ConsoleColors(ConsoleColor color)
    {
        return new ConsoleColors(foreground: color);
    }
}