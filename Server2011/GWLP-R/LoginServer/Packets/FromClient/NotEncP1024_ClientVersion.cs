using System;
using LoginServer.Enums;
using LoginServer.ServerData;
using ServerEngine.DataManagement.DataWrappers;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 1024)]
        public class NotEncP1024_ClientVersion : IPacket
        {
                public class PacketSt1024 : IPacketTemplate
                {
                        public UInt16 Header { get { return 1024; } }
                        public UInt16 Data1;
                        public UInt32 ClientVersion;
                        public UInt32 Data3;
                        public UInt32 Data4;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt1024>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt1024();
                        pParser(pack, message.PacketData);

                        // get the network stuff of the client
                        byte[] ip;
                        uint port;
                        if (NetworkManager.Instance.GetClientInfo(message.NetID, out ip, out port))
                        {
                                // if we've got a valid network-backend
                                // create the client data
                                var data = new ClientData
                                {
                                        NetID = message.NetID,
                                        IPAddress = new IPAddress(ip),
                                        Port = new Port(port),
                                        Status = SyncStatus.ConnectionEstablished,
                                };
                                var newClient = new DataClient(data);

                                // add the new client
                                if (!LoginServerWorld.Instance.Add(newClient))
                                {
                                        NetworkManager.Instance.RemoveClient(message.NetID);
                                }
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt1024> pParser;
        }
}
