using System;
using System.Linq;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataBase;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 7)]
        public class P07_DeleteCharacter : IPacket
        {
                public class PacketSt7 : IPacketTemplate
                {
                        public UInt16 Header { get { return 7; } }
                        public UInt32 CharacterID;//not really sure bout this
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string CharacterName;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt7>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        var pack = new P07_DeleteCharacter.PacketSt7();
                        pParser(pack, message.PacketData);

                        using (var db = (MySQL) DataBaseProvider.GetDataBase())
                        {
                                var dbCharNames = from c in db.charsMasterData
                                                  where c.charName == pack.CharacterName
                                                  select c;

                                if (dbCharNames.Count() != 0)
                                {
                                        var dbChar = dbCharNames.First();

                                        db.charsMasterData.DeleteOnSubmit(dbChar);
                                        db.SubmitChanges();
                                }

                        }

                        var client = LoginServerWorld.Instance.Get<DataClient>(message.NetID);
                        client.Data.SyncCount++;

                        // Note: STREAM TERMINATOR
                        var msg = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P03_StreamTerminator.PacketSt3
                                {
                                        LoginCount = client.Data.SyncCount,
                                        ErrorCode = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(msg);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt7> pParser;
        }
}
