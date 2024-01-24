namespace GmWeb.Web.Common.Auth
{
    public class JwtLogoutResult
    {
        public static JwtLogoutResult Failed => _Failed;
        private static readonly JwtLogoutResult _Failed = new JwtLogoutResult { Succeeded = false };
        public static JwtLogoutResult Success => _Success;
        private static readonly JwtLogoutResult _Success = new JwtLogoutResult { Succeeded = true };

        public bool Succeeded { get; set; }

        public JwtLogoutResult() { }
    }
}
