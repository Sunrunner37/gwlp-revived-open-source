using System;
using LoginServer.ServerData;
using LoginServer.ServerData.DataInterfaces;
using ServerEngine.NetworkManagement;
using ServerEngine.DataManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromGameServer
{
        [PacketAttributes(IsIncoming = true, Header = 65282)]
        public class P65282_ServerStatsReply : IPacket
        {
                public class PacketSt65282 : IPacketTemplate, IHasGameServerData
                {
                        public UInt16 Header { get { return 65282; } }
                        public byte SrvUtilization;
                        public UInt16 ArraySize1;
                        [PacketFieldType(ConstSize = false, MaxSize = 1024)]
                        public UInt16[] MapIDs;

                        #region Implementation of IHasGameServerData

                        public byte Utilization 
                        { 
                                get { return SrvUtilization; }
                                set { SrvUtilization = value; }
                        }

                        public ushort[] AvailableMaps
                        {
                                get { return MapIDs; }
                                set { MapIDs = value; }
                        }

                        #endregion
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65282>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt65282();
                        pParser(pack, message.PacketData);

                        // get the game server
                        var dataGameServer = LoginServerWorld.Instance.Get<DataGameServer>(message.NetID);

                        // paste the data
                        dataGameServer.Data.Paste<IHasGameServerData>(pack);
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65282> pParser;
        }
}
