using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonDB;
using LoginAuthentication;
using Newtonsoft.Json.Linq;

namespace ServerAuthentication
{
    public class ClientData
    {
        /// <summary>
        /// Private Integer for the ClientID for incoming client login
        /// </summary>
        private int ClientID;

        /// <summary>
        /// Settings for the ClientAuthenticaton
        /// </summary>
        private bool RequireIPMatch;

        /// <summary>
        /// Settings for the ClientAuthenticaton
        /// </summary>
        private bool RequireHostnameMatch;

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
        public LoginResponse HandleLoginData(string IP, string Hostname, string AuthString, string ClientIdentity)
        {
            // If the Server is running off of the Local Storage Option
            if (Server.m_LocalStorageEnabled)
            {
                // Create the Server-Side Hash
                string stringBuilder = string.Format("{0},{1}", IP, Hostname);
                string ServerHash = Server.ComputeSha256Hash(stringBuilder);

                Item item = Database.item;

                var clientData = item.FindItem(ClientIdentity);
                ClientFoundation clientFoundation = new ClientFoundation(clientData.ToString());

                #region Locally Stored Client Database
                string localIP = clientFoundation.GetValueFromJson(ClientDatabaseEnum.IPAddress);
                string localHostName = clientFoundation.GetValueFromJson(ClientDatabaseEnum.Hostname);
                string localAuthenticationString = clientFoundation.GetValueFromJson(ClientDatabaseEnum.AuthenticationString);
                #endregion

                #region Check Authentication String
                if (ServerHash != localAuthenticationString)
                    return LoginResponse.ServerError;

                if (ServerHash != AuthString)
                    return LoginResponse.Failed;
                #endregion

                #region 
                if (RequireIPMatch && IP == localIP && RequireHostnameMatch && Hostname == localHostName)
                    return LoginResponse.Good;

                if (RequireHostnameMatch && Hostname != localHostName)
                    return LoginResponse.HostNameMatch_False;

                if (RequireIPMatch && IP != localIP)
                    return LoginResponse.IPMatch_False;

                if (!RequireIPMatch && !RequireHostnameMatch)
                    return LoginResponse.Good;
                #endregion

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
        ServerError = 4,
        Unknown = 5
    }
}
