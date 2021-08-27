using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonDB;

namespace ServerAuthentication
{
    public class Database
    {
        #region Private Variables for JsonDB
        private static JsonDB.Database db;
        private static Collection collection;
        private static Server server;
        private static Item item;
        #endregion

        #region Private Variables for MongoDB
        private static MongoClient mongoClient;
        private static IMongoDatabase mongoDatabase;
        public static IMongoCollection<BsonDocument> mongoCollection;
        #endregion


        /// <summary>
        /// Start the Database for Local Storage
        /// </summary>
        /// <param name="DatabaseName"></param>
        /// <param name="CollectionName"></param>
        public static void StartDatabase(string DatabaseName, string CollectionName)
        {
            // This leaves the option of Mongo Storage Viable, due to how JsonDB & MongoDB are formatted
            // JsonDB takes direct inspiration from MongoDB in terms of Items, Collections, and Database structuring
            if (server.m_LocalStorageEnabled)
            {
                db = new JsonDB.Database(DatabaseName);
                collection = new Collection(db, CollectionName, false);

                db.CheckDB();
                collection.InitializeCollection();
            }
            else
            {
                if (string.IsNullOrEmpty(server.m_MongoURL))
                {
                    Output.Message(OutputType.Error, "MongoURL is Empty!");
                    return;
                }

                mongoClient = new MongoClient(server.m_MongoURL);
                mongoDatabase = mongoClient.GetDatabase(DatabaseName);
                mongoCollection = mongoDatabase.GetCollection<BsonDocument>(CollectionName);
            }
        }

        /// <summary>
        /// Creates a new Item in the Database, does allow for both Local Storage and Cloud MongoDB Storage
        /// For LocalStorage, be sure clientData is defined in the method
        /// For Cloud Mongo Storage, be sure bsonDocument is defined in the method
        /// 
        /// It will throw an error if these are not met
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientData"></param>
        /// <param name="bsonDocument"></param>
        public static void CreateNewItem(dynamic id, ClientDatabase clientData = null, BsonDocument bsonDocument = null)
        {
            // Use Local Storage if Enabled
            if (server.m_LocalStorageEnabled)
            {
                if (string.IsNullOrEmpty(clientData.ToString()))
                {
                    Output.Message(OutputType.Error, "ClientData is Empty!");
                    return;
                }
                item = new Item(db, collection);
                if (clientData.AuthenticationString != null)
                    item.AddItem(id, clientData);
            }
            // If Local Storage is not enabled, then we assume that MongoDB is being used
            else
            {
                // If for whatever reason, the user decides to not input a BsonDocument, throw an error
                if (bsonDocument.IsBsonNull)
                {
                    Output.Message(OutputType.Error, "BsonDocument is empty");
                    return;
                }

                // As the Method States, we are creating a new Item, not editing an existing one    
                var collection = mongoCollection.Find(new BsonDocument("_id", id)).ToList();
                if (collection.Count >= 1)
                {
                    Output.Message(OutputType.Error, "Document already exists in Database!");
                    return;
                }

            }
        }
    }

    public class ClientDatabase
    {
        public string _id { get; set; }
        public string Hostname { get; set; }
        public string IPAddress { get; set; }
        public string AuthenticationString { get; set; }
    }

}
