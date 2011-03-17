// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientStatusChangedEventArgs.cs" company="Alexey Zakharov">
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
//   Holds unique identifier of connected or disconnected client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Witcraft.SocketsLight.Server
{
    using System;

    /// <summary>
    /// Holds unique identifier of connected or disconnected client.
    /// </summary>
    public class ClientStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientStatusChangedEventArgs"/> class.
        /// </summary>
        /// <param name="clientId">
        /// Client unique identifier.
        /// </param>
        public ClientStatusChangedEventArgs(Guid clientId)
        {
            this.ClientId = clientId;
        }

        /// <summary>
        /// Gets client unique identifier.
        /// </summary>
        public Guid ClientId { get; private set; }
    }
}