using com.benjaminapplegate.EasyNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerExample
{
    public class ServerSendPacket
    {
        public static void SendServerClosing()
        {
            Packet packet = new Packet((int)ServerPackets.ServerClosing);
            Program._server.SendPacketToAll(packet);
        }
    }
}
