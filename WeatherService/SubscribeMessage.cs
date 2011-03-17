// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubscribeMessage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the SubscribeMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WeatherService
{
    using System.Runtime.Serialization;

    using Witcraft.SocketsLight.Server;

    /// <summary>
    /// </summary>
    [DataContract]
    public class SubscribeMessage : Message
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public string City { get; set; }
    }
}