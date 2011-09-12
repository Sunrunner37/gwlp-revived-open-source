using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerEngine.Tools
{
        /// <summary>
        ///  This class can basically be used for the whole collision/pathing/movement stuff in GW.
        /// </summary>
        public class GWVector
        {
                private float deltaX;
                private float deltaY;
                private int planeZ;

                /// <summary>
                ///   Initializes a new instance of the class.
                /// </summary>
                public GWVector(float deltaX, float deltaY, int planeZ)
                {
                        this.deltaX = deltaX;
                        this.deltaY = deltaY;
                        this.planeZ = planeZ;
                }

                /// <summary>
                ///   This property contains the X component of a vector.
                /// </summary>
                public float X
                {
                        get { return deltaX; }
                        set { deltaX = value; }
                }

                /// <summary>
                ///   This property contains the Y component of a vector.
                /// </summary>
                public float Y
                {
                        get { return deltaY; }
                        set { deltaY = value; }
                }

                /// <summary>
                ///   This property contains the plane the vector is used for.
                ///   The plane is known as a single chunk of trapezoids within a pmap.
                /// </summary>
                public int PlaneZ
                {
                        get { return planeZ; }
                        set { planeZ = value; }
                }

                /// <summary>
                ///   This property contains the absolute length of a vector.
                ///   Note that this will be calculated.
                /// </summary>
                public float Length
                {
                        get { return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY); }
                        set
                        {
                                GWVector tmp = UnitVector.Clone();
                                deltaX = tmp.X * value;
                                deltaY = tmp.Y * value;
                                planeZ = tmp.PlaneZ;
                        }
                }

                /// <summary>
                ///   This property contains a sub vector of this with the length of 1.
                /// </summary>
                public GWVector UnitVector
                {
                        get { return new GWVector(deltaX / Length, deltaY / Length, planeZ); }
                }

                /// <summary>
                ///   Returns a clone of this.
                /// </summary>
                public GWVector Clone()
                {
                        var result = new GWVector(0, 0, 0) { X = deltaX, Y = deltaY, PlaneZ = planeZ };
                        return result;
                }

                /// <summary>
                ///   Operator overload.
                /// </summary>
                public static GWVector operator +(GWVector v1, GWVector v2)
                {
                        return new GWVector(v1.X + v2.X, v1.Y + v2.Y, v2.PlaneZ);
                }

                /// <summary>
                ///   Operator overload.
                /// </summary>
                public static GWVector operator -(GWVector v1, GWVector v2)
                {
                        return new GWVector(v1.X - v2.X, v1.Y - v2.Y, v2.PlaneZ);
                }

                /// <summary>
                ///   Operator overload.
                /// </summary>
                public static GWVector operator *(GWVector v1, float val)
                {
                        return new GWVector(v1.X * val, v1.Y * val, v1.PlaneZ);
                }

                /// <summary>
                ///   Operator overload.
                /// </summary>
                public static float operator *(GWVector v1, GWVector v2)
                {
                        return (v1.X * v2.Y) - (v1.Y * v2.X);
                }

                /// <summary>
                ///   Factory method
                /// </summary>
                /// <param name="x1">Point 1 X</param>
                /// <param name="y1">Point 1 Y</param>
                /// <param name="x2">Point 2 X</param>
                /// <param name="y2">Point 2 Y</param>
                /// <returns></returns>
                public static GWVector CreateFromAToB(float x1, float y1, float x2, float y2)
                {
                        return new GWVector(x2-x1, y2-y1, 0);
                }
        }
}
