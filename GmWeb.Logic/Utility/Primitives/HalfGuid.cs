using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Extensions.Collections;

namespace GmWeb.Logic.Utility.Primitives;

public record struct HalfGuid
{
    #region Format Configuration
    private static readonly string Pattern_HexDigit = @"[a-f0-9]";
    private static readonly string Pattern_A = $@"(?<A>{Pattern_HexDigit}{{8}})";
    private static readonly string Pattern_B = $@"(?<B>{Pattern_HexDigit}{{4}})";
    private static readonly string Pattern_C = $@"(?<C>{Pattern_HexDigit}{{4}})";
    private static readonly string Pattern_Hyphen = $@"{Pattern_A}\-{Pattern_B}\-{Pattern_C}";
    private static readonly string Pattern_Braced = $@"\{{{Pattern_Hyphen}\}}";
    private static readonly string Pattern_Parens = $@"\({Pattern_Hyphen}\)";
    private static readonly string Pattern_HexArray = $@"\{{0x{Pattern_A},0x{Pattern_B},0x{Pattern_C}\}}";

    private static readonly List<string> ValidPatterns = new List<string>
    {
        Pattern_Hyphen,
        Pattern_Braced,
        Pattern_Parens,
        Pattern_HexArray
    };

    public const int DataLength = 8;
    public const int HexCharacterCount = DataLength * 2;
    public const int HyphenCount = 2;
    public const int BraceCount = 2;
    public const int BracedStringLength = HexCharacterCount + HyphenCount + BraceCount;

    #endregion

    int A { get; }
    short B { get; }
    short C { get; }
    public long ID => (long)this.A << sizeof(int) | (long)this.B << sizeof(short) | (long)this.C;
    public ulong UID => (ulong)this.A << sizeof(int) | (ulong)this.B << sizeof(short) | (ulong)this.C << 0;

    public HalfGuid((int a, short b, short c) tuple)
    {
        (this.A, this.B, this.C) = tuple;
    }
    public HalfGuid(byte[] bytes) : this(ConvertBytes(bytes)) { }
    public HalfGuid(long l) : this(BitConverter.GetBytes(l)) { }
    public HalfGuid(ulong uo) : this(BitConverter.GetBytes(uo)) { }
    public HalfGuid(HalfGuid hg) : this((hg.A, hg.B, hg.C)) { }
    public HalfGuid(Guid guid) : this(guid.ToByteArray()) { }
    public HalfGuid(string s) : this(ParseString(s)) { }

    public static HalfGuid NewHalfGuid()
    {
        var g = Guid.NewGuid();
        var hg = new HalfGuid(g);
        return hg;
    }

    #region Parsing
    private static int ParseInt(string s)
    {
        if (int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result))
            return result;
        throw new Exception($"Attempted to parse an integer value from a non-numeric string: {s}");
    }
    private static short ParseShort(string s)
    {
        if (short.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out short result))
            return result;
        throw new Exception($"Attempted to parse an integer value from a non-numeric string: {s}");
    }
    private static (int a, short b, short c) ParseString(string s)
    {
        if (s.Length > BracedStringLength)
            return ParseGuidString(s);
        else
            return ParseHalfGuidString(s);
    }
    private static (int a, short b, short c) ParseHalfGuidString(string s)
    {
        Match match = null;
        foreach (var pattern in ValidPatterns)
        {
            match = Regex.Match(s, $"^{pattern}$");
            if (match.Success)
                break;
        }
        if (!match.Success)
            throw BadFormat();

        var a = ParseInt(match.Groups["A"].Value);
        var b = ParseShort(match.Groups["B"].Value);
        var c = ParseShort(match.Groups["C"].Value);
        return (a, b, c);
    }
    private static (int a, short b, short c) ParseGuidString(string s)
    {
        var guid = new Guid(s);
        var bytes = guid.ToByteArray();
        return ConvertBytes(bytes);
    }
    private static (int a, short b, short c) ConvertBytes(byte[] bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));
        if (bytes.Length == 0)
            throw new ArgumentOutOfRangeException(nameof(bytes));
        if (bytes.Length % DataLength != 0)
            throw new ArgumentOutOfRangeException(nameof(bytes));
        IEnumerable<byte> data;
        if (bytes.Length > DataLength)
        {
            var ratio = bytes.Length / DataLength;
            data = bytes.XorSelf(ratio);
        }
        else
            data = bytes;
        var a = BitConverter.ToInt32((data = data.Take(4)).ToArray());
        var b = BitConverter.ToInt16((data = data.Take(2)).ToArray());
        var c = BitConverter.ToInt16((data = data.Take(2)).ToArray());
        return (a, b, c);
    }

    #endregion

    #region Formatting and Serialization
    private static Exception BadFormat() => new FormatException("Invalid input format detected in HalfGuid constructor.");
    public byte[] ToByteArray()
    {
        byte[] g = new byte[DataLength];

        g[0] = (byte)(A);
        g[1] = (byte)(A >> 8);
        g[2] = (byte)(A >> 16);
        g[3] = (byte)(A >> 24);
        g[4] = (byte)(B);
        g[5] = (byte)(B >> 8);
        g[6] = (byte)(C);
        g[7] = (byte)(C >> 8);

        return g;
    }
    public override String ToString()
    {
        return ToString("D");
    }
    private static string GetBytes(int value)
        => BitConverter.ToString(BitConverter.GetBytes(value)).ToLowerInvariant().Replace("-", "");
    private static string GetBytes(short value)
        => BitConverter.ToString(BitConverter.GetBytes(value)).ToLowerInvariant().Replace("-", "");

    public String ToString(string format)
    {
        format = (format ?? "D").ToUpper();

        switch (format)
        {
            default:
            case "D": // 00000000-0000-0000-0000-000000000000
                return $"{GetBytes(A)}-{GetBytes(B)}-{GetBytes(C)}";
            case "N": // 00000000000000000000000000000000
                return $"{GetBytes(A)}{GetBytes(B)}{GetBytes(C)}";
            case "B": // {00000000-0000-0000}
                return $"{{{GetBytes(A)}-{GetBytes(B)}-{GetBytes(C)}}}";
            case "P": // (00000000-0000-0000)
                return $"({GetBytes(A)}-{GetBytes(B)}-{GetBytes(C)})";
            case "X": // {0x00000000,0x0000,0x0000}
                return $"{{0x{GetBytes(A)},0x{GetBytes(B)},0x{GetBytes(C)}}}";
        }
    }

    #endregion
}
