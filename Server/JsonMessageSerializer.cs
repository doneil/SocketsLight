// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonMessageSerializer.cs" company="Alexey Zakharov">
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
//   Serialize and deserialize messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Witcraft.SocketsLight.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Json;

    /// <summary>
    /// Serialize and deserialize messages.
    /// </summary>
    public class JsonMessageSerializer : IMessageSerializer
    {
        /// <summary>
        /// Known types.
        /// </summary>
        private readonly IEnumerable<Type> knowTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMessageSerializer"/> class.
        /// </summary>
        /// <param name="knowTypes">
        /// Known types.
        /// </param>
        public JsonMessageSerializer(IEnumerable<Type> knowTypes)
        {
            this.knowTypes = knowTypes;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="JsonMessageSerializer"/> class from being created.
        /// </summary>
        private JsonMessageSerializer()
        {
        }

        /// <summary>
        /// Deserializer message from byte array.
        /// </summary>
        /// <param name="serializedMessage">
        /// </param>
        /// <returns>
        /// Deserialized message.
        /// </returns>
        public Message Deserialize(byte[] serializedMessage)
        {
            var memoryStream = new MemoryStream(serializedMessage) { Position = 0 };

            var dataContractSerializer = new DataContractJsonSerializer(typeof(Message), this.knowTypes);

            var result = (Message)dataContractSerializer.ReadObject(memoryStream);

            memoryStream.Close();

            return result;
        }

        /// <summary>
        /// Serializer message to byte array.
        /// </summary>
        /// <param name="message">
        /// Message to serialize
        /// </param>
        /// <returns>
        /// Serialized message.
        /// </returns>
        public byte[] Serialize(Message message)
        {
            using (var memoryStream = new MemoryStream())
            {
                var dataContractSerializer = new DataContractJsonSerializer(typeof(Message), this.knowTypes);
                dataContractSerializer.WriteObject(memoryStream, message);

                byte[] byteArray = memoryStream.ToArray();
                memoryStream.Close();

                return byteArray;
            }
        }
    }
}