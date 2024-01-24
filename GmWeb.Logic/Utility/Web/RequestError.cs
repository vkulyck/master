using System;
using Newtonsoft.Json;

namespace GmWeb.Logic.Utility.Web;
public class RequestError
{
    [JsonProperty("message")]
    public string Message { get; set; }
    [JsonProperty("stack")]
    public string StackTrace { get; set; }
    public RequestError() { }
    public RequestError(string message)
    {
        this.Message = message;
    }
    public RequestError(Exception ex) : this(ex.Message)
    {
        this.StackTrace = GetFullTrace(ex);
    }

    public static string GetFullTrace(Exception ex, bool recursive = true)
    {
        string trace = "";

        trace += "Name: " + ex.GetType().Name + "\n";
        trace += "Message: " + ex.Message + "\n";
        trace += "Stack Trace: " + (ex.StackTrace ?? "null") + "\n";

        if (recursive)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;

                trace += "\n-------------------- Caused by: --------------------\n";
                trace += "Name: " + ex.GetType().Name + "\n";
                trace += "Message: " + ex.Message + "\n";
                trace += "Stack Trace: " + (ex.StackTrace ?? "null") + "\n";
            }
        }
        return trace;
    }
}