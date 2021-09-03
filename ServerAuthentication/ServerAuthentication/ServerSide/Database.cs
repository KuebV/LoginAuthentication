using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonDB;
using LoginAuthentication;
using LoginAuthentication.Foundation;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoginAuthentication.ServerSide
{
    public class Database
    {
        #region Private Variables for JsonDB
        private static JsonDB.Database db;
        private static Collection collection;
        public static Item item;
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
            Output.Message(OutputType.Info, "Starting Database...");

            // This leaves the option of Mongo Storage Viable, due to how JsonDB & MongoDB are formatted
            // JsonDB takes direct inspiration from MongoDB in terms of Items, Collections, and Database structuring

            if (Server.m_LocalStorageEnabled)
            {
                db = new JsonDB.Database(DatabaseName);
                collection = new Collection(db, CollectionName, false);
                item = new Item(db, collection);

                // It should be mentioned for future me to read, there needs to be Info Messages here because of how JsonDB Loads
                // tl;dr, its too quick and needs to be slowed down by a few miliseconds, and an output fixes it.

                db.CheckDB();
                Output.Message(OutputType.Info, "Database has been checked");

                collection.InitializeCollection();
                Output.Message(OutputType.Info, "Collection has been initialized");
            }
            else
            {
                if (string.IsNullOrEmpty(Server.m_MongoURL))
                {
                    Output.Message(OutputType.Error, "MongoURL is Empty!");
                    return;
                }

                mongoClient = new MongoClient(Server.m_MongoURL);
                mongoDatabase = mongoClient.GetDatabase(DatabaseName);
                mongoCollection = mongoDatabase.GetCollection<BsonDocument>(CollectionName);
            }

            Output.Message(OutputType.Info, "Database is ready!");
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
            if (Server.m_LocalStorageEnabled)
            {
                if (string.IsNullOrEmpty(clientData.ToString()))
                {
                    Output.Message(OutputType.Error, "ClientData is Empty!");
                    return;
                }
                if (clientData.AuthenticationString != null)
                    item.AddItem(id, clientData);
            }
            else
            {
                if (bsonDocument.IsBsonNull)
                {
                    Output.Message(OutputType.Error, "BsonDocument is empty");
                    return;
                }

                var collection = mongoCollection.Find(new BsonDocument("_id", id)).ToList();
                if (collection.Count >= 1)
                {
                    Output.Message(OutputType.Error, "Document already exists in Database!");
                    return;
                }

            }
        }

    }

}