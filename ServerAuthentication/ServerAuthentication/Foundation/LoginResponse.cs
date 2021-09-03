using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginAuthentication.Foundation
{
    public enum LoginResponse
    {
        Good = 0,
        Failed = 1,
        IPMatch_False = 2,
        HostNameMatch_False = 3,
        ServerError = 4,
        Unknown = 5
    }
}
