using System;
using System.IO;
using System.Linq;
using LoginServer.DataBase;
using LoginServer.Enums;
using LoginServer.Packets.ToClient;
using LoginServer.Packets.ToGameServer;
using LoginServer.ServerData;
using ServerEngine;
using ServerEngine.DataManagement;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using ServerEngine.PacketManagement.StaticConvert;

namespace LoginServer.Packets.FromGameServer
{
        [PacketAttributes(IsIncoming = true, Header = 65285)]
        public class P65285_ClientDispatchAcknowledgement : IPacket
        {
                public class PacketSt65285 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65285; } }
                        public UInt32 AccID;
                        public UInt32 MapID;
                        public UInt32 OldMapID;
                        public byte IsOutpost;
                        public byte IsPvE;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65285>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt65285();
                        pParser(pack, message.PacketData);

                        // get client and check for a suitable game server
                        var client = LoginServerWorld.Instance.Get<DataClient>(new AccID(pack.AccID));

                        // set the mapid
                        var mapID = new MapID(pack.MapID);
                        client.Data.MapID = mapID;

                        // get the most suitable game server
                        DataGameServer server;
                        if (!LoginServerWorld.Instance.GetBestGameServer(mapID, out server) && server != null)
                        {
                                // if we've got a server but no map, build one.
                                // Note: BUILD MAP REQUEST
                                var buildMap = new NetworkMessage(server.Data.NetID)
                                {
                                        PacketTemplate = new P65283_BuildMapRequest.PacketSt65283
                                        {
                                                MapID = mapID.Value,
                                                IsOutpost = pack.IsOutpost,
                                                IsPvE = pack.IsPvE
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(buildMap);

                        }
                        else if (server == null)
                        {
                                // no game server found,
                                // Note: STREAM TERMINATOR
                                var msg = new NetworkMessage(message.NetID)
                                {
                                        PacketTemplate = new P03_StreamTerminator.PacketSt3
                                        {
                                                LoginCount = client.Data.SyncCount,
                                                ErrorCode = 5,
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(msg);

                                return true;
                        }

                        // update the client's status
                        client.Data.Status = SyncStatus.TriesToLoadInstance;

                        // if we've got a server and a map, let the game server accept a new player
                        // Note: CLIENT DISPATCH FORWARD
                        var dispatchForward = new NetworkMessage(server.Data.NetID)
                        {
                                PacketTemplate = new P65285_ClientDispatchForward.PacketSt65285
                                {
                                        AccID = client.Data.AccID.Value,
                                        CharID = client.Data.CharID.Value,
                                        MapID = mapID.Value,
                                        Key1 = client.Data.SecurityKeys[0],
                                        Key2 = client.Data.SecurityKeys[1],
                                        IsOutpost = pack.IsOutpost,
                                        IsPvE = pack.IsPvE
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(dispatchForward);

                        // also, let the old game server know the connection stuff of the new game server.
                        // create server connection array
                        var con = new MemoryStream();
                        RawConverter.WriteUInt16(2, con);
                        // the following is the port in big endian
                        RawConverter.WriteByteAr(BitConverter.GetBytes((ushort)server.Data.Port.Value).Reverse().ToArray(), con);
                        RawConverter.WriteByteAr(server.Data.IPAddress.Value, con);
                        RawConverter.WriteByteAr(new byte[16], con);

                        var dispUpdt = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P65286_ClientDispatchConnectionUpdate.PacketSt65286
                                {
                                        AccID = client.Data.AccID.Value,
                                        CharID = client.Data.CharID.Value,
                                        MapID = client.Data.MapID.Value,
                                        OldMapID = pack.OldMapID,
                                        ConnectionInfo = con.ToArray(),
                                        Key1 = client.Data.SecurityKeys[0],
                                        Key2 = client.Data.SecurityKeys[1],
                                        IsOutpost = pack.IsOutpost,
                                        IsPvE = pack.IsPvE
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(dispUpdt);

                        // finally, update the client's database values
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                var chars = from c in db.charsMasterData
                                            where c.charID == client.Data.CharID.Value
                                            select c;

                                // when the check fails, we still return true because the packet-manager expects the packet to be incomplete otherwise.
                                if (chars.Count() == 0) return true;

                                var chara = chars.First();

                                chara.mapID = (int)mapID.Value;
                                db.SubmitChanges();
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65285> pParser;
        }
}
