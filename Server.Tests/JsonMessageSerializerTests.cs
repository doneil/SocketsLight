// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonMessageSerializerTests.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the JsonMessageSerializerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Server.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Witcraft.SocketsLight.Server;

    using Xunit;

    /// <summary>
    /// </summary>
    public class JsonMessageSerializerTests
    {
        /// <summary>
        /// </summary>
        private IMessageSerializer messageSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMessageSerializerTests"/> class.
        /// </summary>
        public JsonMessageSerializerTests()
        {
            this.messageSerializer = new JsonMessageSerializer(new List<Type> { typeof(WeatherMessage) });
        }

        /// <summary>
        /// </summary>
        [Fact]
        public void SerializeMessage()
        {
            var weatherMessage = new WeatherMessage() { Status = Status.Cloudy, Temperature = 12 };
            byte[] serializedWeatherMessage = this.messageSerializer.Serialize(weatherMessage);
            var deserializedWeatherMessage =
                (WeatherMessage)this.messageSerializer.Deserialize(serializedWeatherMessage);
            Assert.Equal(weatherMessage.Status, deserializedWeatherMessage.Status);
            Assert.Equal(weatherMessage.Temperature, deserializedWeatherMessage.Temperature);
        }
    }

    /// <summary>
    /// </summary>
    [DataContract]
    public class WeatherMessage : Message
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public Status Status { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public byte Temperature { get; set; }
    }

    /// <summary>
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// </summary>
        Cloudy,

        /// <summary>
        /// </summary>
        Sunny,

        /// <summary>
        /// </summary>
        Rain
    }
}