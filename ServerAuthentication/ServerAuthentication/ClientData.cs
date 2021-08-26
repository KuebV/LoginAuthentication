using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonDB;

namespace ServerAuthentication
{
    public class ClientData
    {
        private int ClientID;
        private bool RequireIPMatch;
        private bool RequireHostnameMatch;

        private Server _server;

        /// <summary>
        /// Constructor for the Client Data Class
        /// </summary>
        /// <param name="clientId"></param>
        public ClientData(int clientId, ClientAuthenticationSettings settings)
        {
            ClientID = clientId;

            RequireHostnameMatch = settings.HostNameMatch;
            RequireIPMatch = settings.IPMatch;
        }

        /// <summary>
        /// Handle the Login Data, returns the Login Response
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="Hostname"></param>
        /// <param name="AuthString"></param>
        /// <returns></returns>
        public LoginResponse HandleLogin(string IP, string Hostname, string AuthString)
        {
            // If the Server is running off of the Local Storage Option
            if (_server.m_LocalStorageEnabled)
            {
                // Create the Server-Side Hash
                string stringBuilder = string.Format("{0},{1}", IP, Hostname);
                string ServerHash = Server.ComputeSha256Hash(stringBuilder);
    
            }

            return LoginResponse.Unknown;

        }

        public void CreateUser(string IP, string Hostname)
        {
            string stringBuilder = string.Format("{0},{1}", IP, Hostname);
            string UserHash = Server.ComputeSha256Hash(stringBuilder);

            string UsernameHash = UserHash.Substring(0, UserHash.Length / 4);

            var builder = new ClientDatabase
            {
                _id = UsernameHash,
                AuthenticationString = UserHash,
                Hostname = Hostname,
                IPAddress = IP
            };

            Database.CreateNewItem(UsernameHash, builder);
        }
    }

    public class ClientAuthenticationSettings
    {
        public bool IPMatch { get; set; }
        public bool HostNameMatch { get; set; }
    }

    public enum LoginResponse
    {
        Good = 0,
        Failed = 1,
        IPMatch_False = 2,
        HostNameMatch_False = 3,
        Unknown = 4
    }
}
