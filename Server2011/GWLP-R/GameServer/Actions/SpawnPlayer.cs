using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine.ProcessorQueues;

namespace GameServer.Actions
{
        public class SpawnPlayer : IAction
        {
                private int newCharID;

                public SpawnPlayer(int charID)
                {
                        newCharID = charID;
                }

                public void Execute(Map map)
                {
                        // spawn this player for himself
                        CreateSpawnPacketsFor(newCharID, newCharID);

                        var chara = World.GetCharacter(Chars.CharID, newCharID);

                        // Note: FADE INTO MAP
                        var fadeIntoMap = new NetworkMessage((int)chara[Chars.NetID]);
                        fadeIntoMap.PacketTemplate = new P023_InstanceLoadFadeIntoMap.PacketSt23()
                        {
                                AgentID = (ushort)(int)chara[Chars.AgentID],
                                Data2 = 3
                        };
                        QueuingService.PostProcessingQueue.Enqueue(fadeIntoMap);
                        

                        // spawn him for others & spawn others for him
                        var mapID = World.GetCharacter(Chars.CharID, newCharID).MapID;
                        foreach (var charID in World.GetMap(Maps.MapID, mapID).CharIDs)
                        {
                                CreateSpawnPacketsFor(newCharID, charID);
                                CreateSpawnPacketsFor(charID, newCharID);
                        }

                        // ad him to the map
                        World.GetMap(Maps.MapID, mapID).CharIDs.Add(newCharID);

                        // spawn NPC's
                        foreach (var npc in map.Npcs.Values)
                        {

                                // Note: NPC GENERAL STATS
                                var npcStats = new NetworkMessage((int)chara[Chars.NetID]);
                                npcStats.PacketTemplate = new P074_NpcGeneralStats.PacketSt74()
                                {
                                        NpcID = (uint)npc.Stats.NpcID,
                                        FileID = (uint)npc.Stats.FileID,
                                        Data1 = 0,
                                        Scale = (uint)(npc.Stats.Scale << 24),
                                        Data2 = 0,
                                        ProfessionFlags = (uint)npc.Stats.ProfessionFlags, //| (0x00 << 8),
                                        Profession = (byte)npc.Stats.Profession,
                                        Level = (byte)npc.Stats.Level,
                                        ArraySize1 = (ushort)(npc.Stats.Appearance.Length / 2),
                                        Appearance = npc.Stats.Appearance
                                };
                                QueuingService.PostProcessingQueue.Enqueue(npcStats);

                                // Note: NPC MODEL
                                var npcModel = new NetworkMessage((int)chara[Chars.NetID]);
                                npcModel.PacketTemplate = new P075_NpcModel.PacketSt75()
                                {
                                        NpcID = (uint)npc.Stats.NpcID,
                                        ArraySize1 = (ushort)(npc.Stats.ModelHash.Length / 4),
                                        ModelHash = npc.Stats.ModelHash
                                };
                                QueuingService.PostProcessingQueue.Enqueue(npcModel);

                                // Change the name if necessary
                                if (npc.Stats.HasNameHash)
                                {
                                        // Note: NPC NAME
                                        var npcName = new NetworkMessage((int)chara[Chars.NetID]);
                                        npcName.PacketTemplate = new P143_NpcName.PacketSt143()
                                        {
                                                AgentID = (uint)npc.AgentID,
                                                ArraySize1 = (ushort)(npc.Stats.NameHash.Length / 2),
                                                NameHash = npc.Stats.NameHash
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(npcName);
                                }

                                // Note: UPDATE AGENT MAIN STATS
                                var charMain = new NetworkMessage((int)chara[Chars.NetID]);
                                charMain.PacketTemplate = new P021_SpawnObject.PacketSt21()
                                {
                                        AgentID = (uint)npc.AgentID,
                                        Data1 = (0x20 << 24) | (uint)npc.Stats.NpcID, // was assumed to be LocalID
                                        Data2 = 1,
                                        Data3 = 9,
                                        PosX = npc.Stats.Position.X,
                                        PosY = npc.Stats.Position.Y,
                                        Plane = (ushort)npc.Stats.Position.PlaneZ,
                                        Data4 = float.PositiveInfinity,
                                        Rotation = npc.Stats.Rotation,
                                        Data5 = 1,
                                        Speed = npc.Stats.Speed,
                                        Data12 = 1F,
                                        Data13 = 0x41400000,
                                        //Data14 = 1886151033,
                                        Data14 = 1852796515,
                                        Data15 = 0,
                                        Data16 = 0,
                                        Data17 = 0,
                                        Data18 = 0,
                                        Data19 = 0,
                                        Data20 = 0,
                                        Data21 = 0,
                                        Data22 = float.PositiveInfinity,
                                        Data23 = float.PositiveInfinity,
                                        Data24 = 0,
                                        Data25 = 0,
                                        Data26 = float.PositiveInfinity,
                                        Data27 = float.PositiveInfinity,
                                        Data28 = 0
                                };
                                QueuingService.PostProcessingQueue.Enqueue(charMain);
                        }

                        // update status
                        World.GetClient(Clients.CharID, newCharID).Status = SyncState.Playing;
                }

                private static void CreateSpawnPacketsFor(int charID, int recipientCharID)
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

                        // Note: UPDATE AGENT APPEARANCE
                        var charAppear = new NetworkMessage(reNetID);
                        charAppear.PacketTemplate = new P077_UpdateAgentAppearance.PacketSt77()
                        {
                                Data1 = (uint)(int)chara[Chars.LocalID],
                                ID1 = (uint)(int)chara[Chars.AgentID],
                                Appearance = chara.CharStats.Appearance,
                                Data2 = 0,
                                Data3 = 0,
                                Data4 = 0x3CBFA094,
                                Name = (string)chara[Chars.Name]
                        };
                        QueuingService.PostProcessingQueue.Enqueue(charAppear);

                        // Note: UPDATE AGENT MAIN STATS
                        var charMain = new NetworkMessage(reNetID);
                        charMain.PacketTemplate = new P021_SpawnObject.PacketSt21()
                        {
                                AgentID = (uint)(int)chara[Chars.AgentID],
                                Data1 = (uint)(int)chara[Chars.LocalID] | 805306368, // was assumed to be LocalID
                                Data2 = 1,
                                Data3 = 5,
                                PosX = chara.CharStats.Position.X,
                                PosY = chara.CharStats.Position.Y,
                                Plane = (ushort)chara.CharStats.Position.PlaneZ,
                                Data4 = float.PositiveInfinity,
                                Rotation = chara.CharStats.Rotation,
                                Data5 = 1,
                                Speed = chara.CharStats.Speed,
                                Data12 = float.PositiveInfinity,
                                Data13 = 0x41400000,
                                Data14 = 1886151033,
                                Data15 = 0,
                                Data16 = 0,
                                Data17 = 0,
                                Data18 = 0,
                                Data19 = 0,
                                Data20 = 0,
                                Data21 = 0,
                                Data22 = float.PositiveInfinity,
                                Data23 = float.PositiveInfinity,
                                Data24 = 0,
                                Data25 = 0,
                                Data26 = float.PositiveInfinity,
                                Data27 = float.PositiveInfinity,
                                Data28 = 0
                        };
                        QueuingService.PostProcessingQueue.Enqueue(charMain);

                        // Note: UPDATE AGENT EQUIPMENT
                        var charEquip = new NetworkMessage(reNetID);
                        charEquip.PacketTemplate = new P098_UpdateAgentEquipment.PacketSt98()
                        {
                                ID1 = (uint)(int)chara[Chars.AgentID],
                                Weapon1 = 0,
                                Weapon2 = 0,
                                Chest = 0,
                                Head = 0,
                                Arms = 0,
                                Feet = 0,
                                Legs = 0,
                                Data8 = 0,
                                Data9 = 0
                        };
                        QueuingService.PostProcessingQueue.Enqueue(charEquip);

                        // Note: UPDATE AGENT MORALE
                        var charMorale = new NetworkMessage(reNetID);
                        charMorale.PacketTemplate = new P144_UpdateMorale.PacketSt144()
                        {
                                ID1 = (uint)(int)chara[Chars.AgentID],
                                Morale = (uint)chara.CharStats.Morale,
                        };
                        QueuingService.PostProcessingQueue.Enqueue(charMorale);

                        // Note: UPDATE PUBLIC PROFESSIONS
                        var charProf = new NetworkMessage(reNetID);
                        charProf.PacketTemplate = new P154_UpdatePublicProfessions.PacketSt154()
                        {
                                ID1 = (uint)(int)chara[Chars.AgentID],
                                Prof1 = chara.CharStats.ProfessionPrimary,
                                Prof2 = chara.CharStats.ProfessionSecondary
                        };
                        QueuingService.PostProcessingQueue.Enqueue(charProf);

                        //// Note: UPDATE GENERICVALUE PUBLIC LEVEL
                        //var charLvl = new NetworkMessage(reNetID);
                        //charLvl.PacketTemplate = new P147_UpdateGenericValueInt.PacketSt147()
                        //{
                        //        ID1 = (uint)(int)chara[Chars.AgentID],
                        //        ValueID = (int)GenericValues.PublicLvl,
                        //        Value = (ushort)chara.CharStats.Level
                        //};
                        //QueuingService.PostProcessingQueue.Enqueue(charLvl);

                        // Note: UPDATE VITAL STATS
                        var charVital = new NetworkMessage(reNetID);
                        charVital.PacketTemplate = new P228_UpdateVitalStats.PacketSt228()
                        {
                                ID1 = (uint)(int)chara[Chars.AgentID],
                                VitalFlagsBitfield = (uint)chara.CharStats.VitalStats
                        };
                        QueuingService.PostProcessingQueue.Enqueue(charVital);
                        
                }
        }
}
