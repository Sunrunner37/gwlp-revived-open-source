using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ServerEngine.ProcessorQueues;
using ServerEngine.Tools;

namespace ServerEngine.NetworkManagement
{
        public sealed class NetworkManager
        {
                public int MaxClients { get; set; }

                private readonly Dictionary<int, ClientConnection> clients;
                private readonly IDManager netIDs = new IDManager(10);
                private TcpListener tcpListener;

                private static readonly NetworkManager instance = new NetworkManager(); // singleton instance

                private NetworkManager()
                {
                        // Create the client lists
                        clients = new Dictionary<int, ClientConnection>();
                }

                public static NetworkManager Instance
                {
                        get { return instance; }
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
                        // Start the tcp network interface
                        tcpListener = new TcpListener(IPAddress.Any, portGame);
                        tcpListener.Start();
                }

                /// <summary>
                ///   The networkman main task.
                /// </summary>
                public void MainTask()
                {
                        // Check for incoming connections
                        if (tcpListener.Pending() &&
                                (clients.Count <= (MaxClients)))
                        {
                                // Accept the client
                                TcpClient newClient = tcpListener.AcceptTcpClient();

                                // Create a new ClientConnetion object, pass the tcpClient
                                int tmpNetID = netIDs.NewID();
                                clients.Add(tmpNetID, new ClientConnection(tmpNetID, newClient));

                                //Debug.WriteLine("New connection");
                        }

                        // Refresh the clients / Remove them if they have terminated their connection
                        NetworkMessage netMsg;
                        if (QueuingService.NetOutQueue.TryPeek(out netMsg))
                        {
                                ClientConnection client;
                                if (clients.TryGetValue(netMsg.NetID, out client))
                                {
                                        // Enqueue a new message from the global message queue
                                        if (QueuingService.NetOutQueue.TryDequeue(out netMsg))
                                        {
                                                client.ConOutQueue.Enqueue(netMsg);
                                        }
                                }
                        }

                        lock (clients)
                        {
                                foreach (var cl in clients.Values)
                                {
                                        if (!cl.Refresh())
                                        {
                                                // The client has terminated, so remove it.
#warning DEBUG
                                                //RemoveClient(cl.NetID);
                                        }
                                }
                        }
                }

                /// <summary>
                ///   Remove a client fromt the client list.
                /// </summary>
                /// <param name="netID">
                ///   The unique network interface ID of the client
                /// </param>
                public void RemoveClient(int netID)
                {
                        // Check client first for termination
                        lock (clients)
                        {
                                ClientConnection client;
                                if (!clients.TryGetValue(netID, out client)) return;
                                if (!client.IsTerminated)
                                {
                                        client.Terminate();
                                }

                                // Remove it
                                clients.Remove(netID);
                                // Free netID
                                netIDs.FreeID(netID);
                        }

                }

                /// <summary>
                ///   Reply to the requested utilization ratio packet
                /// </summary>
                public int GetUtilization()
                {
                        var ratio = (int)Math.Round(clients.Count / (float)MaxClients);

                        return ratio;
                }

                /// <summary>
                ///   Reply to the requested utilization ratio packet
                /// </summary>
                public bool GetClientInfo(int netID, out byte[] clientIP, out int clientPort)
                {
                        clientIP = new byte[4];
                        clientPort = 0;

                        ClientConnection client;
                        if (!clients.TryGetValue(netID, out client)) return false;
                        if (!client.IsTerminated)
                        {
                                clientIP = client.IP;
                                clientPort = client.Port;
                                return true;
                        }

                        return false;
                }

                /// <summary>
                ///   Tries to create a new login server connection
                /// </summary>
                /// <returns>
                ///   Returns the netID of the login server if possible
                /// </returns>
                public int CreateConnection(string ip, int port)
                {
                        var netID = -1;

                        try
                        {
                                // Open a new connection
                                var newClient = new TcpClient(ip, port);

                                // Create a new client and add it to the server connections list
                                netID = netIDs.NewID();
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
