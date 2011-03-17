// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRecievedEventArgs.cs" company="Alexey Zakharov">
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
//   Hold recieved message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Witcraft.SocketsLight.Client
{
    using System;

    using Witcraft.SocketsLight.Server;

    /// <summary>
    /// Hold recieved message.
    /// </summary>
    public class MessageRecievedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRecievedEventArgs"/> class.
        /// </summary>
        /// <param name="messsage">
        /// Recieved message.
        /// </param>
        public MessageRecievedEventArgs(Message messsage)
        {
            this.Message = messsage;
        }

        /// <summary>
        /// Gets recieved message.
        /// </summary>
        public Message Message { get; private set; }
    }
}