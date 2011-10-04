using System;
using System.Collections.Generic;

namespace ServerEngine
{
        public sealed class IDManager
        {
                private readonly object objLock = new object();

                private readonly Stack<int> freeValues;

                /// <summary>
                ///   Initializes a new instance of the class.
                /// </summary>
                /// <param name="lowestPossible">
                ///   The smallest value that the ID's could take
                /// </param>
                /// <param name="highestPossible">
                ///   The highest possible ID
                /// </param>
                public IDManager(int lowestPossible, int highestPossible)
                {
                        lock (objLock)
                        {
                                // fail check
                                lowestPossible = lowestPossible <= 0 ? 1 : lowestPossible;
                                highestPossible = highestPossible > 1000000 ? 1000000 : highestPossible;

                                if (lowestPossible >= highestPossible)
                                {
                                        throw new IndexOutOfRangeException(
                                                "Lowest value must be at least 1 smaller than highest value");
                                }

                                // create the freeID's stack
                                var freeIDs = new List<int>();
                                for (var i = highestPossible; i > lowestPossible; i--)
                                {
                                        freeIDs.Add(i);
                                }

                                freeValues = new Stack<int>(freeIDs);
                        }
                }

                /// <summary>
                ///   This returns a currently unused ID.
                /// </summary>
                public int RequestID()
                {
                        lock (objLock)
                        {
                                if (freeValues.Count != 0)
                                {
                                        return freeValues.Pop();
                                }

                                throw new IndexOutOfRangeException("No more free IDs");
                        }
                }

                /// <summary>
                ///   This frees a used ID.
                /// </summary>
                public void FreeID(int toFree)
                {
                        lock (objLock)
                        {
                                freeValues.Push(toFree);
                        }
                }
        }
}