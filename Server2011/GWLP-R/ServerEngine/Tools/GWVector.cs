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
                ///   Returns the angle with a given vector
                /// </summary>
                public float CosWith(GWVector v1)
                {
                        return (((v1.X * X) + (v1.Y * Y)) / (v1.Length * Length));
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
                ///   Note: NO SKALAR Product
                /// </summary>
                public static float operator *(GWVector v1, GWVector v2)
                {
                        return (v1.X * v2.Y) - (v1.Y * v2.X);
                }

                /// <summary>
                ///   ToString overload.
                /// </summary>
                public override string ToString()
                {
                    return "[" + deltaX.ToString() + ", " + deltaY.ToString() + "]";
                }
        }
}
