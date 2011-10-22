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
using GameServer.ServerData.Items;

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

                        Item armorPiece = Item.CreateItemStubFromDB(8, 12);

                        armorPiece.Data.OwnerAccID.Value = 10;
                        armorPiece.Data.OwnerCharID.Value = 10;
                        armorPiece.Data.Quantity = 1;
                        armorPiece.Data.Flags = 772998209;
                        armorPiece.Data.Storage = Enums.ItemStorage.Equiped;
                        armorPiece.Data.DyeColor = 10;
                        armorPiece.Data.CreatorCharID = 0;
                        armorPiece.Data.CreatorName = "";

                        armorPiece.Data.PersonalItemID = 8;
                        armorPiece.Data.Slot = (byte)AgentEquipment.Offhand;
                        armorPiece.Data.Stats.Add(new ItemStat(Enums.ItemStatEnums.Attribute, (byte)GWAttribute.Tactics, 9));
                        armorPiece.Data.Stats.Add(new ItemStat(Enums.ItemStatEnums.BaseArmor, 16 , 8));

                        armorPiece.SaveToDB();

                        /*armorPiece.Data.Stats.Add(new ItemStat(Enums.ItemStatEnums.Attribute, (byte)GWAttribute.SpearMastery, 9));
                        armorPiece.Data.Stats.Add(new ItemStat(Enums.ItemStatEnums.DamageType, (byte)DamageType.Piercing, 0));
                        armorPiece.Data.Stats.Add(new ItemStat(Enums.ItemStatEnums.BaseDamageWithReq, 27 , 14));*/
                }
        }
}
