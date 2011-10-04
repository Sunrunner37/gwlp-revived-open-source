using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ServerEngine.DataManagement
{
        public class MultiKeyDictionary<TValue>
                where TValue: IEnumerable<IWrapper>
        {
                private readonly Dictionary<Type, Dictionary<int, TValue>> dicts;

                /// <summary>
                ///   Creates a new instance of the class.
                /// </summary>
                public MultiKeyDictionary()
                {
                        dicts = new Dictionary<Type, Dictionary<int, TValue>>();
                }

                /// <summary>
                ///   Adds a new value to the multikey dictionary
                /// </summary>
                /// <typeparam name="TKey">
                ///   A wrapper-struct for a simple datatype
                /// </typeparam>
                public bool AddOnly<TKey>(TKey key, TValue value)
                        where TKey: IWrapper
                {
                        Contract.Requires(key != null);
                        Contract.Requires(value != null);

                        lock (dicts)
                        {
                                Dictionary<int, TValue> tmpDict;

                                try
                                {
                                        // try to get the right dictionary
                                        tmpDict = dicts[typeof(TKey)];

                                        // add the new dictionary entry
                                        tmpDict.Add(key.Hash(), value);

                                        return true;
                                }
                                // the dictionary for this type does not yet exist
                                catch (KeyNotFoundException)
                                {
                                        // add the new dictionary entry
                                        tmpDict = new Dictionary<int, TValue> {{key.Hash(), value}};

                                        // and add the new dictionary
                                        dicts.Add(typeof(TKey), tmpDict);

                                        return true;
                                }
                                // the value already exists within the sub-dictionary
                                catch (ArgumentException)
                                {
                                        // we suppress this expection, because we do not want to add try catch blocks later on
                                        return false;
                                }

                                // note that argument null exceptions still will be passed
                        }
                }

                /// <summary>
                ///   Adds a new value to the multikey dictionary,
                ///   with all its references.
                /// </summary>
                public bool AddAll(TValue value)
                {
                        Contract.Requires(value != null);

                        lock (dicts)
                        {
                                try
                                {
                                        var result = true;

                                        foreach (var key in value.AsEnumerable())
                                        {
                                                if (key == null) continue;
                                                if (!AddOnly(key, value))
                                                {
                                                        result = false;
                                                }
                                        }

                                        return result;
                                }
                                // any exception occured
                                catch (Exception e)
                                {
                                        Debug.Fail(e.ToString());
                                        return false;
                                }
                        }
                }

                /// <summary>
                ///   Tries to get a value from a key.
                /// </summary>
                /// <param name="key">
                ///   A wrapper-struct for a simple datatype
                /// </param>
                public bool TryGetValue<TKey>(TKey key, out TValue value)
                        where TKey: IWrapper
                {
                        lock (dicts)
                        {
                                try
                                {
                                        // try to get the right dictionary
                                        var tmpDict = dicts[typeof(TKey)];

                                        // retrieve the value
                                        return tmpDict.TryGetValue(key.Hash(), out value);
                                }
                                // the dictionary for this type does not yet exist
                                catch (KeyNotFoundException)
                                {
                                        value = default(TValue);
                                        return false;
                                }

                                // note that argument null exceptions still will be passed
                        }
                }

                /// <summary>
                ///   Removes a single reference (key) to a value from the dictionary.
                ///   The value still may be found by other references.
                /// </summary>
                public bool RemoveOnly<TKey>(TKey key)
                        where TKey : IWrapper
                {
                        lock (dicts)
                        {
                                try
                                {
                                        // try to get the right dictionary
                                        var tmpDict = dicts[typeof(TKey)];

                                        // remove the value
                                        return tmpDict.Remove(key.Hash());
                                }
                                // the dictionary for this type does not yet exist
                                catch (KeyNotFoundException)
                                {
                                        return false;
                                }

                                // note that argument null exceptions still will be passed
                        }
                }

                /// <summary>
                ///   Removes all references to a value from the dictionary.
                ///   The value is searched with the parameter.
                /// </summary>
                public bool RemoveAll<TKey>(TKey key)
                        where TKey : IWrapper
                {
                        lock (dicts)
                        {
                                try
                                {
                                        // try to get the value
                                        TValue tmpValue;
                                        return TryGetValue(key, out tmpValue) && RemoveAll(tmpValue);
                                }
                                // the dictionary for this type does not yet exist
                                catch (KeyNotFoundException)
                                {
                                        return false;
                                }

                                // note that argument null exceptions still will be passed
                        }
                }

                /// <summary>
                ///   Removes all references to a value from the dictionary.
                /// </summary>
                public bool RemoveAll(TValue value)
                {
                        lock (dicts)
                        {
                                try
                                {
                                        var result = true;

                                        foreach (var key in value
                                                .AsEnumerable()
                                                .Where(key => !RemoveOnly(key)))
                                        {
                                                result = false;
                                        }

                                        return result;
                                }
                                // any exception occured
                                catch (Exception)
                                {
                                        return false;
                                }
                        }
                }
                
                /// <summary>
                ///   This property contains all values (without redundancies)
                /// </summary>
                public IEnumerable<TValue> Values
                {
                        get
                        {
                                // NOTE: BUG: THE FOLLOWING NEEDS TO RETURN AN ENUMERATION THAT WILL NOT UPDATE WHEN dict.Values CHANGES!
                                var values = new List<TValue>();
                                
                                // get all values
                                foreach (var dict in dicts.Values)
                                {
                                        values.AddRange(dict.Values);
                                }

                                // remove redundant data
                                values.Distinct(new KeyEqualityComparer<TValue>(x => x.GetEnumerator().Current.ToString()));
                                
                                return values.ToList();
                        }
                }

                #region Nested Class: Distinct Helper Class

                public class KeyEqualityComparer<T> : IEqualityComparer<T>
                {
                        private readonly Func<T, object> keyExtractor;

                        public KeyEqualityComparer(Func<T, object> keyExtractor)
                        {
                                this.keyExtractor = keyExtractor;
                        }

                        public bool Equals(T x, T y)
                        {
                                return this.keyExtractor(x).Equals(this.keyExtractor(y));
                        }

                        public int GetHashCode(T obj)
                        {
                                return this.keyExtractor(obj).GetHashCode();
                        }
                }

                #endregion
        }
}
