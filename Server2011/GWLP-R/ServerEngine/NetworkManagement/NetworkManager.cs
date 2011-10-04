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
                private int maxClients;
                private bool isInitialized;

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

                        // Set this to 'not initialized'
                        isInitialized = false;
                }

                /// <summary>
                ///   This property contains the singleton instance of the class
                /// </summary>
                public static NetworkManager Instance
                {
                        get { return instance; }
                }

                /// <summary>
                ///   Init the object
                /// </summary>
                /// <param name="maxClients">
                ///   The maximum client count
                /// </param>
                public void Init(int maxClients)
                {
                        this.maxClients = maxClients;
                        isInitialized = true;
                }

                ~NetworkManager()
                {
                        tcpListener.Stop();
                }

                /// <summary>
                ///   Starts the TCP listeners on the given ports
                /// </summary>
                /// <param name="portGame" />
                public void StartListeners(int portGame)
                {
                        if (!isInitialized) throw new Exception("Not initialized. Call Init() first."); 

                        // Start the tcp network interface
                        tcpListener = new TcpListener(IPAddress.Any, portGame);
                        tcpListener.Start();
                }

                /// <summary>
                ///   The network manager main task.
                /// </summary>
                public void MainTask()
                {
                        if (!isInitialized) throw new Exception("Not initialized. Call Init() first."); 

                        // Check for incoming connections
                        if (tcpListener.Pending())
                        {
                                if (clients.Count > maxClients)
                                {
                                        throw new Exception("Client maximum reached.");
                                }

                                // Accept the client
                                var newClient = tcpListener.AcceptTcpClient();

                                // Create a new ClientConnetion object, pass the tcpClient
                                var tmpNetID = netIDs.RequestID();
                                clients.Add(tmpNetID, new ClientConnection(tmpNetID, newClient) {IsPaused = false});
                        }

                        // Distribute the client messages
                        NetworkMessage netMsg;
                        if (QueuingService.NetOutQueue.TryPeek(out netMsg))
                        {
                                ClientConnection client;
                                if (clients.TryGetValue(netMsg.NetID.Value, out client))
                                {
                                        // Enqueue a new message from the global message queue
                                        if (QueuingService.NetOutQueue.TryDequeue(out netMsg))
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
                                        // If refresh failed, the client has terminated (check that)
                                        if (!tcl.IsTerminated)
                                        {
                                                tcl.Terminate();
                                        }

                                        // Remove it
                                        clients.Remove(tcl.NetID);
                                        // Free netID
                                        netIDs.FreeID(tcl.NetID);
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
                        if (!isInitialized) throw new Exception("Not initialized. Call Init() first."); 

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
                        }

                }

                /// <summary>
                ///   Pauses a client. It cannot send/recieve anything, but it cannot be terminated automatically too.
                ///   Returns true if successfully paused, otherwise the client may have terminated.
                /// </summary>
                /// <param name="netID">
                ///   The network ID of the client.
                /// </param>
                public bool PauseClient(NetID netID)
                {
                        if (!isInitialized) throw new Exception("Not initialized. Call Init() first.");

                        lock (objLock)
                        {
                                ClientConnection client;
                                if (!clients.TryGetValue(netID.Value, out client)) return false;

                                // Check client for termination first
                                if (!client.IsTerminated)
                                {
                                        client.IsPaused = true;
                                        return true;
                                }
                        }

                        return false;
                }

                /// <summary>
                ///   Reply to the requested utilization ratio packet
                /// </summary>
                public int GetUtilization()
                {
                        if (!isInitialized) throw new Exception("Not initialized. Call Init() first."); 

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
                        if (!isInitialized) throw new Exception("Not initialized. Call Init() first."); 

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

                                return false;
                        }
                }

                /// <summary>
                ///   Tries to create a new login server connection
                /// </summary>
                /// <returns>
                ///   Returns the netID of the login server if possible
                /// </returns>
                public int CreateConnection(string ip, int port)
                {
                        if (!isInitialized) throw new Exception("Not initialized. Call Init() first."); 

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
                                        Debug.Fail("No login server connection could be created.", e.ToString());
                                }

                                return netID;
                        }
                }
        }
}
