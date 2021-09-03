using com.benjaminapplegate.EasyNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LoginAuthentication;

namespace ServerExample
{
    class Program
    {

        public static EasyServer _server;
        public static Server authServer;

        public static void ClientConnect(TcpClient client, int clientId)
        {
            return;
        }

        public static void ServerFullConnect(TcpClient client, int clientId)
        {
            Packet packet = new Packet((int)ServerPackets.ServerFull);
            _server.SendPacketToTcpClient(packet, client);
        }

        static void Main(string[] args)
        {

            Server server = new Server(UseLocalStorage: true, MongoURL: null);
            Database.StartDatabase("LocalAuthentication", "ExampleAuthentication");

            var clientSettings = new ClientAuthenticationSettings
            {
                HostNameMatch = false,
                IPMatch = false
            };


            ClientData clientData = new ClientData(1, clientSettings);
            LoginResponse responseFromServer = clientData.HandleLoginData("127.0.0.1",
                "DESKTOP-D219KL",
                "3c2b70d35fb3cae66b6fdfe9d5b1650a86a24de1a81e5f24aa23ffd9c2640828",
                "3C2B70D35FB3CA");

            switch (responseFromServer)
            {
                case LoginResponse.Good:
                    clientData.GenerateOneTimeCode();
                    string code = clientData.GetOneTimeCode();
                    Console.WriteLine(code);
                    break;
                default:
                    Console.WriteLine("Something broke");
                    break;
            }
        }
    }
}
