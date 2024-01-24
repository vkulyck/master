using Newtonsoft.Json;

namespace GmWeb.Logic.Utility.Redis
{
    public class UserRegistrationData
    {
        [JsonIgnore]
        public string Token { get; set; }
        public string Email { get; set; }

        public override string ToString() => $"RegData: {this.Email}";
    }
}
