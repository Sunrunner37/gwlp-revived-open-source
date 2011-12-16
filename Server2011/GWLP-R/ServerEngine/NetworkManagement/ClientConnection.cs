using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using ServerEngine.DataManagement.DataWrappers;

namespace ServerEngine.NetworkManagement
{
        internal sealed class ClientConnection
        {
                private readonly object objLock = new object();

                private const int RecieveTimeout = 1000;
                private const int SendTimeout = 1000;

                private readonly ConcurrentQueue<NetworkMessage> outgoingQueue;
                private DateTime lastWrittenPacket;
                private DateTime lastConCheck;
                private readonly Socket socket;
                private readonly NetID netID;

                /// <summary>
                ///   Initializes a new instance of the class.
                /// </summary>
                /// <param name="netID">
                ///   The unique ID of this client connection object.
                /// </param>
                /// <param name="socket">
                ///   The TCP network socket.
                /// </param>
                public ClientConnection(NetID netID, Socket socket)
                {
                        this.netID = netID;

                        this.socket = socket;
                        this.socket.ReceiveTimeout = RecieveTimeout;
                        this.socket.SendTimeout = SendTimeout;

                        outgoingQueue = new ConcurrentQueue<NetworkMessage>();
                        lastWrittenPacket = DateTime.Now;
                        lastConCheck = DateTime.Now;
                }

                ~ClientConnection()
                {
                        socket.Dispose();
                }

                /// <summary>
                ///   This event is triggered when the connection to the client is lost
                /// </summary>
                public event NetworkClientEventHandler LostConnection;

                /// <summary>
                ///   This property contains the network ID of the client
                /// </summary>
                public NetID NetID
                {
                        get { return netID; }
                }

                /// <summary>
                ///   The port of the remote client.
                /// </summary>
                public int Port
                {
                        get { return ((IPEndPoint)socket.RemoteEndPoint).Port; }
                }

                /// <summary>
                ///   The ip of the remote client.
                /// </summary>
                public byte[] IP
                {
                        get { return ((IPEndPoint)socket.RemoteEndPoint).Address.GetAddressBytes(); }
                }

                /// <summary>
                ///   This property contains all messages that have to be send.
                /// </summary>
                public ConcurrentQueue<NetworkMessage> OutgoingQueue
                {
                        get { return outgoingQueue; }
                }

                /// <summary>
                ///   Writes/reads packets if necessary.
                /// </summary>
                public void Refresh()
                {
                        lock (objLock)
                        {

                                // connection check
                                if (!IsConnected())
                                {
                                        OnConnectionLost();
                                        return;
                                }

                                // read data if possible
                                if (socket.Available > 0)
                                {
                                        NetworkMessage tmpPck;

                                        if (ReadPacket(out tmpPck))
                                        {
                                                // if everything went correctly, enqueue the new packet
                                                QueuingService.NetInQueue.Enqueue(tmpPck);
                                        }
                                }

                                // write data if there is any and the time is ok
                                if (DateTime.Now.Subtract(lastWrittenPacket).TotalMilliseconds > 5 &&
                                    outgoingQueue.Count != 0)
                                {
                                        // send this packet
                                        NetworkMessage tmpMsg;
                                        if (outgoingQueue.TryDequeue(out tmpMsg))
                                        {
                                                if (WritePacket(tmpMsg))
                                                {
                                                        lastWrittenPacket = DateTime.Now;
                                                }
                                        }
                                }
                        }
                }

                /// <summary>
                ///   This terminates the network connections.
                /// </summary>
                public void Disconnect()
                {

                        socket.Disconnect(false);
                }

                private bool ReadPacket(out NetworkMessage netMessage)
                {
                        // create a new network message
                        netMessage = new NetworkMessage(netID);
                        var tmpData = new List<byte>();

                        // connection check
                        if (!socket.Connected) return false;
                        
                        // get the data if any is available
                        while (socket.Available > 0)
                        {
                                var buffer = new byte[socket.Available];

                                try
                                {
                                        // read data from the socket
                                        socket.Receive(buffer);

                                        // save it to the temporary byte list
                                        tmpData.AddRange(buffer);

                                        // debug output
                                        Debug.WriteLine("-->" + BitConverter.ToString(tmpData.ToArray()).Replace("-", " "));
                                }
                                catch (SocketException e)
                                {

                                        if (e.SocketErrorCode == SocketError.WouldBlock ||
                                        e.SocketErrorCode == SocketError.IOPending ||
                                        e.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                                        {
                                                // socket buffer is probably full, wait and try again
                                                Thread.Sleep(30);
                                        }
                                        else
                                                return false;
                                }
                        }

                        // everything finished, return true
                        netMessage.PacketData = new MemoryStream(tmpData.ToArray());
                        return true;
                }

                private bool WritePacket(NetworkMessage netMessage)
                {
                        // failcheck
                        if (netMessage == null) return false;

                        // connection check
                        if (!socket.Connected) return false;

                        // try sending the raw packet data
                        try
                        {
                                // write the data
                                socket.Send(netMessage.PacketData.ToArray());

                                // debug output
                                if (netMessage.Header != 19)
                                        Debug.WriteLine(BitConverter.ToString(netMessage.PacketData.ToArray()).Replace("-", " "));
                        }
                        catch (SocketException e)
                        {

                                if (e.SocketErrorCode == SocketError.WouldBlock ||
                                e.SocketErrorCode == SocketError.IOPending ||
                                e.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                                {
                                        // socket buffer is probably full, wait and try again
                                        Thread.Sleep(30);
                                }
                                else
                                        return false;  // any serious error occurr
                        }

                        return true;
                }

                /// <summary>
                ///   Checks whether the connection is still active.
                /// </summary>
                /// <returns>
                ///   Returns true if it the connection is still active.
                /// </returns>
                private bool IsConnected()
                {
                        if (!socket.Connected)
                        {
                                return false;
                        }

                        try
                        {
                                if (socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0)
                                {
                                        // if theres no connection anymore
                                        // disconnect the socket
                                        socket.Disconnect(false);
                                        return false;
                                }

                                return true;
                        }
                        catch (SocketException)
                        {
                                Debug.WriteLine("Failed to check connection of Client[{0}]", netID.Value);
                                return false;
                        }
                }

                /// <summary>
                ///   This method triggers the LostClient event
                /// </summary>
                private void OnConnectionLost()
                {
                        if (LostConnection != null)
                        {
                                // disconnect the socket (if it isnt already disconnected)
                                socket.Disconnect(false);

                                // debug output
                                Debug.WriteLine("Lost connection to Client[{0}].", netID.Value);

                                // trigger event
                                LostConnection(netID);
                        }
                }
        }
}
