// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageSerializer.cs" company="Alexey Zakharov">
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
//   Defines the IMessageSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Witcraft.SocketsLight.Server
{
    /// <summary>
    /// Serialize and deserialize messages.
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Serializer message to byte array.
        /// </summary>
        /// <param name="message">
        /// Message to serialize
        /// </param>
        /// <returns>
        /// Serialized message.
        /// </returns>
        byte[] Serialize(Message message);

        /// <summary>
        /// Deserializer message from byte array.
        /// </summary>
        /// <param name="serializedMessage">
        /// Serialized message.
        /// </param>
        /// <returns>
        /// Deserialized message.
        /// </returns>
        Message Deserialize(byte[] serializedMessage);
    }
}