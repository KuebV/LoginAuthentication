using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginAuthentication.Foundation
{
    public class DataRetreive
    {
        /// <summary>
        /// Constructor
        /// All Methods & Variables are used Internally
        /// </summary>
        /// <param name="JsonData"></param>
        public DataRetreive(string JsonData)
        {
            JsonClientData = JsonData;
        }

        private string JsonClientData;

        /// <summary>
        /// Get Data from the Json Variable
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal string GetValueFromJson(ClientDatabaseEnum value)
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
