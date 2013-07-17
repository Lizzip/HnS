using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;

namespace HnS
{
    class Networking
    {
        TcpClient client;
        string localIP = "127.0.0.1";
        int port = 1490;
        int bufferSize = 2048;
        byte[] readBuffer;

        public Networking(HnS.Game1 game1)
        {
            client = new TcpClient();
            client.NoDelay = true;
            client.Connect(localIP, port);
            readBuffer = new byte[bufferSize];

            client.GetStream().BeginRead(readBuffer, 0, bufferSize, StreamRecieved, null);
        }

        private void StreamRecieved(IAsyncResult ar)
        {
            client.GetStream().BeginRead(readBuffer, 0, bufferSize, StreamRecieved, null);
        }

    }
}
