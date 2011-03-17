// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientRegistry.cs" company="Alexey Zakharov">
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
//   Defines the ClientRegistry type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Witcraft.SocketsLight.Server
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// </summary>
    public class ClientRegistry
    {
        /// <summary>
        /// </summary>
        private readonly SortedDictionary<Guid, Client> clients = new SortedDictionary<Guid, Client>();

        /// <summary>
        /// </summary>
        private readonly object syncLock = new object();

        /// <summary>
        /// </summary>
        /// <param name="client">
        /// </param>
        public void Add(Client client)
        {
            lock (this.syncLock)
            {
                this.clients.Add(client.ID, client);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId">
        /// </param>
        /// <returns>
        /// </returns>
        public bool Contains(Guid clientId)
        {
            return this.clients.ContainsKey(clientId);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public IList<Client> GetAll()
        {
            var list = new List<Client>();
            foreach (var pair in this.clients)
            {
                list.Add(pair.Value);
            }

            return list.AsReadOnly();
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId">
        /// </param>
        /// <returns>
        /// </returns>
        public Client GetById(Guid clientId)
        {
            if (this.clients.ContainsKey(clientId))
            {
                return this.clients[clientId];
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="clientId">
        /// </param>
        public void Remove(Guid clientId)
        {
            lock (this.syncLock)
            {
                this.clients.Remove(clientId);
            }
        }
    }
}