using System;
using JsonDB;

namespace LoginAuthentication
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

        public string ClientIdentity;

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

        private string GetUnixTime() => DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

        /// <summary>
        /// Handles the Client Disconnect
        /// Removes One-Time-Code & Cleans Up anything else
        /// </summary>
        public void HandleClientDisconnect() => Server.ClientInformation.Remove(ClientID);

        /// <summary>
        /// Handle Client Data 
        /// Returns the One-Time-Code
        /// </summary>
        /// <returns></returns>
        public void GenerateOneTimeCode()
        {
            string hashedTime = Server.ComputeSha256Hash(GetUnixTime());
            string onetimecode = hashedTime.Substring(0, hashedTime.Length / 6);
            Server.ClientInformation.Add(ClientID, onetimecode);
        }

        public string GetOneTimeCode()
        {
            string code;
            if (Server.ClientInformation.TryGetValue(ClientID, out code))
                return code;

            return null;
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
                Foundation clientFoundation = new Foundation(clientData.ToString());

                #region Locally Stored Client Database
                string localIP = clientFoundation.GetValueFromJson(ClientDatabaseEnum.IPAddress);
                string localHostName = clientFoundation.GetValueFromJson(ClientDatabaseEnum.Hostname);
                string localAuthenticationString = clientFoundation.GetValueFromJson(ClientDatabaseEnum.AuthenticationString);
                #endregion

                #region Check Authentication String
                Output.Message(OutputType.Info, "Matching Hashed Authentication Strings");

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

            ClientIdentity = UserHash;

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