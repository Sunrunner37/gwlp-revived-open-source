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
                private readonly object objLock = new object();

                private readonly Dictionary<ulong, TValue> dicts;
                private readonly Dictionary<TValue, int> values;

                /// <summary>
                ///   Creates a new instance of the class.
                /// </summary>
                public MultiKeyDictionary()
                {
                        dicts = new Dictionary<ulong, TValue>();
                        values = new Dictionary<TValue, int>();
                }

                /// <summary>
                ///   Adds a new value to the multikey dictionary
                /// </summary>
                /// <typeparam name="TKey">
                ///   A wrapper-struct for a simple datatype
                /// </typeparam>
                public bool AddOnly<TKey>(TKey key, TValue value)
                        where TKey: class, IWrapper
                {
                        lock (objLock)
                        {
                                try
                                {
                                        // add the new dictionary entry
                                        dicts.Add(key.Hash(), value);

                                        // add the values stuff
                                        if (values.ContainsKey(value))
                                        {
                                                // increase the reference counter
                                                values[value]++;
                                        }
                                        else
                                        {
                                                values.Add(value, 1);
                                        }

                                        return true;
                                }
                                // the value already exists within the dictionary
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
                        lock (objLock)
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

                                                        break;
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
                        where TKey: class, IWrapper
                {
                        lock (objLock)
                        {
                                // retrieve the value
                                return dicts.TryGetValue(key.Hash(), out value);

                                // note that argument null exceptions still will be passed
                        }
                }

                /// <summary>
                ///   Removes a single reference (key) to a value from the dictionary.
                ///   The value still may be found by other references.
                /// </summary>
                public bool RemoveOnly<TKey>(TKey key)
                        where TKey : class, IWrapper
                {
                        lock (objLock)
                        {
                                try
                                {
                                        // get the value
                                        var value = dicts[key.Hash()];

                                        // do the values stuff
                                        if (values.ContainsKey(value))
                                        {
                                                values[value]--;
                                                if (values[value] == 0) values.Remove(value);
                                        }

                                        // remove the value
                                        return dicts.Remove(key.Hash());
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
                        lock (objLock)
                        {
                                try
                                {
                                        var result = true;

                                        // note that null reference keys are neither added nor removed ;)
                                        foreach (var key in value
                                                .AsEnumerable()
                                                .Where(key => key != null && !RemoveOnly(key)))
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
                                lock (objLock)
                                {
                                        // NOTE: THE FOLLOWING NEEDS TO RETURN AN ENUMERATION THAT WILL NOT UPDATE WHEN dict.Values CHANGES!
                                        return values.Keys;
                                }
                        }
                }
        }
}
