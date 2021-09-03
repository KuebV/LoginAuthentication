using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginAuthentication.Foundation
{
    public class ClientDatabase
    {
        public string _id { get; set; }
        public string Hostname { get; set; }
        public string IPAddress { get; set; }
        public string AuthenticationString { get; set; }
    }
}
