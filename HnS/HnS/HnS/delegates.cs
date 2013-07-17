using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace HnS
{
    public delegate void ConnectionEvent(object sender, TcpClient user);
    public delegate void DataReceivedEvent(TcpClient sender, byte[] data);
}
