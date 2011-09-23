using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using GameServer.Actions;
using GameServer.DataBase;
using GameServer.Enums;
using System.Linq;
using GameServer.Interfaces;
using GameServer.ServerData;
using ServerEngine.DataBase;
using ServerEngine.Tools;

namespace GameServer.Modules
{
        public class Movement : IModule
        {
                public Movement(string pmapsDir)
                {
                        // init the dict
                        maps = new Dictionary<int, PathingMap>();

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
                                                newMap.GameFileHash = (uint)pmapHead.MapHash;
                                                newMap.Translation = new GWVector(pmapHead.StartX, pmapHead.StartY, 0);
                                                newMap.Size = new GWVector(pmapHead.Width, pmapHead.Heigth, 0);

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
                                                                Debug.WriteLine(string.Format("Database incomplete! Cannot load pathing map file '{0}', but continuing with next file", pmapPath));
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

                                                                var newTrapezoid = new Trapezoid()
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

                private static DateTime lastHandledMov;
                private static DateTime lastHandledCollision;
                private static Dictionary<int, PathingMap> maps;

                public void Execute()
                {
                        World.GetMaps().AsParallel().ForAll(ProcessMovePackets);
                }

                private static void ProcessMovePackets(Map map)
                {
                        foreach (int charID in map.CharIDs)
                        {
                                var chara = World.GetCharacter(Chars.CharID, charID);
                                GWVector newAim = chara.CharStats.Position;

                                //make sure we always have a unit vector
                                if (chara.CharStats.Direction.X != 0 || chara.CharStats.Direction.Y != 0)
                                    chara.CharStats.Direction = chara.CharStats.Direction.UnitVector;

                                if (chara.CharStats.MoveState != MovementState.NotMoving)
                                {
                                        CheckCollision(chara, map, out newAim);
                                }

                                switch (chara.CharStats.MoveState)
                                {
                                        case MovementState.MoveChangeDir:
                                                {
                                                        var action = new MovePlayer((int)chara[Chars.CharID], newAim);
                                                        map.ActionQueue.Enqueue(action.Execute);
                                                        chara.CharStats.MoveState = MovementState.MoveKeepDir;
                                                }
                                                break;
                                        case MovementState.MoveKeepDir:
                                                if (DateTime.Now.Subtract(lastHandledMov).Milliseconds > 250)
                                                {
                                                        var action = new MovePlayer((int)chara[Chars.CharID], newAim);
                                                        map.ActionQueue.Enqueue(action.Execute);

                                                        lastHandledMov = DateTime.Now;
                                                }
                                                break;
                                        case MovementState.NotMovingUnhandled: // c
                                                {
                                                        var action = new MovePlayer((int)chara[Chars.CharID], chara.CharStats.Position);
                                                        map.ActionQueue.Enqueue(action.Execute);
                                                        chara.CharStats.MoveState = MovementState.NotMoving;
                                                }
                                                break;
                                }
                        }
                }

                private static void CheckCollision(Character chara, Map map, out GWVector newAim)
                {
                        PathingMap pmap = null;

                        // check if we've got a map of it
                        if (maps.TryGetValue(chara.MapID, out pmap))
                        {
                                // update chara's Trapezoid
                                UpdateTrapezoidIndex(chara, pmap);

                                // get the distance to the border (value between 0[at border] to 320)
                                int distanceToBorder;
                                newAim = AimToBorder(chara, chara.CharStats.Direction, pmap, out distanceToBorder);

                                //check for collition
                                if (distanceToBorder == 0)
                                {
                                    if (DateTime.Now.Subtract(lastHandledCollision).Milliseconds > 500)
                                    {
                                        if (chara.CharStats.AtBorder)
                                        {
                                                GWVector deltaPos = chara.CharStats.Position - chara.CharStats.LastCollisionPosition;

                                                if (deltaPos.X != 0 || deltaPos.Y != 0)
                                                    deltaPos = newAim.UnitVector;

                                                newAim = AimToBorder(chara, deltaPos, pmap, out distanceToBorder);

                                                float speedModifier;
                                                var trap = pmap.Trapezoids[(int)chara.CharStats.TrapezoidIndex];

                                                GetCollisionMovement(chara.CharStats.Direction, trap, out speedModifier);

                                                if (Math.Abs(speedModifier) < 0.5)
                                                {
                                                    chara.CharStats.SpeedModifier = 0.35F;
                                                }
                                                else
                                                {
                                                    chara.CharStats.SpeedModifier = 0.7F;
                                                }


                                                var action = new MovePlayer((int)chara[Chars.CharID], newAim);
                                                map.ActionQueue.Enqueue(action.Execute);
                                        }
                                        else
                                        {
                                            chara.CharStats.AtBorder = true;
                                        }

                                        // report collision
                                        chara.CharStats.LastCollisionPosition = chara.CharStats.Position;
                                        chara.CharStats.MoveType = (int)MovementType.Collision;
                                        lastHandledCollision = DateTime.Now;
                                    }
                                }
                                else
                                {
                                    chara.CharStats.AtBorder = false;
                                    chara.CharStats.SpeedModifier = 1F; //dunno if unnecessary
                                }
                        }
                        else
                        {
                                // no pmap, no collision check
                                newAim = chara.CharStats.Position;
                        }
                }

                private static void UpdateTrapezoidIndex(Character chara, PathingMap pmap)
                {
                        // check if the client is in the given trapezoid
                        var state = InTrapezoid(pmap.Trapezoids[(int)chara.CharStats.TrapezoidIndex], chara.CharStats.Position);

                        uint trapIndex = 0xFFFFFFFF;
                        switch (state)
                        {
                                case 0: // inside trapezoid, everything ok.
                                        {
                                                trapIndex = 0;
                                        }
                                        break;
                                case 1: // the client is under the trapezoid, we need to change traps
                                        {
                                                trapIndex = SearchTrapezoid(pmap, pmap.Trapezoids[(int)chara.CharStats.TrapezoidIndex].AdjacentsBottom, chara.CharStats.Position);
                                        }
                                        break;
                                case 3: // the client is above the trapezoid, we need to change traps
                                        {
                                                trapIndex = SearchTrapezoid(pmap, pmap.Trapezoids[(int)chara.CharStats.TrapezoidIndex].AdjacentsTop, chara.CharStats.Position);
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
                                chara.CharStats.TrapezoidIndex = trapIndex;
                                chara.CharStats.Position.PlaneZ = pmap.Trapezoids[(int)trapIndex].Plane;
                        }
                }

                private static GWVector AimToBorder(Character chara, GWVector direction, PathingMap pmap, out int distanceToBorder)
                {
                        GWVector dir = direction * 32;
                        GWVector targetLoc = chara.CharStats.Position;
                        GWVector aimToBorder = targetLoc;
                        uint trapIndex = chara.CharStats.TrapezoidIndex;
                        uint lastTrapIndex = trapIndex;

                        for (int i = 0; i < 10; i++)
                        {
                                lastTrapIndex = trapIndex;
                                aimToBorder = targetLoc;
                                targetLoc = aimToBorder + dir;

                                switch (InTrapezoid(pmap.Trapezoids[(int)trapIndex], targetLoc))
                                {
                                        case 0: // the targetLoc is inside trapezoid, everything ok.
                                                break;
                                        case 1: // the targetLoc is under the trapezoid, we need to change traps
                                                {
                                                        trapIndex = SearchTrapezoid(pmap, pmap.Trapezoids[(int)trapIndex].AdjacentsBottom, targetLoc);
                                                        if (trapIndex == 0xFFFFFFFF)
                                                        {
                                                            aimToBorder.PlaneZ = chara.CharStats.Position.PlaneZ; // pmap.Trapezoids[(int)lastTrapIndex].Plane;
                                                            distanceToBorder = i;
                                                            return aimToBorder;
                                                        }
                                                }
                                                break;
                                        case 3: // the targetLoc is above the trapezoid, we need to change traps
                                                {
                                                        trapIndex = SearchTrapezoid(pmap, pmap.Trapezoids[(int)trapIndex].AdjacentsTop, targetLoc);
                                                        if (trapIndex == 0xFFFFFFFF)
                                                        {
                                                            aimToBorder.PlaneZ = chara.CharStats.Position.PlaneZ; // pmap.Trapezoids[(int)lastTrapIndex].Plane;
                                                            distanceToBorder = i;
                                                            return aimToBorder;
                                                        }
                                                }
                                                break;
                                        default: // case 2 or 4: the char is out of the map borders
                                                {
                                                        aimToBorder.PlaneZ = chara.CharStats.Position.PlaneZ; // pmap.Trapezoids[(int)lastTrapIndex].Plane;
                                                        distanceToBorder = i;
                                                        return aimToBorder;
                                                }
                                }
                        }

                        aimToBorder.PlaneZ = chara.CharStats.Position.PlaneZ; // pmap.Trapezoids[(int)lastTrapIndex].Plane;
                        distanceToBorder = 9;
                        return aimToBorder;
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

                private static void GetCollisionMovement(GWVector dir, Trapezoid trap, out float speedModifier)
                {
                        speedModifier = 1F;

                        GWVector border;

                        if (Math.Abs(dir.X) < Math.Abs(dir.Y))
                        {
                                if (dir.Y > 0)
                                { //top border
                                        border = (trap.TopLeft - trap.TopRight);
                                        speedModifier = border.CosWith(dir);
                                }
                                else
                                { //bottom border
                                        border = (trap.BottomRight - trap.BottomLeft);
                                        speedModifier = border.CosWith(dir);
                                }
                        }
                        else
                        {
                                if (dir.X > 0)
                                { //right border
                                        border = (trap.TopRight - trap.BottomRight);
                                        speedModifier = border.CosWith(dir);
                                }
                                else
                                { //left border
                                        border = (trap.BottomLeft - trap.TopLeft);
                                        speedModifier = border.CosWith(dir);
                                }
                        }
                }

                private class PathingMap
                {
                        public int MapID;
                        public uint GameFileHash;           // hash of mapfile
                        public List<Trapezoid> Trapezoids;
                        public GWVector Translation;
                        public GWVector Size;

                        public PathingMap()
                        {
                                Trapezoids = new List<Trapezoid>();
                        }
                }

                private class Trapezoid
                {
                        public uint TrapezoidID;
                        public List<uint> AdjacentsBottom;
                        public List<uint> AdjacentsTop;
                        public GWVector BottomRight;
                        public GWVector BottomLeft;
                        public GWVector TopRight;
                        public GWVector TopLeft;

                        public int Plane
                        {
                                get { return BottomRight.PlaneZ; }
                                set
                                {
                                        BottomRight.PlaneZ = value;
                                        BottomLeft.PlaneZ = value;
                                        TopRight.PlaneZ = value;
                                        TopLeft.PlaneZ = value;
                                }
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
