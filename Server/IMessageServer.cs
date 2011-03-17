// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageServer.cs" company="Alexey Zakharov">
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
//   Recieves and send messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Witcraft.SocketsLight.Server
{
    using System;

    /// <summary>
    /// Recieves and send messages.
    /// </summary>
    public interface IMessageServer
    {
        /// <summary>
        /// Occurs when new client has connected
        /// </summary>
        event EventHandler<ClientStatusChangedEventArgs> ClientConnected;

        /// <summary>
        /// Occurs when client has disconnected
        /// </summary>
        event EventHandler<ClientStatusChangedEventArgs> ClientDisconnected;

        /// <summary>
        /// Occurs when new message has been recieved
        /// </summary>
        event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        /// <summary>
        /// </summary>
        /// <param name="clientId">
        /// </param>
        /// <param name="message">
        /// </param>
        void SendMessageToClient(Guid clientId, Message message);

        /// <summary>
        /// Start listnening for incoming messages.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop listening for incoming messages.
        /// </summary>
        void Stop();
    }
}