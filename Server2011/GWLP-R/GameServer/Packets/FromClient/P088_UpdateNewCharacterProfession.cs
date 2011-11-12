using System;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 88)]
        public class P088_UpdateNewCharacterProfession : IPacket
        {
                public class PacketSt88 : IPacketTemplate
                {
                        public UInt16 Header { get { return 88; } }
                        public byte Campaign; // enum pvp, prov, fc, nf
                        public byte Profession;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt88>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new P088_UpdateNewCharacterProfession.PacketSt88();
                        pParser(pack, message.PacketData);

                        // get the character
                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;

                        chara.Data.ProfessionPrimary = pack.Profession;
                        chara.Data.IsPvp = (sbyte) ((pack.Campaign == 0) ? 1 : 0);

                        switch (pack.Campaign)
                        {
                                case 0: // pvp
                                        chara.Data.MapID = new MapID(248);
                                        break;
                                case 1: // proph
                                        chara.Data.MapID = new MapID(148);
                                        break;
                                case 2: // factions
                                        chara.Data.MapID = new MapID(505);
                                        break;
                                case 3: // nightfall
                                        chara.Data.MapID = new MapID(449);
                                        break;
                        }

                        var updateProfessions = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P171_UpdatePrivProfessions.PacketSt171
                                {
                                        ID1 = 50,
                                        Prof1 = pack.Profession,
                                        Prof2 = 0,
                                        Data3 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(updateProfessions);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt88> pParser;
        }
}
