namespace GmWeb.Web.Common.Auth
{
    public class JwtAccessResult
    {
        public static JwtAccessResult Revoked => new JwtAccessResult { IsRevoked = true };
        public static JwtAccessResult Valid => new JwtAccessResult { IsValid = true };

        public bool IsRevoked { get; set; } = false;
        public bool IsValid { get; set; } = false;

        private JwtAccessResult() { }
    }
}
