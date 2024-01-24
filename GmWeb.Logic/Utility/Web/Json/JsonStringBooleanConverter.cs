using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Web;

public class JsonStringBooleanConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            // try to parse number directly from bytes
            ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
            if (Utf8Parser.TryParse(span, out bool boolean, out int bytesConsumed) && span.Length == bytesConsumed)
                return boolean;

            // try to parse from a string if the above failed, this covers cases with other escaped/UTF characters
            if (Boolean.TryParse(reader.GetString(), out boolean))
                return boolean;
        }

        // fallback to default handling
        return reader.GetBoolean();
    }

    public override void Write(Utf8JsonWriter writer, bool boolean, JsonSerializerOptions options)
    {
        writer.WriteStringValue(boolean.ToString());
    }
}