using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerEngine.Tools
{
        public class MultiKeyDictionary<TKeyIdentifier, TValue>
                where TKeyIdentifier : struct, IComparable, IFormattable, IConvertible
                where TValue : IIdentifiable<TKeyIdentifier> // because one cant use ": Enum"
        {
                public MultiKeyDictionary()
                {
                        dict = new Dictionary<int, TValue>();
                }

                private readonly Dictionary<int, TValue> dict;

                public bool TryGetValue(KeyValuePair<TKeyIdentifier, object> indetifierKeyPair, out TValue value)
                {
                        lock (dict)
                        {
                                if (dict.TryGetValue(HashID(indetifierKeyPair.Key.GetHashCode(), indetifierKeyPair.Value.GetHashCode()), out value))
                                        return true;
                                return false;
                        }
                }

                /// <summary>
                ///   Removes all refences to a value that could be identified with the parameter.
                ///   The value cant be found with a different identifier of it, cause all
                ///   references have been deleted.
                /// </summary>
                /// <param name="indetifierKeyPair"></param>
                /// <returns></returns>
                public bool Remove(KeyValuePair<TKeyIdentifier, object> indetifierKeyPair)
                {
                        lock (dict)
                        {
                                TValue value;
                                if (dict.TryGetValue(HashID(indetifierKeyPair.Key.GetHashCode(), indetifierKeyPair.Value.GetHashCode()), out value))
                                {
                                        var hashes = from h in dict.AsEnumerable()
                                                     where h.Value.GetHashCode() == value.GetHashCode()
                                                     select h.Key;
                                        int[] keys = hashes.ToArray();
                                        foreach (var key in keys)
                                        {
                                                dict.Remove(key);
                                        }

                                        return true;
                                }

                                return false;
                        }
                }

                public void Add(TValue value)
                {
                        lock (dict)
                        {
                                foreach (var kvp in value.IdentifierKeyEnumeration)
                                {
                                        dict.Add(HashID(kvp.Key.GetHashCode(), kvp.Value.GetHashCode()), value);
                                }
                        }
                }

                public Type GetValueType()
                {
                        return typeof(TValue);
                }

                public IEnumerable<TValue> Values { get { return dict.Values; } }

                /// <summary>
                ///   Creates an int packed with id and id type to be used as a dictionary key.
                /// </summary>
                /// <returns>The resulting int</returns>
                private static int HashID(int enumObjHash, int keyHash)
                {
                        int result = enumObjHash << 16;

                        result |= keyHash;

                        return result;
                }
        }
}
