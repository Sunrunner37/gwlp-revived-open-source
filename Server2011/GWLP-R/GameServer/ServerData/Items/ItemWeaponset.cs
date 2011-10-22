using System;
using System.Linq;
using System.Collections.Generic;
using GameServer.Enums;

namespace GameServer.ServerData.Items
{
        public class ItemWeaponset
        {
                /// <summary>
                ///   Stores infomation about all weaponsets.
                ///   NOTE: Loading and saving from/to db always with personalID!
                ///         Dont use this object to do so directly!
                ///   when the characters items get loaded this object will get updated
                ///   from personalID to its correspondant localID.
                /// </summary>
                private uint[] leadhand;
                private uint[] offhand;

                public ItemWeaponset()
                {
                        leadhand = new uint[4];
                        offhand = new uint[4];
                }

                public void SetLeadhand(int set, uint value)
                {
                        leadhand[set] = value;
                }

                public uint GetLeadhand(int set)
                {
                        return leadhand[set];
                }

                public void SetOffhand(int set, uint value)
                {
                        offhand[set] = value;
                }

                public uint GetOffhand(int set)
                {
                        return offhand[set];
                }

                public uint Leadhand1
                {
                        get
                        {
                                return leadhand[0];
                        }
                        set
                        {
                                leadhand[0] = value;
                        }
                }

                public uint Leadhand2
                {
                        get
                        {
                                return leadhand[1];
                        }
                        set
                        {
                                leadhand[1] = value;
                        }
                }

                public uint Leadhand3
                {
                        get
                        {
                                return leadhand[2];
                        }
                        set
                        {
                                leadhand[2] = value;
                        }
                }

                public uint Leadhand4
                {
                        get
                        {
                                return leadhand[3];
                        }
                        set
                        {
                                leadhand[3] = value;
                        }
                }

                public uint Offhand1
                {
                        get
                        {
                                return offhand[0];
                        }
                        set
                        {
                                offhand[0] = value;
                        }
                }

                public uint Offhand2
                {
                        get
                        {
                                return offhand[1];
                        }
                        set
                        {
                                offhand[1] = value;
                        }
                }

                public uint Offhand3
                {
                        get
                        {
                                return offhand[2];
                        }
                        set
                        {
                                offhand[2] = value;
                        }
                }

                public uint Offhand4
                {
                        get
                        {
                                return offhand[3];
                        }
                        set
                        {
                                offhand[3] = value;
                        }
                }
        }
}