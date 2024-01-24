using System;

namespace GmWeb.Logic.Utility.Redis
{
    public interface IAccountToken
    {
        Guid AccountID { get; }
        string TokenID { get; }
        TimeSpan Lifetime { get; }
        string TokenType { get; }
    }
}
