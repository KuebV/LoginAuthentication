using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServerAuthentication
{
    public class Server
    {
        /// <summary>
        /// Part of the AuthenticationServer Constructor, whether or not the LocalStorage is enabled
        /// </summary>
        public readonly bool m_LocalStorageEnabled;

        /// <summary>
        /// The MongoURL String
        /// </summary>
        public readonly string m_MongoURL;


        /// <summary>
        /// AuthenticationServer constructor
        /// </summary>
        /// <param name="port"></param>
        /// <param name="limitClientConnections"></param>
        /// <param name="maxConnections"></param>
        public Server(bool UseLocalStorage = false, string MongoURL = null)
        {
            m_LocalStorageEnabled = UseLocalStorage;
            m_MongoURL = MongoURL;
        }

        /// <summary>
        /// Dictionary containing the current ClientInformation for the connected Users
        /// </summary>
        public static Dictionary<int, string> ClientInformation = new Dictionary<int, string>();

        /// <summary>
        /// Should only be used after the client is successfully authenticated  
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public static void GenerateOneTimeCode(int clientID)
        {
            long unix = DateTimeOffset.Now.ToUnixTimeSeconds();
            string hashedClientInfo = ComputeSha256Hash(unix.ToString());
            string onetimecode = hashedClientInfo.Substring(0, hashedClientInfo.Length / 3);

            ClientInformation.Add(clientID, hashedClientInfo);


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
