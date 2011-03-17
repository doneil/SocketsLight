// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the MainPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WeatherWidget
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    using WeatherService;

    using Witcraft.SocketsLight.Client;
    using Witcraft.SocketsLight.Server;

    /// <summary>
    /// </summary>
    public partial class MainPage : UserControl
    {
        /// <summary>
        /// </summary>
        private IMessageClient messageClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.messageClient = new MessageClient(
                4530, new JsonMessageSerializer(new List<Type>() { typeof(WeatherMessage), typeof(SubscribeMessage) }));

            this.messageClient.MessageRecieved += this.messageClient_MessageRecieved;
            this.messageClient.ConnectCompleted +=
                (s, args) => { this.Dispatcher.BeginInvoke(() => { this.SubscribeButton.IsEnabled = true; }); };
            this.messageClient.ConnectAsync();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void messageClient_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            if (e.Message is WeatherMessage)
            {
                var message = (WeatherMessage)e.Message;
                this.Dispatcher.BeginInvoke(() => { this.Temperature.Text = message.Temperature.ToString(); });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Subscribe_Click(object sender, RoutedEventArgs e)
        {
            this.messageClient.SendMessageAsync(
                new SubscribeMessage() { City = ((ComboBoxItem)this.CityComboBox.SelectedItem).Content.ToString() });
        }
    }
}