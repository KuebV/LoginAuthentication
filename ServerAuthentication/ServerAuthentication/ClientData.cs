using System;
using JsonDB;
using LoginAuthentication.ServerSide;

namespace LoginAuthentication
{
    public class ClientData
    {
        /// <summary>
        /// Private Integer for the ClientID for incoming client login
        /// </summary>
        private int ClientID;

        public string ClientIdentity;

        /// <summary>
        /// Constructor for the Client Data Class
        /// </summary>
        /// <param name="clientId"></param>
        public ClientData(int clientId, LoginAuthenticationSettings settings)
        {
            ClientID = clientId;
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

            ServerSide.Database.CreateNewItem(UsernameHash, builder);
        }
    }

    public class LoginAuthenticationSettings
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