// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Global type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Web
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Web;

    using WeatherService;

    using Witcraft.SocketsLight.Server;

    /// <summary>
    /// </summary>
    public class Global : HttpApplication
    {
        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void Application_End(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void Application_Error(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void Application_Start(object sender, EventArgs e)
        {
            var policyServer = new PolicyServer("clientaccesspolicy.xml");
            IMessageServer messageServer = new MessageServer(
                IPAddress.Any,
                4530,
                new JsonMessageSerializer(new List<Type>() { typeof(WeatherMessage), typeof(SubscribeMessage) }));

            ThreadPool.QueueUserWorkItem((o) => { policyServer.Start(); });
            ThreadPool.QueueUserWorkItem((o) => { messageServer.Start(); });

            var weatherService = new WeatherService(messageServer);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void Session_End(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void Session_Start(object sender, EventArgs e)
        {
        }
    }
}