// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeatherMessage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the WeatherMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WeatherService
{
    using System.Runtime.Serialization;

    using Witcraft.SocketsLight.Server;

    /// <summary>
    /// </summary>
    [DataContract]
    public class WeatherMessage : Message
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public int Temperature { get; set; }
    }
}