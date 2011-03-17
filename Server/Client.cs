// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" company="Alexey Zakharov">
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
//   Message client
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Witcraft.SocketsLight.Server
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Message client
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="tcpClient">
        /// </param>
        public Client(TcpClient tcpClient)
        {
            this.TcpClient = tcpClient;
            this.ID = Guid.NewGuid();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Client"/> class from being created.
        /// </summary>
        private Client()
        {
        }

        /// <summary>
        /// Gets client uniqueidentifier
        /// </summary>
        public Guid ID { get; private set; }

        /// <summary>
        /// Gets connection object
        /// </summary>
        public TcpClient TcpClient { get; private set; }
    }
}