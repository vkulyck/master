using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.QRCode;

public class TotpConfig
{
    public string Email { get; private set; }
    public string Key { get; private set; }
    public string Label { get; private set; }

    #region Immutable Properties
    public static readonly string DefaultLabel = "Goodmojo";
    // Google Authenticator only supports 6 digits
    public const int GoogleAuthenticatorDigits = 6;
    protected string EncodedEmail => this.Encoder.Encode(this.Email);
    protected string EncodedLabel => this.Encoder.Encode(this.Label);
    public string SharedKey => FormatKey(this.Key);
    public string AuthenticatorUri
    {
        get => $"otpauth://totp/{this.EncodedLabel}:{this.EncodedEmail}?secret={this.Key}&issuer={this.EncodedLabel}&digits={GoogleAuthenticatorDigits}";
    }
    protected UrlEncoder Encoder => UrlEncoder.Default;
    #endregion

    public TotpConfig(string RawKey, string Email, string Label = null)
    {
        this.Key = RawKey;
        this.Email = Email;
        this.Label = Label ?? DefaultLabel;
    }

    protected static string FormatKey(string key)
    {
        var pieces = key.Chunk(4).Select(x => string.Join("", x));
        var formatted = string.Join(" ", pieces).ToLowerInvariant();
        return formatted;
    }
}