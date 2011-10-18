using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
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
                public event NetworkClientEventHandler LostClient;

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
                        lock (objLock)
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
                                                var tmpClient = new ClientConnection(new NetID(tmpNetID), newClient.Client);

                                                // add event handler
                                                tmpClient.LostConnection += RemoveClient;

                                                // add the client
                                                clients.Add(tmpNetID, tmpClient);
                                        }
                                        else
                                        {
                                                Debug.WriteLine("Client maximum reached.");
                                        }
                                }

                                // Distribute the client messages
                                // Determines how many tasks will be created as a maximum.
                                var msgCount = QueuingService.NetOutQueue.Count;
                                var taskCount = (msgCount > 10) ? 10 : msgCount;

                                Parallel.For(0, taskCount, delegate(int i)
                                {
                                        NetworkMessage netMsg;
                                        if (QueuingService.NetOutQueue.TryPeek(out netMsg))
                                        {
                                                ClientConnection client;
                                                if (clients.TryGetValue(netMsg.NetID.Value, out client))
                                                {
                                                        // Enqueue a new message from the global message queue
                                                        if (QueuingService.NetOutQueue.TryDequeue(out netMsg))
                                                        {
                                                                client.OutgoingQueue.Enqueue(netMsg);
                                                        }
                                                }
                                        }
                                });

                                // Refresh the clients
                                // when a client has lost its connection, it will automatically remove the itself
                                for (int i = 0; i < clients.Values.Count; i++)
                                {
                                        clients.Values.ElementAt(i).Refresh();
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

                                // terminate any network connection of the client
                                client.Disconnect();

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
                                // failcheck
                                if (!clients.TryGetValue(netID.Value, out client)) return false;

                                // get the client's network stats
                                clientIP = client.IP;
                                clientPort = (uint)client.Port;
                                return true;
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
                                try
                                {
                                        // Open a new connection
                                        var newClient = new TcpClient(ip, port);

                                        // Create a new ClientConnetion object, pass the tcpClient
                                        var tmpNetID = netIDs.RequestID();
                                        var tmpClient = new ClientConnection(new NetID(tmpNetID), newClient.Client);

                                        // add event handler
                                        tmpClient.LostConnection += RemoveClient;

                                        // add the client
                                        clients.Add(tmpNetID, tmpClient);

                                        return new NetID(tmpNetID);
                                }
                                catch (Exception e)
                                {
                                        Debug.Fail("The connection could not be created.", e.ToString());
                                }

                                return new NetID(-1);
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
}
