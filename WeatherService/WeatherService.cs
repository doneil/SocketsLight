// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeatherService.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the WeatherService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WeatherService
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Witcraft.SocketsLight.Server;

    /// <summary>
    /// </summary>
    public class WeatherService
    {
        /// <summary>
        /// </summary>
        private IMessageServer messageServer;

        /// <summary>
        /// </summary>
        private SortedDictionary<Guid, Subscriber> subscribers = new SortedDictionary<Guid, Subscriber>();

        /// <summary>
        /// </summary>
        private Timer updateTimer;

        /// <summary>
        /// </summary>
        private Dictionary<string, WeatherInfo> weather = new Dictionary<string, WeatherInfo>()
            {
                { "Moscow", new WeatherInfo() { Temperature = 12 } },
                { "London", new WeatherInfo() { Temperature = 13 } }
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherService"/> class.
        /// </summary>
        /// <param name="messageServer">
        /// </param>
        public WeatherService(IMessageServer messageServer)
        {
            this.messageServer = messageServer;
            this.messageServer.ClientConnected += this.messageServer_ClientConnected;
            this.messageServer.ClientDisconnected += this.messageServer_ClientDisconnected;
            this.messageServer.MessageRecieved += this.messageServer_MessageRecieved;

            this.updateTimer = new Timer(this.UpdateWeather, null, 0, 5000);
        }

        /// <summary>
        /// </summary>
        /// <param name="state">
        /// </param>
        private void UpdateWeather(object state)
        {
            var random = new Random();

            foreach (var weatherData in this.weather)
            {
                weatherData.Value.Temperature = random.Next(-30, 30);
            }

            foreach (var subscriber in this.subscribers)
            {
                if (subscriber.Value.City != null)
                {
                    if (this.weather.ContainsKey(subscriber.Value.City))
                    {
                        this.messageServer.SendMessageToClient(
                            subscriber.Key,
                            new WeatherMessage() { Temperature = this.weather[subscriber.Value.City].Temperature });
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void messageServer_ClientConnected(object sender, ClientStatusChangedEventArgs e)
        {
            this.subscribers.Add(e.ClientId, new Subscriber());
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void messageServer_ClientDisconnected(object sender, ClientStatusChangedEventArgs e)
        {
            this.subscribers.Remove(e.ClientId);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void messageServer_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            if (e.Message is SubscribeMessage)
            {
                var message = (SubscribeMessage)e.Message;
                if (this.subscribers.ContainsKey(e.ClientId))
                {
                    this.subscribers[e.ClientId].City = message.City;
                }
            }
        }
    }
}