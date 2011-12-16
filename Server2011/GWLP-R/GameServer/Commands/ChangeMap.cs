using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.Packets.ToLoginServer;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataBase;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.StaticConvert;

namespace GameServer.Commands
{
        [CommandAttribute(Description = "Parameter: MapID (see database). Tries to transfer to another map.")]
        class ChangeMap : IAction
        {
                private CharID newCharID;
                private MapID mapID;
                private bool validCommand;

                public ChangeMap(CharID charID, string mapID)
                {
                        newCharID = charID;
                        int tmpMapID;
                        validCommand = int.TryParse(mapID.Trim(), out tmpMapID);

                        this.mapID = new MapID((uint)(validCommand ? tmpMapID : 0));
                }

                public void Execute(DataMap oldMap)
                {
                        // failcheck
                        if (!validCommand) return;

                        // get the character
                        var chara = oldMap.Get<DataCharacter>(newCharID);

                        // check if this is valid
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                var maps = from m in db.mapsMasterData
                                          where m.mapID == mapID.Value
                                          select m;

                                // if not, send notification
                                if (maps.Count() == 0 || maps.First().gameMapID == 0)
                                {
                                        // Note: CHAT MESSAGE
                                        var chatMsg = new NetworkMessage(chara.Data.NetID)
                                        {
                                                PacketTemplate = new P081_GeneralChatMessage.PacketSt81
                                                {
                                                        Message = "Ĉć" +
                                                        "There is no such map, or the db is missing the GameMapID." +
                                                        BitConverter.ToChar(new byte[] { 0x01, 0x00 }, 0)
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(chatMsg);

                                        // Note: CHAT MESSAGE NO OWNER
                                        var chatOwner = new NetworkMessage(chara.Data.NetID)
                                        {
                                                PacketTemplate = new P082_GeneralChatNoOwner.PacketSt82
                                                {
                                                        Data1 = 0,
                                                        Data2 = (byte)ChatColors.DarkOrange_DarkOrange
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(chatOwner);

                                        // and stop the rest from being executed
                                        return;
                                }
                        }

                        var dispatchAck = new NetworkMessage(GameServerWorld.Instance.LoginSrvNetID)
                        {
                                PacketTemplate = new P65285_ClientDispatchAcknowledgement.PacketSt65285
                                {
                                        AccID = chara.Data.AccID.Value,
                                        MapID = mapID.Value,
                                        OldMapID = chara.Data.MapID.Value,
                                        IsOutpost = 1,
                                        IsPvE = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(dispatchAck);
                }
        }
}
