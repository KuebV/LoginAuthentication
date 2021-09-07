using JsonDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LoginAuthentication.Foundation;

namespace LoginAuthentication.ServerSide
{
    public class Server
    {
        /// <summary>
        /// Part of the AuthenticationServer Constructor, whether or not the LocalStorage is enabled
        /// </summary>
        public static bool m_LocalStorageEnabled;

        /// <summary>
        /// The MongoURL String
        /// </summary>
        public static string m_MongoURL;

        /// <summary>
        /// Settings for the ClientAuthenticaton
        /// </summary>
        private bool RequireIPMatch;

        /// <summary>
        /// Settings for the ClientAuthenticaton
        /// </summary>
        private bool RequireHostnameMatch;

        /// <summary>
        /// ServerSide.Server Constructor
        /// </summary>
        /// <param name="UseLocalStorage"></param>
        /// <param name="MongoURL"></param>
        /// <param name="settings"></param>
        public Server(bool UseLocalStorage = false, string MongoURL = null, LoginAuthenticationSettings settings = null)
        {
            m_LocalStorageEnabled = UseLocalStorage;
            m_MongoURL = MongoURL;

            if (settings == null)
            {
                Output.Message(OutputType.Error, "Settings not defined, loading defaults :\nHostNameMatch : True\nIPMatch : True");
                RequireHostnameMatch = true;
                RequireIPMatch = true;
            }

            RequireHostnameMatch = settings.HostNameMatch;
            RequireIPMatch = settings.IPMatch;
        }

        /// <summary>
        /// Dictionary containing the current ClientInformation for the connected Users
        /// </summary>
        public static Dictionary<int, string> ClientInformation = new Dictionary<int, string>();

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

                Item item = ServerSide.Database.item;

                var clientData = item.FindItem(ClientIdentity);
                Foundation.DataRetreive clientFoundation = new Foundation.DataRetreive(clientData.ToString());

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

            if (!string.IsNullOrEmpty(m_MongoURL))
            {
                string stringBuilder = string.Format("{0},{1}", IP, Hostname);
                string ServerHash = Server.ComputeSha256Hash(stringBuilder);

                DataRetreive dataRetreive = new DataRetreive(JsonData: null, _id: ClientIdentity);

                #region MongoDB Data
                string storedIP = dataRetreive.GetValueFromBson(ClientDatabaseEnum.IPAddress);
                string storedHost = dataRetreive.GetValueFromBson(ClientDatabaseEnum.Hostname);
                string storedAuth = dataRetreive.GetValueFromBson(ClientDatabaseEnum.AuthenticationString);
                #endregion

                #region Check Authentication String
                Output.Message(OutputType.Info, "Matching Hashed Authentication Strings");

                if (ServerHash != storedAuth)
                    return LoginResponse.ServerError;

                if (ServerHash != AuthString)
                    return LoginResponse.Failed;
                #endregion

            }

            return LoginResponse.Unknown;

        }

        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
