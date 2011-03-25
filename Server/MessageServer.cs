// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageServer.cs" company="Alexey Zakharov">
//   This file is part of SocketsLight Framework.
//
//   SocketsLight Framework is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or 
//   (at your option) any later version. 
//   
//   SocketsLight Framework is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of 
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
//   GNU General Public License for more details. 
//   
//   You should have received a copy of the GNU General Public License 
//   along with Foobar.  If not, see http://www.gnu.org/license.
// </copyright>
// <summary>
//   Recieves and cast messages.
// </summary>
//
//
// History:
// March 25, 2011 - Dan O'Neil - Added a fix for a Null Reference when server is shutting down.
//                             - Added a new method to send raw strings to clients.
// --------------------------------------------------------------------------------------------------------------------


namespace Witcraft.SocketsLight.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.IO;
    using System.Text;
    using System.Diagnostics;

    /// <summary>
    /// Recieves and send messages.
    /// </summary>
    public class MessageServer : IMessageServer
    {
        /// <summary>
        /// </summary>
        private readonly ClientRegistry clientRegistry = new ClientRegistry();

        /// <summary>
        /// Tcp listner used to listen incoming socket messages.
        /// </summary>
        private readonly TcpListener listener;

        /// <summary>
        /// Serialize and deserialize messages.
        /// </summary>
        private readonly IMessageSerializer messageSerializer;

        /// <summary>
        /// Indicates that sever is stopped.
        /// </summary>
        private bool isStopped;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageServer"/> class.
        /// </summary>
        /// <param name="address">
        /// </param>
        /// <param name="port">
        /// </param>
        /// <param name="messageSerializer">
        /// </param>
        public MessageServer(IPAddress address, int port, IMessageSerializer messageSerializer)
        {
            this.listener = new TcpListener(address, port);
            this.messageSerializer = messageSerializer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageServer"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used for raw bytes, no serialization.
        /// </remarks>
        /// <param name="address">
        /// </param>
        /// <param name="port">
        /// </param>
        public MessageServer(IPAddress address, int port)
        {
            this.listener = new TcpListener(address, port);
        }


        /// <summary>
        /// Occurs when new client has connected
        /// </summary>
        public event EventHandler<ClientStatusChangedEventArgs> ClientConnected;

        /// <summary>
        /// Occurs when client has disconnected
        /// </summary>
        public event EventHandler<ClientStatusChangedEventArgs> ClientDisconnected;

        /// <summary>
        /// Occurs when new message has been recieved
        /// </summary>
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        /// <summary>
        /// </summary>
        /// <param name="clientId">
        /// </param>
        /// <param name="message">
        /// </param>
        public void SendMessageToClient(Guid clientId, Message message)
        {
            Client client = this.clientRegistry.GetById(clientId);

            if (client != null)
            {
                var args = new SocketAsyncEventArgs();

                byte[] serializedMessage = this.messageSerializer.Serialize(message);
                List<byte> messageByteList = serializedMessage.ToList();

                for (int i = 1023; i < serializedMessage.Length; i += 1023)
                {
                    messageByteList.Insert(i, 0);
                    i++;
                }

                messageByteList.Add(1);

                args.SetBuffer(messageByteList.ToArray(), 0, messageByteList.Count);

                try
                {
                    client.TcpClient.Client.SendAsync(args);
                }
                catch (Exception)
                {
                    this.clientRegistry.Remove(client.ID);
                    client.TcpClient.Close();
                    this.OnClientDisconnected(client.ID);
                }
            }
        }

        /// <summary>
        /// Sends a raw, unserialized string to a client.
        /// </summary>
        /// <remarks>
        /// Converts the string to a UTF8 byte array before sending.
        /// </remarks>
        /// <param name="clientId">
        /// </param>
        /// <param name="message">
        /// </param>
        public void SendUTF8StringToClient(Guid clientId, string messagestring)
        {
            Client client = this.clientRegistry.GetById(clientId);
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            // Get the byte array
            byte[] straightBytes = encoding.GetBytes(messagestring);

            if (client != null)
            {
                var args = new SocketAsyncEventArgs();

                args.SetBuffer(straightBytes, 0, straightBytes.Length);

                try
                {
                    client.TcpClient.Client.SendAsync(args);
                }
                catch (Exception)
                {
                    this.clientRegistry.Remove(client.ID);
                    client.TcpClient.Close();
                    this.OnClientDisconnected(client.ID);
                }
            }
        }

        /// <summary>
        /// Start listnening for incoming messages.
        /// </summary>
        public void Start()
        {
            this.listener.Start();
            this.listener.BeginAcceptTcpClient(this.OnAcceptTcpClient, null);
        }

        /// <summary>
        /// Stop listening for incoming messages.
        /// </summary>
        public void Stop()
        {
            this.isStopped = true;
            if (this.listener != null)
            {
                this.listener.Stop();
            }

            foreach (Client client in this.clientRegistry.GetAll())
            {
                client.TcpClient.Close();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="clientMessageReceivedState">
        /// </param>
        private void BeginReceiveClientMessages(ClientMessageReceivedState clientMessageReceivedState)
        {
            try
            {
                clientMessageReceivedState.Client.TcpClient.Client.BeginReceive(
                    clientMessageReceivedState.MessagePacket,
                    0,
                    clientMessageReceivedState.MessagePacket.Length,
                    SocketFlags.None,
                    this.ClientMessageReceived,
                    clientMessageReceivedState);
            }
            catch (SocketException e)
            {
                // TODO:  Add logger here.
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ar">
        /// </param>
        private void ClientMessageReceived(IAsyncResult ar)
        {
            var state = (ClientMessageReceivedState)ar.AsyncState;
            try
            {
                // 3/25/11 Dan O'Neil - If a client was connected to a server, and you tried to stop the server, this was causing a "hang".
                // Added this null check for a quicker server close.
                if (state.Client.TcpClient.Client == null)
                {
                    Debug.WriteLine("Could not find a client, leaving ClientMessageReceived");
                    return;
                }

                int bytesRead = state.Client.TcpClient.Client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.SerializedMessage.AddRange(state.MessagePacket.Take(bytesRead - 1));

                    if (state.MessagePacket[bytesRead - 1] > 0)
                    {
                        this.OnMessageRecieved(state.Client.ID, state.SerializedMessage.ToArray());
                        state.SerializedMessage.Clear();
                    }
                }

                this.BeginReceiveClientMessages(state);
            }
            catch (SocketException)
            {
                this.clientRegistry.Remove(state.Client.ID);
                this.OnClientDisconnected(state.Client.ID);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="asyncResult">
        /// </param>
        private void OnAcceptTcpClient(IAsyncResult asyncResult)
        {
            if (this.isStopped)
            {
                return;
            }

            this.listener.BeginAcceptTcpClient(this.OnAcceptTcpClient, null);

            TcpClient tcpClient = this.listener.EndAcceptTcpClient(asyncResult);

            var client = new Client(tcpClient);

            this.BeginReceiveClientMessages(
                new ClientMessageReceivedState
                {
                    MessagePacket = new byte[1024],
                    SerializedMessage = new List<byte>(),
                    Client = client
                });

            this.clientRegistry.Add(client);

            this.OnClientConnected(client.ID);

        }

        /// <summary>
        /// Invokes <see cref="ClientConnected"/> event.
        /// </summary>
        /// <param name="clientId">
        /// Client uniqueidentifier
        /// </param>
        private void OnClientConnected(Guid clientId)
        {
            EventHandler<ClientStatusChangedEventArgs> handler = this.ClientConnected;
            if (handler != null)
            {
                handler(this, new ClientStatusChangedEventArgs(clientId));
            }
        }

        /// <summary>
        /// Invokes <see cref="ClientDisconnected"/> event.
        /// </summary>
        /// <param name="clientId">
        /// Client uniqueidentifier
        /// </param>
        private void OnClientDisconnected(Guid clientId)
        {
            EventHandler<ClientStatusChangedEventArgs> handler = this.ClientDisconnected;
            if (handler != null)
            {
                handler(this, new ClientStatusChangedEventArgs(clientId));
            }
        }

        /// <summary>
        /// Invokes <see cref="MessageRecieved"/> event.
        /// </summary>
        /// <param name="clientId">
        /// Uniqueidentifier of client, which sent the serializedMessage.
        /// </param>
        /// <param name="serializedMessage">
        /// Recived serialized message.
        /// </param>
        private void OnMessageRecieved(Guid clientId, byte[] serializedMessage)
        {
            EventHandler<MessageRecievedEventArgs> handler = this.MessageRecieved;
            if (handler != null)
            {
                handler(
                    this, new MessageRecievedEventArgs(clientId, this.messageSerializer.Deserialize(serializedMessage)));
            }
        }

        /// <summary>
        /// Inner object used as async method state.
        /// </summary>
        private class ClientMessageReceivedState
        {
            /// <summary>
            /// Client.
            /// </summary>
            public Client Client { get; set; }

            /// <summary>
            /// MessagePacket.
            /// </summary>
            public byte[] MessagePacket { get; set; }

            /// <summary>
            /// SerializedMessage
            /// </summary>
            public List<byte> SerializedMessage { get; set; }
        }
    }
}