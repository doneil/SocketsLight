// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageClient.cs" company="Alexey Zakharov">
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
//   Defines the IMessageClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Witcraft.SocketsLight.Client
{
    using System;
    using System.ComponentModel;

    using Witcraft.SocketsLight.Server;

    /// <summary>
    /// </summary>
    public interface IMessageClient
    {
        /// <summary>
        /// Occurs when client has connected to message server
        /// </summary>
        event EventHandler<AsyncCompletedEventArgs> ConnectCompleted;

        /// <summary>
        /// Occurs when message is sent
        /// </summary>
        event EventHandler<AsyncCompletedEventArgs> SendCompleted;

        /// <summary>
        /// Occure if connection with message server has been lost.
        /// </summary>
        event EventHandler<EventArgs> ConnectionLost;

        /// <summary>
        /// Occurs when new message have been recieved.
        /// </summary>
        event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        /// <summary>
        /// Indicates if client is connected to message server.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Connect to message server async.
        /// </summary>
        /// <param name="state">
        /// </param>
        void ConnectAsync();

        /// <summary>
        /// Disconnects from message server.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Connect to message server async.
        /// </summary>
        /// <param name="state">
        /// </param>
        void ConnectAsync(object state);

        /// <summary>
        /// Send message.
        /// </summary>
        /// <param name="message">
        /// </param>
        void SendMessageAsync(Message message);

        /// <summary>
        /// Send message.
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="userState">
        /// </param>
        void SendMessageAsync(Message message, object userState);
    }
}