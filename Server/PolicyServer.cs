// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PolicyServer.cs" company="Alexey Zakharov">
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
//   Listen for policy file requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


// --------------------------------------------------------------------------------------------------------------------
// Dan O'Neil/Jason Cwik - 3/16/2011 - Added the following changes:
// 1) Added a couple of constructor overloads to add port and specifiy listening IP Address
// 2) PolicyServer was failing for Flash clients.  Added an extra space to the buffer to compensate for the NULL passed
//    at the ned of <policy-file-request/>
// --------------------------------------------------------------------------------------------------------------------

namespace Witcraft.SocketsLight.Server
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Diagnostics;

    /// <summary>
    /// Listen for policy file requests.
    /// </summary>
    public class PolicyServer
    {
        /// <summary>
        /// </summary>
        private const string policyRequestString = "<policy-file-request/>";

        /// <summary>
        /// </summary>
        private readonly byte[] policy;

        /// <summary>
        /// </summary>
        private bool isStopped;

        /// <summary>
        /// </summary>
        private TcpListener listener;

        /// <summary>
        /// </summary>
        private int preferredPort;

        /// <summary>
        /// </summary>
        private IPAddress ipAddr;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyServer"/> class.
        /// </summary>
        /// <param name="policyRelativeFilePath">
        /// </param>
        public PolicyServer(string policyRelativeFilePath):this(policyRelativeFilePath, 943, IPAddress.Any)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyServer"/> class.
        /// </summary>
        /// <param name="policyRelativeFilePath">
        /// </param>
        /// <param name="port">
        /// Preferred port for server to listen on
        /// </param>
        public PolicyServer(string policyRelativeFilePath, int port):this(policyRelativeFilePath, port, IPAddress.Any)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyServer"/> class.
        /// </summary>
        /// <param name="policyRelativeFilePath">
        /// </param>
        /// <param name="port">
        /// Preferred port for server to listen on
        /// </param>
        /// <param name="ipAddress">
        /// Preferred IPAddress for server to listen on
        /// </param>
        public PolicyServer(string policyRelativeFilePath, int port, IPAddress ipAddress)
        {
            preferredPort = port;
            ipAddr = ipAddress;

            //Breaking the string out allows for easier debugging
            string tmpStr = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, policyRelativeFilePath));
            //Console.WriteLine("PolicyServer - Created and using policy:  \n" + tmpStr);
            this.policy = Encoding.UTF8.GetBytes(tmpStr);
        }

        /// <summary>
        /// Start listenining for policy requests.
        /// </summary>
        public void Start()
        {
            this.listener = new TcpListener(ipAddr, preferredPort);
            this.listener.Start();

            this.listener.BeginAcceptTcpClient(this.OnAcceptTcpClient, null);
        }

        /// <summary>
        /// Stop listeninig for policy requests
        /// </summary>
        public void Stop()
        {
            this.isStopped = true;

            try
            {
                this.listener.Stop();
            }
            catch (SocketException e)
            {
                // TODO : add logger here.
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ar">
        /// </param>
        private void OnAcceptTcpClient(IAsyncResult ar)
        {
            if (this.isStopped)
            {
                return;
            }

            // Wait for the next connection.
            this.listener.BeginAcceptTcpClient(this.OnAcceptTcpClient, null);

            try
            {
                TcpClient client = this.listener.EndAcceptTcpClient(ar);
                this.SendPolicyFile(client);
            }
            catch (SocketException)
            {
                return;
            }
        }

        /// <summary>
        /// Send policy file on client request.
        /// </summary>
        /// <param name="client">
        /// </param>
        private void SendPolicyFile(TcpClient client)
        {
            Stream clientStream = client.GetStream();

            //Needed to add the +1 for Flash Policy Requests, they append a NULL to the string
            var buffer = new byte[policyRequestString.Length + 1];

            client.ReceiveTimeout = 5000;

            clientStream.Read(buffer, 0, buffer.Length);

            clientStream.Write(this.policy, 0, this.policy.Length);

            clientStream.Close();

            client.Close();
        }
    }
}