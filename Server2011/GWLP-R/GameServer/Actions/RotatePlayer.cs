using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;

namespace GameServer.Actions
{
        public class RotatePlayer : IAction
        {
                private int newCharID;

                public RotatePlayer(int charID)
                {
                        newCharID = charID;
                }

                public void Execute(Map map)
                {
                        // send message to all available players
                        foreach (var charID in map.CharIDs)
                        {
                                CreatePackets(newCharID, charID);
                        }
                }

                private static void CreatePackets(int charID, int recipientCharID)
                {
                        var chara = World.GetCharacter(Chars.CharID, charID);
                        
                        // get the recipient of all those packets
                        int reNetID = 0;
                        if (recipientCharID != charID)
                        {
                                reNetID = (int)World.GetCharacter(Chars.CharID, recipientCharID)[Chars.NetID];
                        }
                        else
                        {
                                reNetID = (int)chara[Chars.NetID];
                        }

                        // Note: ROTATE AGENT
                        var rotAgent = new NetworkMessage(reNetID);
                        rotAgent.PacketTemplate = new P035_RotateAgent.PacketSt35()
                        {
                                AgentID = (ushort)(int)chara[Chars.AgentID],
                                Rotation = chara.CharStats.Rotation,
                                Data1 = 0x40060A92
                        };
                        QueuingService.PostProcessingQueue.Enqueue(rotAgent);
                        
                }
        }
}
