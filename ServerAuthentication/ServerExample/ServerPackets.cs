using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerExample
{
    public enum ServerPackets
    {
        Message = 1,
        ServerClosing = 2,
        ClientDisconnecting = 3,
        ServerFull = 4
    }
}
