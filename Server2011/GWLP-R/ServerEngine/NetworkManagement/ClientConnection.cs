using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using ServerEngine.ProcessorQueues;

namespace ServerEngine.NetworkManagement
{
        internal sealed class ClientConnection
        {
                private readonly Queue<NetworkMessage> conOutQueue;
                private readonly NetworkStream netStream;
                private DateTime lastRefresh;
                private DateTime lastConCheck;
                private readonly TcpClient tcpConnection;
                private readonly int netID;


                /// <summary>
                ///   Initializes a new instance of the class.
                /// </summary>
                /// <param name="netID">
                ///   The unique ID of this client connection object.
                /// </param>
                /// <param name="tcpClient">
                ///   The TCP client object.
                /// </param>
                public ClientConnection(int netID, TcpClient tcpClient)
                {
                        this.netID = netID;

                        tcpConnection = tcpClient;
                        netStream = tcpClient.GetStream();

                        IsTerminated = false;

                        conOutQueue = new Queue<NetworkMessage>();
                        lastRefresh = DateTime.Now;
                        lastConCheck = DateTime.Now;
                }

                /// <summary>
                ///   This property determines whether a client connection has been terminated
                /// </summary>
                public bool IsTerminated { get; private set; }

                /// <summary>
                ///   This property determines whether a client connection has been paused.
                ///   Paused clients dont do a connection checks, but try to send data if possible.
                ///   Paused clients also do not terminate themselfes automatically. 
                /// </summary>
                public bool IsPaused { get; set; }

                /// <summary>
                ///   This property contains the network ID of the client
                /// </summary>
                public int NetID
                {
                        get { return netID; }
                }

                /// <summary>
                ///   The port of the remote client.
                /// </summary>
                public int Port
                {
                        get { return ((IPEndPoint)tcpConnection.Client.RemoteEndPoint).Port; }
                }

                /// <summary>
                ///   The ip of the remote client.
                /// </summary>
                public byte[] IP
                {
                        get { return ((IPEndPoint)tcpConnection.Client.RemoteEndPoint).Address.GetAddressBytes(); }
                }

                /// <summary>
                ///   This property contains all messages that have to be send.
                /// </summary>
                public Queue<NetworkMessage> ConOutQueue
                {
                        get { return conOutQueue; }
                }

                /// <summary>
                ///   This refreshes the network connection, and writes/reads
                ///   packets if necessary.
                ///   This must be called by the <c>ClientManager</c> object
                /// </summary>
                /// <returns>
                ///   Returns false if the client connection has been terminated.
                /// </returns>
                public bool Refresh()
                {
                        // If is connected
                        if (IsTerminated) return IsTerminated;

                        // Check for connection
                        if(DateTime.Now.Subtract(lastConCheck).TotalMilliseconds > 10 && !IsTerminated && !IsConnected())
                        {
                                Terminate();
                        }
                        lastConCheck = DateTime.Now;

                        // read data if possible
                        if (netStream.DataAvailable)
                        {
                                var tmpPck = ReadPacket();

                                // failcheck
                                if (tmpPck != null)
                                {
                                        QueuingService.NetInQueue.Enqueue(tmpPck);
                                }
                        }

                        // recalculate the timespan between now and the last written packet:
                        int timeSpan = (int) DateTime.Now.Subtract(lastRefresh).TotalMilliseconds;

                        // if theres a packet in the private queue and the time span is high enough
                        if ((timeSpan > 10) && (conOutQueue.Count != 0))
                        {
                                lastRefresh = DateTime.Now;

                                // send this packet
                                NetworkMessage tmpMsg = conOutQueue.Dequeue();
                                WritePacket(tmpMsg);
                        }

                        return !IsTerminated;
                }

                /// <summary>
                ///   This terminates the network connections.
                /// </summary>
                public void Terminate()
                {
                        if (IsPaused) Debug.WriteLine("Terminating paused (!) client...", netID);

                        IsTerminated = true;

                        netStream.Close();
                        tcpConnection.Close();

                        Debug.WriteLine("Lost connection to Client[{0}].", netID);
                }

                /// <summary>
                ///   Recieves a packet from the current network stream. (helps by automatic decryption if necessary)
                /// </summary>
                /// <returns>
                ///   Returns the message if there were no errors,
                ///   else returns an invalid message with <c>InternalMessage(0, 0, new List of byte'())</c>.
                /// </returns>
                private NetworkMessage ReadPacket()
                {
                        // pause check
                        if (IsPaused) return null;

                        var result = new List<byte>();

                        while (netStream.DataAvailable)
                        {
                                result.Add((byte)netStream.ReadByte());
                        }

                        Debug.WriteLine("-->"+BitConverter.ToString(result.ToArray()).Replace("-", " "));

                        return new NetworkMessage(netID) {PacketData = new MemoryStream(result.ToArray())};
                }

                /// <summary>
                ///   Sends a packet via the current network stream. (helps by automatic encryption if necessary)
                /// </summary>
                /// <param name="netMessage">
                ///   The message that has to be send
                /// </param>
                private void WritePacket(NetworkMessage netMessage)
                {
                        //// pause check
                        //if (IsPaused) return;

                        try
                        {
                                tcpConnection.Client.Send(netMessage.PacketData.ToArray());//, 0, (int)netMessage.PacketData.Length);

                                if (netMessage.Header != 19)
                                        Debug.WriteLine(BitConverter.ToString(netMessage.PacketData.ToArray()).Replace("-", " "));
                        }
                        catch (Exception)
                        {
                                
                                Debug.WriteLine("Failed to send data to Client[{0}]", netID);
                                if (!IsPaused) Terminate();
                        }
                        
                }

                /// <summary>
                ///   Checks whether the connection is still active.
                /// </summary>
                /// <returns>
                ///   Returns true if it the connection is still active.
                /// </returns>
                private bool IsConnected()
                {
                        // pause check
                        if (IsPaused) return true;

                        try
                        {
                                return !(tcpConnection.Client.Poll(1, SelectMode.SelectRead) && tcpConnection.Client.Available == 0);
                        }
                        catch (SocketException)
                        {
                                Debug.WriteLine("Failed to send connection-check data to Client[{0}]", netID);
                                return false;
                        }
                }
        }
}
