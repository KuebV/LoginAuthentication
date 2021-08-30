using Newtonsoft.Json;
using ServerAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginAuthentication
{
    public class ClientFoundation
    {
        public ClientFoundation(string JsonData)
        {
            JsonClientData = JsonData;
        }

        private string JsonClientData;

        /// <summary>
        /// Get Data from the Json Variable
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetValueFromJson(ClientDatabaseEnum value)
        {
            ClientDatabase clientData = JsonConvert.DeserializeObject<ClientDatabase>(JsonClientData);
            switch (value)
            {
                case ClientDatabaseEnum.AuthenticationString:
                    return clientData.AuthenticationString;
                case ClientDatabaseEnum.Hostname:
                    return clientData.Hostname;
                case ClientDatabaseEnum.IPAddress:
                    return clientData.IPAddress;
                case ClientDatabaseEnum._id:
                    return clientData._id;
            }

            return null;
        }
    }
}
