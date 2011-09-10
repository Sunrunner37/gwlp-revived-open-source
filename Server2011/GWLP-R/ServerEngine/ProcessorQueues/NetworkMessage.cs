using System;
using System.IO;
using System.Linq;
using ServerEngine.PacketManagement.Definitions;

namespace ServerEngine.ProcessorQueues
{
        public sealed class NetworkMessage
        {
                private IPacketTemplate packetTemplate;
                private MemoryStream packetData;

                /// <summary>
                ///   Initializes a new instance of the class.
                /// </summary>
                public NetworkMessage(int netID)
                {
                        NetID = netID;
                        packetData = new MemoryStream();
                }

                /// <summary>
                ///   This property contains the first two bytes of the packet.
                ///   If the packet is smaller than two bytes, this has <c>{ 0xFF, 0xFF }</c>
                /// </summary>
                public UInt16 Header
                {
                        get
                        {
                                if (packetData.Length >= 2)
                                {
                                        var tmp = packetData.Position;
                                        packetData.Seek(0, SeekOrigin.Begin);

                                        var buffer = new byte[2];
                                        packetData.Read(buffer, 0, 2);

                                        packetData.Position = tmp;
                                        return BitConverter.ToUInt16(buffer, 0);
                                }

                                return (UInt16)(packetTemplate != null ? packetTemplate.Header : 0xFFFF);
                        }
                }

                /// <summary>
                ///   This property contains the unique number
                ///   that every network client object gets after instantiation.
                /// </summary>
                public int NetID { get; set; }

                /// <summary>
                ///   This property contains the raw packet data as a byte list
                /// </summary>
                public MemoryStream PacketData
                {
                        get { return packetData; }
                        set { packetData = value; }
                }

                /// <summary>
                ///   This property contains the packet data as a packet struct of type IPacketTemplate
                /// </summary>
                public IPacketTemplate PacketTemplate
                {
                        get { return packetTemplate; }
                        set { packetTemplate = value; }
                }



        }
}