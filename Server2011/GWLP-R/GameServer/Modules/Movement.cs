using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using GameServer.Actions;
using GameServer.Enums;
using System.Linq;
using GameServer.Interfaces;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataBase;
using ServerEngine.GuildWars.Tools;

namespace GameServer.Modules
{
        public class Movement : IModule
        {
                public Movement(string pmapsDir)
                {
                        // init the dict
                        maps = new Dictionary<int, PathingMap>();
                        
                        // failcheck
                        if (!Directory.Exists(pmapsDir)) return;

                        // get the file names (include sub directories)
                        var pmapPaths = Directory.GetFiles(pmapsDir, "*.pmap", SearchOption.AllDirectories);

                        // load the files
                        foreach (var pmapPath in pmapPaths)
                        {
                                try
                                {
                                        using (var fs = File.OpenRead(pmapPath))
                                        {
                                                // set the buffer
                                                var buffer = new byte[fs.Length];

                                                // create a new map
                                                var newMap = new PathingMap();

                                                // read the file into memory
                                                fs.Lock(0, fs.Length);
                                                fs.Read(buffer, 0, (int)fs.Length);
                                                fs.Unlock(0, fs.Length);
                                                fs.Close();

                                                // unnecessary...
                                                //GCHandle pinBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                                                // read the header
                                                var pmapHead = (PathingMapHeader)Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0), typeof(PathingMapHeader));

                                                // weak file check:
                                                if (pmapHead.Magic != 1347240272)
                                                        throw new Exception(string.Format("Unkown file type! Cannot load pathing map file '{0}'", pmapPath));

                                                // add the most necessary header data
                                                newMap.GameFileHash = pmapHead.MapHash;

                                                // scan the database for the map id
                                                int mapID;
                                                using (var db = (MySQL)DataBaseProvider.GetDataBase())
                                                {
                                                        var map = from m in db.mapsMasterData
                                                                where m.gameMapFileID == newMap.GameFileHash
                                                                select m;

                                                        if (map.Count() != 0)
                                                        {
                                                                mapID = map.First().gameMapID;
                                                        }
                                                        else
                                                        {
                                                                mapID = 0;
                                                                Debug.WriteLine(string.Format("DB Data missing for '{0}' [skipped]", pmapPath));
                                                        }
                                                }
                                                newMap.MapID = mapID;

                                                // security check, cause we dont have all maps available
                                                if (newMap.MapID != 0)
                                                {
                                                        var trapezoids = new List<PathingTrapezoid>();
                                                        var adjacents = new List<Adjacent>();

                                                        // read the trapezoids
                                                        var tmpPos = pmapHead.FirstTrapezoid;
                                                        for (var i = 0; i < pmapHead.TrapezoidCount; i++)
                                                        {
                                                                // get the trapezoid
                                                                var trapezoid = (PathingTrapezoid)Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(buffer, tmpPos), typeof(PathingTrapezoid));

                                                                trapezoids.Add(trapezoid);

                                                                tmpPos += Marshal.SizeOf(trapezoid);
                                                        }

                                                        // read the adjacents
                                                        tmpPos = pmapHead.FirstAdjacent;
                                                        for (var i = 0; i < pmapHead.AdjacentCount; i++)
                                                        {
                                                                // get the adjacent struct
                                                                var adj = (Adjacent)Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(buffer, tmpPos), typeof(Adjacent));

                                                                adjacents.Add(adj);

                                                                tmpPos += Marshal.SizeOf(adj);
                                                        }

                                                        // now re-format the trapezoids:
                                                        for (var i = 0; i < trapezoids.Count; i++)
                                                        {
                                                                var trapezoid = trapezoids[i];

                                                                var newTrapezoid = new Trapezoid
                                                                {
                                                                        TrapezoidID = (uint)i,
                                                                        BottomRight = new GWVector(trapezoid.XBR, trapezoid.YB, trapezoid.Plane),
                                                                        BottomLeft = new GWVector(trapezoid.XBL, trapezoid.YB, trapezoid.Plane),
                                                                        TopRight = new GWVector(trapezoid.XTR, trapezoid.YT, trapezoid.Plane),
                                                                        TopLeft = new GWVector(trapezoid.XBL, trapezoid.YT, trapezoid.Plane),
                                                                };

                                                                for (int j = 0; j < trapezoid.AdjacentsCount; j++)
                                                                {
                                                                        if (adjacents[trapezoid.Adjacents + j].YL == trapezoid.YB)
                                                                        {
                                                                                newTrapezoid.AdjacentsBottom.Add(adjacents[trapezoid.Adjacents + j].Trapezoid);
                                                                        }
                                                                        if (adjacents[trapezoid.Adjacents + j].YL == trapezoid.YT)
                                                                        {
                                                                                newTrapezoid.AdjacentsTop.Add(adjacents[trapezoid.Adjacents + j].Trapezoid);
                                                                        }
                                                                }

                                                                newMap.Trapezoids.Add(newTrapezoid);
                                                        }

                                                        // ready with the main stuff (read the whole file), lets add the map to the dict
                                                        maps.Add(newMap.MapID, newMap);
                                                }

                                        }
                                }
                                catch (Exception e)
                                {
                                        Debug.WriteLine(e.Message);
                                }
                        }
                }

                private static DateTime lastCycle;
                private static float dt; // time per cycle
                public static Dictionary<int, PathingMap> maps;

                public void Execute()
                {
                        dt = 0.001F * DateTime.Now.Subtract(lastCycle).Milliseconds;
                        lastCycle = DateTime.Now;

                        GameServerWorld.Instance.GetAll<DataMap>().AsParallel().ForAll(ProcessMovePackets);
                }

                private static void ProcessMovePackets(DataMap map)
                {
                        // the following linq expression returns an IEnumerable<CharID> of all characters on that map
                        foreach (var chara in map.GetAll<DataCharacter>())
                        {
                                // failcheck
                                if (chara == null) continue;

                                var state = chara.Data.MoveState;

                                if (state != MovementState.NotMoving)
                                {
                                        CalculatePosition(chara);
                                }

                                switch (state)
                                {
                                        case MovementState.MoveChangeDir:
                                                {
                                                        var action = new MovePlayer(chara.Data.CharID, chara.Data.Position);
                                                        map.Data.ActionQueue.Enqueue(action.Execute);

                                                        chara.Data.MoveState = MovementState.MoveKeepDir;
                                                        chara.Data.LastMovement = DateTime.Now;
                                                }
                                                break;
                                        case MovementState.MoveKeepDir:
                                                {
                                                        if (DateTime.Now.Subtract(chara.Data.LastMovement).Milliseconds > 250)
                                                        {
                                                                var action = new MovePlayer(chara.Data.CharID, chara.Data.Position);
                                                                map.Data.ActionQueue.Enqueue(action.Execute);

                                                                chara.Data.LastMovement = DateTime.Now;
                                                        }
                                                }
                                                break;
                                        case MovementState.NotMovingUnhandled:
                                                {
                                                        var action = new MovePlayer(chara.Data.CharID, chara.Data.Position);
                                                        map.Data.ActionQueue.Enqueue(action.Execute);

                                                        chara.Data.Rotation = (float)Math.Atan2(chara.Data.Direction.Y, chara.Data.Direction.X);
                                                        var action2 = new RotatePlayer(chara.Data.CharID);
                                                        map.Data.ActionQueue.Enqueue(action2.Execute);

                                                        chara.Data.MoveState = MovementState.NotMoving;
                                                        chara.Data.LastMovement = DateTime.Now;
                                                }
                                                break;
                                }
                        }
                }

                private static void CalculatePosition(DataCharacter chara)
                {
                        PathingMap pmap;

                        // failcheck... if we've got a map of it
                        if (!maps.TryGetValue((int) chara.Data.MapID.Value, out pmap)) return;

                        // update chara's Trapezoid
                        UpdateTrapezoidIndex(chara, pmap);

                        if (chara.Data.MoveState != MovementState.MoveKeepDir) return;

                        var dir = chara.Data.Direction * (chara.Data.Speed * dt);
                        var newPos = dir + chara.Data.Position;

                        GWVector crossedBorder;
                        var outOfBorder = GetOutOfBorders(newPos, pmap.Trapezoids[(int)chara.Data.TrapezoidIndex], pmap, out crossedBorder);

                        if (outOfBorder)
                        {
                                var length = crossedBorder.Length;

                                if (length != 0)
                                {
                                        var f = (crossedBorder.X * dir.X + crossedBorder.Y * dir.Y) / (length * length);

                                        var newDir = crossedBorder * f;

                                        if (newDir.X != 0 || newDir.Y != 0)
                                        {
                                                newPos = newDir + chara.Data.Position;

                                                var angle = newDir.CosWith(dir);

                                                chara.Data.SpeedModifier = Math.Max(0.33F, angle);
                                                chara.Data.MoveType = MovementType.Collision;

                                                outOfBorder = GetOutOfBorders(newPos, pmap.Trapezoids[(int)chara.Data.TrapezoidIndex], pmap, out crossedBorder);

                                                if (!outOfBorder)
                                                {
                                                        chara.Data.Position = newPos;
                                                }
                                        }
                                }
                        }
                        else
                        {
                                chara.Data.Position = newPos;
                        }
                }

                private static bool GetOutOfBorders(GWVector pos, Trapezoid currentTrap, PathingMap pmap, out GWVector crossedBorder)
                {
                        var state = InTrapezoid(currentTrap, pos);
                        IEnumerable<uint> trapezoids;

                        switch (state)
                        {
                            case 0:
                                crossedBorder = new GWVector(0, 0, 0);
                                return false;
                            case 1:
                                {
                                    crossedBorder = currentTrap.BottomLeft - currentTrap.BottomRight;
                                    trapezoids = currentTrap.AdjacentsBottom;
                                }
                                break;
                            case 2:
                                {
                                    crossedBorder = currentTrap.BottomRight - currentTrap.TopRight;
                                    return true;
                                }
                            case 3:
                                {
                                    crossedBorder = currentTrap.TopRight - currentTrap.TopLeft;
                                    trapezoids = currentTrap.AdjacentsTop;
                                }
                                break;
                            default: //case 4
                                {
                                    crossedBorder = currentTrap.TopLeft - currentTrap.BottomLeft;
                                    return true;
                                }
                        }

                        var trapIndex = SearchTrapezoid(pmap, trapezoids, pos);

                        return trapIndex == 0xFFFFFFFF;
                }

                private static void UpdateTrapezoidIndex(DataCharacter chara, PathingMap pmap)
                {
                        // check if the client is in the given trapezoid
                        var state = InTrapezoid(pmap.Trapezoids[(int)chara.Data.TrapezoidIndex], chara.Data.Position);

                        uint trapIndex;
                        switch (state)
                        {
                                case 0: // inside trapezoid, everything ok.
                                        {
                                                trapIndex = 0;
                                        }
                                        break;
                                case 1: // the client is under the trapezoid, we need to change traps
                                        {
                                                trapIndex = SearchTrapezoid(pmap, pmap.Trapezoids[(int)chara.Data.TrapezoidIndex].AdjacentsBottom, chara.Data.Position);
                                        }
                                        break;
                                case 3: // the client is above the trapezoid, we need to change traps
                                        {
                                                trapIndex = SearchTrapezoid(pmap, pmap.Trapezoids[(int)chara.Data.TrapezoidIndex].AdjacentsTop, chara.Data.Position);
                                        }
                                        break;
                                default: // case 2 or 4: the char is out of the map borders
                                        {
                                                trapIndex = 0xFFFFFFFF;
                                        }
                                        break;
                        }

                        if (trapIndex > 0 && trapIndex != 0xFFFFFFFF)
                        {
                                chara.Data.TrapezoidIndex = trapIndex;
                                chara.Data.Position.PlaneZ = pmap.Trapezoids[(int)trapIndex].Plane;
                        }
                }

                /// <summary>
                ///   Checks if a point is in a trapezoid.
                ///   Returns the probably crossed border.
                ///   (0= in trap, 1= bottom, 2= right, 3= top, 4= left)
                /// </summary>
                /// <param name="trap">The trapezoid</param>
                /// <param name="pos">The point</param>
                /// <returns></returns>
                private static int InTrapezoid(Trapezoid trap, GWVector pos)
                {
                        // is pos under the bottom?
                        if (pos.Y < trap.BottomRight.Y)
                                return 1;

                        // is pos over the top?
                        if (pos.Y > trap.TopRight.Y)
                                return 3;

                        // is pos out of right border?
                        var bCrossCtoP = (trap.TopRight - trap.BottomRight) * (pos - trap.TopRight);
                        if (bCrossCtoP < 0)
                                return 2;

                        // is pos out of left border?
                        var dCrossAtoP = (trap.BottomLeft - trap.TopLeft) * (pos - trap.BottomLeft);
                        if (dCrossAtoP < 0)
                                return 4;

                        return 0;
                }

                private static bool TryGetTrapezoid(PathingMap pmap, GWVector pos, out Trapezoid trapezoid)
                {
                        foreach (var trap in pmap.Trapezoids)
                        {
                                if (trap.Plane != pos.PlaneZ) continue;
                                if (InTrapezoid(trap, pos) == 0)
                                {
                                        trapezoid = trap;
                                        return true;
                                }
                        }

                        trapezoid = null;
                        return false;
                }

                private static GWVector GetCenterOfTrapezoid(Trapezoid trap)
                {
                        return new GWVector((trap.BottomLeft.X+trap.BottomRight.X + trap.TopLeft.X + trap.TopRight.X) / 4,
                                (trap.TopLeft.Y + trap.BottomLeft.Y) / 2,
                                trap.Plane);
                }

                /// <summary>
                ///   Search for the trapezoid the player is in.
                /// </summary>
                /// <param name="map"></param>
                /// <param name="preferedTrapezoidIDs"></param>
                /// <param name="pos"></param>
                /// <returns></returns>
                private static uint SearchTrapezoid(PathingMap map, IEnumerable<uint> preferedTrapezoidIDs, GWVector pos)
                {
                        foreach (var trapezoidID in preferedTrapezoidIDs)
                        {
                                if (InTrapezoid(map.Trapezoids[(int)trapezoidID], pos) == 0)
                                {
                                        return trapezoidID;
                                }
                        }

                        // if nothing was found, search all but the ones already tried
                        foreach (var trapezoid in map.Trapezoids)
                        {
                                if (!preferedTrapezoidIDs.Contains(trapezoid.TrapezoidID) && InTrapezoid(trapezoid, pos) == 0)
                                {
                                        return trapezoid.TrapezoidID;
                                }
                        }

                        // if it isnt on the trapezoids anymore
                        return 0xFFFFFFFF;
                }

                public class PathingMap
                {
                        public int MapID;
                        public uint GameFileHash;           // hash of mapfile
                        public readonly List<Trapezoid> Trapezoids;

                        public PathingMap()
                        {
                                Trapezoids = new List<Trapezoid>();
                        }
                }

                public class Trapezoid
                {
                        public uint TrapezoidID;
                        public readonly List<uint> AdjacentsBottom;
                        public readonly List<uint> AdjacentsTop;
                        public GWVector BottomRight;
                        public GWVector BottomLeft;
                        public GWVector TopRight;
                        public GWVector TopLeft;

                        public int Plane
                        {
                                get { return BottomRight.PlaneZ; }
                        }

                        public Trapezoid()
                        {
                                AdjacentsBottom = new List<uint>();
                                AdjacentsTop = new List<uint>();
                        }
                }

                [StructLayout(LayoutKind.Sequential, Pack = 1)]
                private struct PathingMapHeader
                {
                        public int Magic;                          // MAGIC Bytes (PMAP)
                        public uint MapHash;                  // hash of mapfile
                        public int TrapezoidCount;
                        public int FirstTrapezoid;         //Offset to Trapezoid Chunk
                        public int AdjacentCount;
                        public int FirstAdjacent;          //Offset to Adjacent Chunk
                        public float StartX;
                        public float StartY;
                        public float Width;
                        public float Heigth;
                }

                [StructLayout(LayoutKind.Sequential, Pack = 1)]
                struct PathingTrapezoid
                {
                        public int AdjacentsCount;
                        public int Adjacents;		//start index in adjacent array
                        public short Plane;		//plane the trapezoid is in
                        public float YT;
                        public float YB;
                        public float XTL;
                        public float XTR;
                        public float XBL;
                        public float XBR;
                };

                [StructLayout(LayoutKind.Sequential, Pack = 1)]
                struct Adjacent
                {
                        public uint Trapezoid;                  //index of trapezoid
                        public float XL;		//left/right relative to walking direction
                        public float YL;		//			L /\ R
                        public float XR;		//			  || <-- walking direction
                        public float YR;		//			R \/ L
                };
        }
}
