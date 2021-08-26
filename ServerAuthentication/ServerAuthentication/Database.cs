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
        private static JsonDB.Database db;
        private static Collection collection;
        private static Server server;
        private static Item item;

        public static void StartDatabase(string DatabaseName, string CollectionName)
        {
            db = new JsonDB.Database(DatabaseName);
            collection = new Collection(db, CollectionName, false);

            db.CheckDB();
            collection.InitializeCollection();
        }

        public static void CreateNewItem(string id, ClientDatabase clientData)
        {
            item = new Item(db, collection);
            if (clientData.AuthenticationString != null)
                item.AddItem(id, clientData);
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
