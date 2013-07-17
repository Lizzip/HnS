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
using System.IO;

namespace HnS
{
    class Networking
    {
        TcpClient client;
        string localIP = "2.124.124.169";
        int port = 1490;
        int bufferSize = 2048;
        byte[] readBuffer;
        public MemoryStream readStream, writeStream;
        public BinaryReader reader;
        public BinaryWriter writer;
        EntityManager entityManager;
        Hero hero, player2;
        Debugger debugger;

        public Networking(HnS.Game1 game1, EntityManager eManager, Debugger deb, bool serverEnabled)
        {
            //Get entities and manager
            entityManager = eManager;
            hero = entityManager.getHero();
            player2 = entityManager.getPlayer2();
            debugger = deb;

            //Establish connection
            if (serverEnabled)
            {
                client = new TcpClient();
                client.NoDelay = true;
                client.Connect(localIP, port);
                readBuffer = new byte[bufferSize];

                client.GetStream().BeginRead(readBuffer, 0, bufferSize, StreamRecieved, null);

                readStream = new MemoryStream();
                reader = new BinaryReader(readStream);

                writeStream = new MemoryStream();
                writer = new BinaryWriter(writeStream);
            }
        }

        private void StreamRecieved(IAsyncResult ar)
        {
            int bytesRead = 0;

            try
            {
                lock (client.GetStream())
                {
                    bytesRead = client.GetStream().EndRead(ar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (bytesRead == 0)
            {
                client.Close();
                return;
            }

            byte[] data = new byte[bytesRead];

            for (int i = 0; i < bytesRead; i++)
            {
                data[i] = readBuffer[i];
            }

            ProcessData(data);

            client.GetStream().BeginRead(readBuffer, 0, bufferSize, StreamRecieved, null);
        }

        private void ProcessData(byte[] data)
        {
            readStream.SetLength(0);
            readStream.Position = 0;

            readStream.Write(data, 0, data.Length);
            readStream.Position = 0;

            Protocol p;

            try
            {
                p = (Protocol)reader.ReadByte();

                //Player Connect and Disconnect
                if (p == Protocol.Connected)
                {
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                    debugger.Out("Player 2 has connected");

                    if (player2.getExists() == false)
                    {
                        player2.setExists(true);
                        writeStream.Position = 0;
                        writer.Write((byte)Protocol.Connected);
                        SendData(GetDataFromMemoryStream(writeStream));
                    }
                }
                else if (p == Protocol.Disconnected)
                {
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                    debugger.Out("Player 2 has disconnected");

                    player2.setExists(false);
                }
                else if (p == Protocol.PlayerMoved)
                {
                    float px, py;
                    px = reader.ReadSingle();
                    py = reader.ReadSingle();
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                    player2.setRecievedInfo(new Vector2(px, py));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public byte[] GetDataFromMemoryStream(MemoryStream ms)
        {
            byte[] result;

            //Async method called this, so lets lock the object to make sure other threads/async calls need to wait to use it.
            lock (ms)
            {
                int bytesWritten = (int)ms.Position;
                result = new byte[bytesWritten];

                ms.Position = 0;
                ms.Read(result, 0, bytesWritten);
            }

            return result;
        }

        public void SendData(byte[] b)
        {
            //Try to send the data.  If an exception is thrown, disconnect the client
            try
            {
                lock (client.GetStream())
                {
                    client.GetStream().BeginWrite(b, 0, b.Length, null, null);
                }
            }
            catch (Exception e)
            {
               
            }
        }

    }
}
