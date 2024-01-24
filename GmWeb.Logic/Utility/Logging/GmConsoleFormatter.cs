using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace GmWeb.Logic.Utility.Logging;
public class GmConsoleFormatter : ConsoleFormatter, IDisposable
{
    private const string LoglevelPadding = ": ";
    private IDisposable _optionsReloadToken;

    protected GmConsoleFormatterOptions FormatterOptions { get; set; }

    public GmConsoleFormatter(IOptionsMonitor<GmConsoleFormatterOptions> options)
        : base("GM")
    {
        this.FormatterOptions = options.CurrentValue;
        _optionsReloadToken = options.OnChange(opts => FormatterOptions = opts);
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }
        LogLevel logLevel = logEntry.LogLevel;
        ConsoleColors logLevelColors = GetLogLevelConsoleColors(logLevel);
        string logLevelString = GetLogLevelString(logLevel);

        string timestamp = null;
        string timestampFormat = FormatterOptions.TimestampFormat;
        if (timestampFormat != null)
        {
            DateTimeOffset dateTimeOffset = GetCurrentDateTime();
            timestamp = dateTimeOffset.ToString(timestampFormat);
        }
        if (timestamp != null)
        {
            textWriter.Write(timestamp);
        }
        if (logLevelString != null)
        {
            textWriter.WriteColoredMessage(logLevelString, logLevelColors.Background, logLevelColors.Foreground);
        }
        CreateDefaultLogMessage(textWriter, logEntry, message, scopeProvider);
    }

    private void CreateDefaultLogMessage<TState>(TextWriter textWriter, in LogEntry<TState> logEntry, string message, IExternalScopeProvider scopeProvider)
    {
        bool category = FormatterOptions.IncludeCategories;
        bool singleLine = FormatterOptions.SingleLine;
        int eventId = logEntry.EventId.Id;
        Exception exception = logEntry.Exception;

        // Example:
        // info: ConsoleApp.Program[10]
        //       Request received

        // category and event id
        textWriter.Write(LoglevelPadding);
        if (this.FormatterOptions.IncludeCategories)
        {
            textWriter.Write(logEntry.Category);
        }

        if (this.FormatterOptions.IncludeEvents)
        {
            textWriter.Write('[');
#if NETCOREAPP
            Span<char> span = stackalloc char[10];
            if (eventId.TryFormat(span, out int charsWritten))
                textWriter.Write(span.Slice(0, charsWritten));
            else
#endif
            textWriter.Write(eventId.ToString());
            textWriter.Write(']');
        }
        if (!singleLine)
        {
            textWriter.Write(Environment.NewLine);
        }

        // scope information
        WriteScopeInformation(textWriter, scopeProvider, singleLine, logEntry.LogLevel);
        WriteMessage(textWriter, message, singleLine, logEntry.LogLevel);

        // Example:
        // System.InvalidOperationException
        //    at Namespace.Class.Function() in File:line X
        if (exception != null)
        {
            // exception message
            WriteMessage(textWriter, exception.ToString(), singleLine, logEntry.LogLevel);
        }
        if (singleLine)
        {
            textWriter.Write(Environment.NewLine);
        }
    }

    private void WriteMessage(TextWriter textWriter, string message, bool singleLine, LogLevel logLevel)
    {
        if (!string.IsNullOrEmpty(message))
        {
            if (singleLine)
            {
                textWriter.Write(' ');
                WriteReplacing(textWriter, Environment.NewLine, " ", message);
            }
            else
            {
                textWriter.Write(MessagePadding(logLevel));
                WriteReplacing(textWriter, Environment.NewLine, NewLineMessagePadding(logLevel), message);
                textWriter.Write(Environment.NewLine);
            }
        }

        static void WriteReplacing(TextWriter writer, string oldValue, string newValue, string message)
        {
            string newMessage = message.Replace(oldValue, newValue);
            writer.Write(newMessage);
        }
    }

    private DateTimeOffset GetCurrentDateTime()
    {
        return FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
    }

    private string GetLogLevelString(LogLevel logLevel)
        => this.FormatterOptions.LevelStrings.TryGetValue(logLevel, out string levelString) ? levelString : default;

    private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
    {
        bool disableColors = (FormatterOptions.ColorBehavior == LoggerColorBehavior.Disabled) ||
            (FormatterOptions.ColorBehavior == LoggerColorBehavior.Default && System.Console.IsOutputRedirected);
        if (disableColors)
            return ConsoleColors.None;

        if (this.FormatterOptions.Colors.TryGetValue(logLevel, out var colors))
            return colors;
        return ConsoleColors.None;
    }

    private string NewLineMessagePadding(LogLevel logLevel)
        => Environment.NewLine + MessagePadding(logLevel);
    private string MessagePadding(LogLevel logLevel)
        => new string(' ', GetLogLevelString(logLevel).Length + LoglevelPadding.Length);

    private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider scopeProvider, bool singleLine, LogLevel logLevel)
    {
        if (FormatterOptions.IncludeScopes && scopeProvider != null)
        {
            bool paddingNeeded = !singleLine;
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (paddingNeeded)
                {
                    paddingNeeded = false;
                    state.Write(MessagePadding(logLevel));
                    state.Write("=> ");
                }
                else
                {
                    state.Write(" => ");
                }
                state.Write(scope);
            }, textWriter);

            if (!paddingNeeded && !singleLine)
            {
                textWriter.Write(Environment.NewLine);
            }
        }
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}