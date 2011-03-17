// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageClient.cs" company="Alexey Zakharov">
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
//   Defines the MessageClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Witcraft.SocketsLight.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Windows;

    using Witcraft.SocketsLight.Server;

    /// <summary>
    /// Send and recives messages.
    /// </summary>
    public class MessageClient : IMessageClient
    {
        /// <summary>
        /// Used to aggregate message frames.
        /// </summary>
        private List<byte> messageBuffer = new List<byte>();

        /// <summary>
        /// Serialize and deserialize messages.
        /// </summary>
        private IMessageSerializer messageSerializer;

        /// <summary>
        /// Port which will be used for message transfer.
        /// </summary>
        private int port;

        /// <summary>
        /// Socket which will be used for message transfer.
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageClient"/> class.
        /// </summary>
        /// <param name="port">
        /// </param>
        /// <param name="messageSerializer">
        /// </param>
        public MessageClient(int port, IMessageSerializer messageSerializer)
        {
            this.port = port;
            this.messageSerializer = messageSerializer;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="MessageClient"/> class from being created.
        /// </summary>
        private MessageClient()
        {
        }

        /// <summary>
        /// Occurs when client has connected to message server
        /// </summary>
        public event EventHandler<AsyncCompletedEventArgs> ConnectCompleted;

        /// <summary>
        /// Occure if connection with message server has been lost.
        /// </summary>
        public event EventHandler<EventArgs> ConnectionLost;

        /// <summary>
        /// Occurs when new message have been recieved.
        /// </summary>
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        /// <summary>
        /// Occurs when message is sent
        /// </summary>
        public event EventHandler<AsyncCompletedEventArgs> SendCompleted;

        /// <summary>
        /// Indicates if client is connected to message server.
        /// </summary>
        public bool Connected
        {
            get
            {
                return this.socket.Connected;
            }
        }

        /// <summary>
        /// Connect to message server async.
        /// </summary>
        /// <param name="state">
        /// </param>
        public void ConnectAsync()
        {
            this.ConnectAsync(null);
        }

        /// <summary>
        /// Connect to message server async.
        /// </summary>
        /// <param name="state">
        /// </param>
        public void ConnectAsync(object state)
        {
            if (!this.Connected)
            {
                var endPoint = new DnsEndPoint(Application.Current.Host.Source.DnsSafeHost, this.port);
                var args = new SocketAsyncEventArgs { UserToken = state, RemoteEndPoint = endPoint };
                args.Completed += this.OnSocketConnected;
                this.socket.ConnectAsync(args);
            }
        }

        /// <summary>
        /// Disconnect from message server.
        /// </summary>
        public void Disconnect()
        {
            this.socket.Close();
        }

        /// <summary>
        /// Send message.
        /// </summary>
        /// <param name="message">
        /// </param>
        public void SendMessageAsync(Message message)
        {
            this.SendMessageAsync(message, null);
        }

        /// <summary>
        /// Send message.
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="userState">
        /// </param>
        public void SendMessageAsync(Message message, object userState)
        {
            if ((this.socket == null) || (this.Connected == false))
            {
                // TODO : throw exception.
            }

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
            args.Completed += this.SocketSendCompleted;
            args.UserToken = userState;

            this.socket.SendAsync(args);
        }

        /// <summary>
        /// </summary>
        /// <param name="e">
        /// </param>
        private void OnConnected(AsyncCompletedEventArgs e)
        {
            EventHandler<AsyncCompletedEventArgs> handler = this.ConnectCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="e">
        /// </param>
        private void OnConnectionLost()
        {
            EventHandler<EventArgs> handler = this.ConnectionLost;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="e">
        /// </param>
        private void OnMessageRecieved(MessageRecievedEventArgs e)
        {
            EventHandler<MessageRecievedEventArgs> handler = this.MessageRecieved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="e">
        /// </param>
        private void OnSendCompleted(AsyncCompletedEventArgs e)
        {
            EventHandler<AsyncCompletedEventArgs> handler = this.SendCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void OnSocketConnected(object sender, SocketAsyncEventArgs e)
        {
            AsyncCompletedEventArgs asyncArgs;
            if (this.Connected)
            {
                var response = new byte[1024];
                e.SetBuffer(response, 0, response.Length);
                e.Completed -= this.OnSocketConnected;
                e.Completed += this.OnSocketReceive;

                this.socket.ReceiveAsync(e);
                asyncArgs = new AsyncCompletedEventArgs(null, false, e.UserToken);
                this.OnConnected(asyncArgs);
            }

            // TODO: Handle error while connection.
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void OnSocketReceive(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 0)
            {
                this.OnConnectionLost();

                this.socket.Close();

                return;
            }

            this.messageBuffer.AddRange(e.Buffer.Take(e.BytesTransferred - 1));
            if (e.Buffer[e.BytesTransferred - 1] > 0)
            {
                this.OnMessageRecieved(
                    new MessageRecievedEventArgs(this.messageSerializer.Deserialize(this.messageBuffer.ToArray())));
                this.messageBuffer.Clear();
            }

            this.socket.ReceiveAsync(e);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void SocketSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                this.OnSendCompleted(new AsyncCompletedEventArgs(null, false, e.UserToken));
            }

            // TODO: Handle error if send is not successful.
        }
    }
}