﻿using System;
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
    public class Networking
    {
        #region Variables

        TcpClient client;
        string localIP = "127.0.0.1", ip;
        byte id;
        int port = 1490;
        int bufferSize = 2048;
        byte[] readBuffer;
        public MemoryStream readStream, writeStream;
        public BinaryReader reader;
        public BinaryWriter writer;
        EntityManager entityManager;
        Hero hero, player2;
        Debugger debugger;

        #endregion

        #region Constructors
        public Networking()
        {
            //Get entities and manager
            bool serverEnabled = true;
            entityManager = Game1.entityManager;
            hero = entityManager.getHero();
            player2 = entityManager.getPlayer2();
            debugger = Game1.debugger;

            try
            {
                client = new TcpClient();
                client.NoDelay = true;
                client.Connect(localIP, port);
            }
            catch (Exception ex)
            {
                serverEnabled = false;
                Game1.enableNetworking = false;
            }

            if (serverEnabled)
            {
                Game1.enableNetworking = true;
                readBuffer = new byte[bufferSize];

                client.GetStream().BeginRead(readBuffer, 0, bufferSize, StreamRecieved, null);

                readStream = new MemoryStream();
                reader = new BinaryReader(readStream);

                writeStream = new MemoryStream();
                writer = new BinaryWriter(writeStream);

                debugger.Out("Connected to server: " + localIP);
            }
        }
        #endregion

        #region Handle Data
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

                if (p == Protocol.Connected) //Player Connected
                {
                    id = reader.ReadByte();
                    ip = reader.ReadString();
                    debugger.Out("Player 2 has connected");

                    if (player2.getExists() == false)
                    {
                        player2.setExists(true);
                        writeStream.Position = 0;
                        writer.Write((byte)Protocol.Connected);
                        SendData(GetDataFromMemoryStream(writeStream));
                    }
                }
                else if (p == Protocol.Disconnected) //Player Disconnected
                {
                    id = reader.ReadByte();
                    ip = reader.ReadString();
                    debugger.Out("Player 2 has disconnected");

                    player2.setExists(false);
                }
                else if (p == Protocol.PlayerMoved) //Player Moved
                {
                    float px, py;
                    px = reader.ReadSingle(); 
                    py = reader.ReadSingle();
                    id = reader.ReadByte();
                    ip = reader.ReadString(); // -- Throwing error - Don't know why yet. TO FIX
                    player2.setRecievedInfo(new Vector2(px, py));
                }
                else if (p == Protocol.PlayerAnimationState)
                {
                    List<float> values = new List<float>();
                    int floatValues = 6;
                    bool bloodActive;
                    
                    for (int i = 0; i < floatValues; i++)
                    {
                        values.Add(reader.ReadSingle());
                    }
                    
                    bloodActive = reader.ReadBoolean();

                    if (bloodActive)
                    {
                        values.Add(1.0f);
                        values.Add(reader.ReadSingle());
                        values.Add(reader.ReadSingle());
                    }
                    else values.Add(-1.0f);

                    player2.getAnimationState(values);
                }
                else if (p == Protocol.KeyPressDown)
                {
                    int key, numKeys = reader.ReadInt32();
                    
                    if (numKeys > 0)
                    {
                        Microsoft.Xna.Framework.Input.Keys[] pressed = new Microsoft.Xna.Framework.Input.Keys[numKeys];

                        for (int i = 0; i < numKeys; i++)
                        {
                            key = reader.ReadInt32();
                            pressed[i] = (Microsoft.Xna.Framework.Input.Keys)key;
                        }

                        player2.getPressedKeys(pressed);
                    }

                    id = reader.ReadByte();
                    ip = reader.ReadString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Helper
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
        #endregion

        #region Transmit Data
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
        #endregion

    }
}
