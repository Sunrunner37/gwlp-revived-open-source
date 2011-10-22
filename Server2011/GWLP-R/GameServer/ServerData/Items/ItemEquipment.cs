using System;
using System.Linq;
using System.Collections.Generic;
using GameServer.Enums;

namespace GameServer.ServerData.Items
{
        public class ItemEquipment
        {
                /// <summary>
                ///   Stores infomation about all equiped items.
                /// </summary>
                private uint[] equipedItems;

                public ItemEquipment()
                {
                        equipedItems = new uint[15];
                }

                public void SetPart(AgentEquipment part, uint localID)
                {
                        equipedItems[(int)part] = localID;
                }

                public uint GetPart(AgentEquipment part)
                {
                        return equipedItems[(int)part];
                }
                
                public uint Leadhand
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.Leadhand];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.Leadhand] = value;
                        }
                }

                public uint Offhand
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.Offhand];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.Offhand] = value;
                        }
                }

                public uint Head
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.Head];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.Head] = value;
                        }
                }

                public uint Chest
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.Chest];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.Chest] = value;
                        }
                }

                public uint Arms
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.Arms];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.Arms] = value;
                        }
                }

                public uint Legs
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.Legs];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.Legs] = value;
                        }
                }

                public uint Feet
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.Feet];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.Feet] = value;
                        }
                }

                public uint CostumeHead
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.CostumeHead];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.CostumeHead] = value;
                        }
                }

                public uint Costume
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.Costume];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.Costume] = value;
                        }
                }

                public uint Backpack
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.Backpack];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.Backpack] = value;
                        }
                }

                public uint Beltpouch
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.Beltpouch];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.Beltpouch] = value;
                        }
                }

                public uint Bag1
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.Bag1];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.Bag1] = value;
                        }
                }

                public uint Bag2
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.Bag2];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.Bag2] = value;
                        }
                }

                public uint EquipmentPack
                {
                        get
                        {
                                return equipedItems[(int)AgentEquipment.EquipmentPack];
                        }
                        set
                        {
                                equipedItems[(int)AgentEquipment.EquipmentPack] = value;
                        }
                }
        }
}