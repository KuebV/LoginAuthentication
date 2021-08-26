using com.benjaminapplegate.EasyNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerExample
{
    class ServerHandlePacket
    {
        public static void HandleMessage(int clientId, Packet packet)
        {

            Packet packetToSend = new Packet((int)ServerPackets.Message);
            string packetRead = packet.ReadString();
            Console.WriteLine("Incoming Packet : {0}", packetRead);

            Program._server.SendPacketToId(packetToSend, clientId);


        }
        public static void HandleClientDisconnect(int clientId, Packet packet)
        {
            Console.WriteLine($"Client {clientId} has disconnected from the server");
            Program._server.ConnectedClients[clientId].Close();
            Program._server.ConnectedClients[clientId] = null;
        }
    }
}
