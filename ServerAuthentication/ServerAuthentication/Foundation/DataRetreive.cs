using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Threading.Tasks;
using LoginAuthentication.ServerSide;

namespace LoginAuthentication.Foundation
{
    public class DataRetreive
    {
        /// <summary>
        /// Constructor
        /// All Methods & Variables are used Internally
        /// </summary>
        /// <param name="JsonData"></param>
        public DataRetreive(string JsonData, dynamic _id = null)
        {
            JsonClientData = JsonData;
        }

        private string JsonClientData;
        private dynamic id;

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

        internal dynamic GetValueFromBson(ClientDatabaseEnum value)
        {
            var collection = Database.mongoCollection;
            var listDoc = collection.Find(new BsonDocument("_id", id)).ToList();

            if (listDoc.Count < 1)
                return null;

            foreach(var doc in listDoc)
            {
                string enumValue = Enum.GetName(typeof(ClientDatabaseEnum), value);
                return doc[enumValue];
            }

            Output.Message(OutputType.Error, "Invalid Bson Value!");
            return null;
        }
    }
}
