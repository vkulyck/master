using System;
using System.Collections.Generic;
using System.Text;

namespace Yubico
{
    public enum YubicoAuthenticationResponse
    {
        AuthenticationSuccess,
        AuthenticationFailure,
        OtpError,
        ServerError
    }
}
