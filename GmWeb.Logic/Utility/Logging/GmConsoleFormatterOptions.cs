using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace GmWeb.Logic.Utility.Logging;
public class GmConsoleFormatterOptions : SimpleConsoleFormatterOptions
{
    public bool IncludeEvents { get; set; } = true;
    public bool IncludeCategories { get; set; } = true;
    public Dictionary<LogLevel, ConsoleColors> Colors { get; }
    public Dictionary<LogLevel, string> LevelStrings { get; }
    public GmConsoleFormatterOptions()
    {
        this.Colors = new()
        {
            [LogLevel.None] = ConsoleColor.DarkGray,
            [LogLevel.Trace] = ConsoleColor.White,
            [LogLevel.Debug] = ConsoleColor.Blue,
            [LogLevel.Information] = (ConsoleColor.Green, ConsoleColor.Blue),
            [LogLevel.Warning] = ConsoleColor.Yellow,
            [LogLevel.Error] = ConsoleColor.Magenta,
            [LogLevel.Critical] = ConsoleColor.Red
        };

        this.LevelStrings = new()
        {
            [LogLevel.Trace] = "trce",
            [LogLevel.Debug] = "dbug",
            [LogLevel.Information] = "info",
            [LogLevel.Warning] = "warn",
            [LogLevel.Error] = "fail",
            [LogLevel.Critical] = "crit"
        };
    }
}