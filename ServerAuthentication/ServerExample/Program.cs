using com.benjaminapplegate.EasyNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerAuthentication;
using System.IO;

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

            _server = new EasyServer(5000, 7700);
            _server.AddPacketHandler((int)ServerPackets.Message, ServerHandlePacket.HandleMessage);
            _server.AddPacketHandler((int)ServerPackets.ClientDisconnecting,
                ServerHandlePacket.HandleClientDisconnect);


            _server.clientConnection = ClientConnect;
            _server.serverFullConnection = ServerFullConnect;

            _server.StartServer();

            string localDirectory = Directory.GetCurrentDirectory();

            authServer = new Server(UseLocalStorage: true);
            Database.StartDatabase("LocalDatabase", "ClientIdentities");

            bool serverRunning = true;
            while (serverRunning)
            {
                if (Console.ReadLine().Equals("exit"))
                {
                    ServerSendPacket.SendServerClosing();
                    _server.Stop();
                    Environment.Exit(0);
                }
            }
        }
    }
}
