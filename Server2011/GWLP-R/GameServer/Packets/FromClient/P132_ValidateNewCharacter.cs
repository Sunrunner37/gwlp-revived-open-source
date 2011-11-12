using System;
using System.IO;
using System.Linq;
using GameServer.Enums;
using GameServer.Packets.ToLoginServer;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataBase;
using ServerEngine.GuildWars.DataWrappers.Chars;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using ServerEngine.PacketManagement.StaticConvert;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 132)]
        public class P132_ValidateNewCharacter : IPacket
        {
                public class PacketSt132 : IPacketTemplate
                {
                        public UInt16 Header { get { return 132; } }
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string Charname;
                        [PacketFieldType(ConstSize = true, MaxSize = 8)]
                        public byte[] Appearance;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt132>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        var pack = new P132_ValidateNewCharacter.PacketSt132();
                        pParser(pack, message.PacketData);

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;

                        chara.Data.LookSex = (byte) (pack.Appearance[0] & 1);
                        chara.Data.LookHeight = (byte) ((pack.Appearance[0] >> 1) & 0xF);
                        chara.Data.LookSkinColor = (byte) (((pack.Appearance[0] >> 5) | (pack.Appearance[1] << 3)) & 0x1F);
                        chara.Data.LookHaircolor = (byte) ((pack.Appearance[1] >> 2) & 0x1F);
                        chara.Data.LookFace = (byte) (((pack.Appearance[1] >> 7) | (pack.Appearance[2] << 1)) & 0x1F);
                        chara.Data.LookProfession = (byte) ((pack.Appearance[2] >> 4) & 0xF);
                        chara.Data.LookHairstyle = (byte) (pack.Appearance[3] & 0x1F);
                        chara.Data.LookCampaign = (byte) ((pack.Appearance[3] >> 6) & 3);
                        chara.Data.Name = new Name(pack.Charname);

                        var dispatch = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P141_DispatchConnectionTermination.PacketSt141
                                {
                                        GameMapID = (ushort) chara.Data.MapID.Value,
                                        Data1 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(dispatch);

                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                var dbCharNames = from c in db.charsMasterData
                                                        where c.charName == chara.Data.Name.Value
                                                        select c;

                                if (dbCharNames.Count() != 0)
                                {
                                        var errorMessage = new NetworkMessage(message.NetID)
                                        {
                                                PacketTemplate = new ToClient.Packet381.PacketSt381
                                                {
                                                        Data1 = 29
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(errorMessage);
                                }
                                else
                                {
                                        chara.Data.SaveToDB();

                                        #region appearance

                                        var appearance = new MemoryStream();
                                        RawConverter.WriteUInt16(6, appearance);
                                        RawConverter.WriteUInt16((ushort)(from m in db.mapsMasterData where m.gameMapID == chara.Data.MapID.Value select m).First().gameMapID, appearance);
                                        RawConverter.WriteByteAr(new byte[] { 0x33, 0x36, 0x31, 0x30, }, appearance);
                                        RawConverter.WriteByte((byte) ((chara.Data.LookSkinColor << 5) | (chara.Data.LookHeight << 1) | chara.Data.LookSex), appearance);
                                        RawConverter.WriteByte((byte) ((chara.Data.LookFace << 7) | (chara.Data.LookHaircolor << 2) | (chara.Data.LookSkinColor >> 3)), appearance);
                                        RawConverter.WriteByte((byte) ((chara.Data.LookProfession << 4) | (chara.Data.LookFace >> 1)), appearance);
                                        RawConverter.WriteByte((byte) ((chara.Data.LookCampaign << 6) | chara.Data.LookHairstyle), appearance);
                                        RawConverter.WriteByteAr(new byte[16], appearance);
                                        RawConverter.WriteByte((byte) ((chara.Data.Level << 4) | chara.Data.LookCampaign), appearance);
                                        RawConverter.WriteByte((byte)
                                                ((128) |
                                                (chara.Data.LookShowHelm == 1 ? 64 : 0) |
                                                (chara.Data.ProfessionSecondary << 2) |
                                                (chara.Data.IsPvp == 1 ? 2 : 0) |
                                                (chara.Data.Level > 15 ? 1 : 0)),
                                                appearance);
                                        RawConverter.WriteByteAr(new byte[] { 0xDD, 0xDD }, appearance);
                                        RawConverter.WriteByte((byte)0, appearance);
                                        RawConverter.WriteByteAr(new byte[] { 0xDD, 0xDD, 0xDD, 0xDD }, appearance);

                                        #endregion appearance

                                        var createChara = new NetworkMessage(message.NetID)
                                        {
                                                PacketTemplate = new ToClient.P378_CreateCharacter.PacketSt378
                                                {
                                                        StaticHash = new byte[16],
                                                        Appearance = appearance.ToArray(),
                                                        CharName = pack.Charname,
                                                        ArraySize1 = (ushort) appearance.Length,
                                                        GameMapID = (ushort) chara.Data.MapID.Value
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(createChara);

                                        var map = GameServerWorld.Instance.Get<DataMap>(new MapID(chara.Data.MapID.Value));
                                        if (map != null)
                                        {
                                                // get the char and remove it
                                                map.Remove(map.Get<DataCharacter>(new CharID(chara.Data.CharID.Value)));
                                        }

                                }
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt132> pParser;
        }
}
