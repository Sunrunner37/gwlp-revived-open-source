using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;
using ServerEngine.GuildWars.Tools;

namespace GameServer.Commands
{
    [CommandAttribute(Description = "No Parameters. Does stuff for testing purpose.")]
    class Test : IAction
    {
        private readonly CharID newCharID;

        public Test(CharID charID)
        {
            newCharID = charID;
        }

        public void Execute(DataMap map)
        {
            var chara = map.Get<DataCharacter>(newCharID);

            var testSkill = new NetworkMessage(chara.Data.NetID)
            {
                PacketTemplate = new P216_SkillActivate.PacketSt216
                {
                    CasterAgentID = chara.Data.AgentID.Value,
                    SkillID = 0x615,
                    Data2 = 0
                }
            };
            QueuingService.PostProcessingQueue.Enqueue(testSkill);

            testSkill = new NetworkMessage(chara.Data.NetID)
            {
                PacketTemplate = new P147_UpdateGenericValueInt.PacketSt147
                {
                    ValueID = (uint)GenericValues.CastSpellAnimated,
                    AgentID = chara.Data.AgentID.Value,
                    Value = 0x615
                }
            };
            QueuingService.PostProcessingQueue.Enqueue(testSkill);
       } 
    }
}
