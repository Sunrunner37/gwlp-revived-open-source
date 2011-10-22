using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToLoginServer;
using GameServer.ServerData;
using ServerEngine;
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
