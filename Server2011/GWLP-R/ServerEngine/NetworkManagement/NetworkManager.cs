using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using ServerEngine.DataManagement.DataWrappers;
using IPAddress = System.Net.IPAddress;

namespace ServerEngine.NetworkManagement
{
        public sealed class NetworkManager
        {
                private readonly object objLock = new object();

                private readonly Dictionary<int, ClientConnection> clients;
                private readonly IDManager netIDs;
                private TcpListener tcpListener;
                private int maxClients = 100;

                /// <summary>
                ///   Singleton instance
                /// </summary>
                private static readonly NetworkManager instance = new NetworkManager();

                /// <summary>
                ///   Creates a new instance of the class
                /// </summary>
                private NetworkManager()
                {
                        // Create the client lists
                        clients = new Dictionary<int, ClientConnection>();

                        // Adjust the NetID manager
                        netIDs = new IDManager(10, 10000);
                }

                /// <summary>
                ///   This property contains the singleton instance of the class
                /// </summary>
                public static NetworkManager Instance
                {
                        get { return instance; }
                }

                ~NetworkManager()
                {
                        tcpListener.Stop();
                }

                /// <summary>
                ///   This property contains the maximum amount of client that this NetMan will accept
                /// </summary>
                public int MaximumClients { set { maxClients = value; } }

                /// <summary>
                ///   This event is triggered whenever a client connection has terminated
                /// </summary>
                public event NetManEventHandler LostClient;

                /// <summary>
                ///   Starts the TCP listeners on the given ports
                /// </summary>
                /// <param name="portGame" />
                public void StartListeners(int portGame)
                {
                        // Start the tcp network interface
                        tcpListener = new TcpListener(IPAddress.Any, portGame);
                        tcpListener.Start();
                }

                /// <summary>
                ///   The network manager main task.
                /// </summary>
                public void MainTask()
                {
                        // Check for incoming connections
                        if (tcpListener.Pending())
                        {
                                // check utilization
                                if (clients.Count <= maxClients)
                                {
                                        // Accept the client
                                        var newClient = tcpListener.AcceptTcpClient();

                                        // Create a new ClientConnetion object, pass the tcpClient
                                        var tmpNetID = netIDs.RequestID();
                                        clients.Add(tmpNetID, new ClientConnection(tmpNetID, newClient));

                                        // note that we dont need an AddClient event, as the client will be added when the new packet arrives
                                }
                                else
                                {
                                        Debug.WriteLine("Client maximum reached.");
                                }
                        }

                        // Distribute the client messages
                        NetworkMessage netMsg;
                        if (QueuingService.NetOutQueue.TryPeek(out netMsg))
                        {
                                ClientConnection client;
                                if (clients.TryGetValue(netMsg.NetID.Value, out client))
                                {
                                        // Enqueue a new message from the global message queue
                                        if (!client.IsTerminated && QueuingService.NetOutQueue.TryDequeue(out netMsg))
                                        {
                                                client.ConOutQueue.Enqueue(netMsg);
                                        }
                                }
                        }

                        // Refresh the clients / Remove them if they have terminated their connection
                        var terminatedClients = clients.Values.Where(cl => !cl.Refresh()).ToList();

                        foreach (var tcl in terminatedClients)
                        {
                                lock (objLock)
                                {
                                        // the following will do every thing for us
                                        // (termination check, remove, free id, event trigger etc.)
                                        clients.Remove(tcl.NetID);
                                }
                        }
                }

                /// <summary>
                ///   Remove a client fromt the client list.
                /// </summary>
                /// <param name="netID">
                ///   The unique network interface ID of the client
                /// </param>
                public void RemoveClient(NetID netID)
                {
                        lock (objLock)
                        {
                                ClientConnection client;
                                if (!clients.TryGetValue(netID.Value, out client)) return;

                                // Check client for termination first
                                if (!client.IsTerminated)
                                {
                                        client.Terminate();
                                }

                                // Remove it
                                clients.Remove(netID.Value);
                                // Free netID
                                netIDs.FreeID(netID.Value);

                                // trigger event, cause client terminated
                                OnClientLost(netID);
                        }

                }

                /// <summary>
                ///   Reply to the requested utilization ratio packet
                /// </summary>
                public int GetUtilization()
                {
                        lock (objLock)
                        {
                                var ratio = (int) Math.Round(clients.Count/(float) maxClients);

                                return ratio;
                        }
                }

                /// <summary>
                ///   Reply to the requested utilization ratio packet
                /// </summary>
                public bool GetClientInfo(NetID netID, out byte[] clientIP, out uint clientPort)
                {
                        lock (objLock)
                        {
                                clientIP = new byte[4];
                                clientPort = 0;

                                ClientConnection client;
                                if (!clients.TryGetValue(netID.Value, out client)) return false;
                                if (!client.IsTerminated)
                                {
                                        clientIP = client.IP;
                                        clientPort = (uint)client.Port;
                                        return true;
                                }

                                // remove if terminated
                                RemoveClient(netID);

                                return false;
                        }
                }

                /// <summary>
                ///   Tries to create a new login server connection
                /// </summary>
                /// <returns>
                ///   Returns the netID of the login server if possible
                /// </returns>
                public NetID CreateConnection(string ip, int port)
                {
                        lock (objLock)
                        {
                                var netID = -1;

                                try
                                {
                                        // Open a new connection
                                        var newClient = new TcpClient(ip, port);

                                        // Create a new client and add it to the server connections list
                                        netID = netIDs.RequestID();
                                        clients.Add(netID, new ClientConnection(netID, newClient));
                                }
                                catch (Exception e)
                                {
                                        Debug.Fail("The connection could not be created.", e.ToString());
                                }

                                return new NetID(netID);
                        }
                }

                /// <summary>
                ///   This method triggers the LostClient event
                /// </summary>
                private void OnClientLost(NetID netID)
                {
                        if (LostClient != null)
                        {
                                LostClient(netID);
                        }
                }
        }

        /// <summary>
        ///   This delegate represents handler methods for network manager events
        /// </summary>
        /// <param name="netID">
        ///   This is to identify the client that triggered the event
        /// </param>
        public delegate void NetManEventHandler(NetID netID);
}
