using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace GmWeb.Service.Logging
{
    public class ColorConsoleLoggerConfiguration
    {
        public int EventId { get; set; }

        public Dictionary<LogLevel, ConsoleColor> LogLevels { get; set; } = new()
        {
            [LogLevel.Information] = ConsoleColor.Green
        };
    }
}