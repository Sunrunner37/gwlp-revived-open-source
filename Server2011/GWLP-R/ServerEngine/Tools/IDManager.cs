using System.Collections.Generic;

namespace ServerEngine.Tools
{
        public sealed class IDManager
        {
                private readonly int startValue;
                private readonly List<int> usedValues;

                /// <summary>
                ///   Initializes a new instance of the class.
                /// </summary>
                /// <param name="startValue">
                ///   The smallest value that the ID's could take
                /// </param>
                public IDManager(int startValue)
                {
                        this.startValue = startValue;

                        usedValues = new List<int>();
                }

                /// <summary>
                ///   This returns a currently unused ID, and sets it to 'used'.
                /// </summary>
                /// <returns>
                ///   Returns a currently unused ID.
                /// </returns>
                public int NewID()
                {
                        int result = GetSmallestFree();

                        usedValues.Add(result);

                        return result;
                }

                /// <summary>
                ///   This frees a used ID.
                /// </summary>
                /// <param name="toFree">
                ///   The ID that will be freed.
                /// </param>
                public void FreeID(int toFree)
                {
                        usedValues.Remove(toFree);
                }

                /// <summary>
                ///   This searches for the smallest unused value.
                ///   (Which is >= startValue and not in usedValues)
                /// </summary>
                /// <returns>
                ///   Returns the smallest unused value.
                /// </returns>
                private int GetSmallestFree()
                {
                        int result = startValue;

                        usedValues.Sort();
                        foreach (int val in usedValues)
                        {
                                // if there is a gap:
                                if (val > (result + 1))
                                {
                                        result++;
                                        break;
                                } // or if it is just greater than the last checked ID
                                else if (val >= result)
                                {
                                        result = val + 1;
                                }
                        }

                        return result;
                }
        }
}