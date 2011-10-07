using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ServerEngine.DataManagement.DataInterfaces;
using ServerEngine.NetworkManagement;

namespace ServerEngine.DataManagement
{
        public class World
        {
                protected Dictionary<Type, MultiKeyDictionary<IEnumerable<IWrapper>>> worldData;

                /// <summary>
                ///   Creates a new instance of the class
                /// </summary>
                public World()
                {
                        worldData = new Dictionary<Type, MultiKeyDictionary<IEnumerable<IWrapper>>>();
                }

                /// <summary>
                ///   Try to add a value of 'T'
                /// </summary>
                public bool Add<T>(T value)
                        where T : IEnumerable<IWrapper>
                {
                        try
                        {
                                // get the right dict
                                var tmpDict = worldData[typeof(T)];

                                // add the value
                                return tmpDict.AddAll(value);
                        }
                        // we've got no dict of it ;)
                        catch (KeyNotFoundException)
                        {
                                // create a new dict
                                var tmpDict = new MultiKeyDictionary<IEnumerable<IWrapper>>();

                                // add the value
                                if (tmpDict.AddAll(value))
                                {
                                        // add the dict
                                        worldData.Add(typeof(T), tmpDict);

                                        return true;
                                }

                                return false;
                        }

                }

                /// <summary>
                ///   Try to get a value of type 'T' which should be an IEnumerable of IWrapper
                /// </summary>
                public T Get<T>(IWrapper key)
                        where T : IEnumerable<IWrapper>
                {
                        try
                        {
                                // get the right dict
                                var tmpDict = worldData[typeof(T)];

                                // create a tmp value
                                IEnumerable<IWrapper> result;
                                if (tmpDict.TryGetValue(key, out result))
                                {
                                        return (T)result;
                                }

                                return default(T);
                        }
                        catch (Exception)
                        {
                                return default(T);
                        }

                }

                /// <summary>
                ///   Try to get all values of type 'T' which should be an IEnumerable of IWrapper
                /// </summary>
                public IEnumerable<T> GetAll<T>()
                        where T : IEnumerable<IWrapper>
                {
                        try
                        {
                                // get the right dict
                                var tmpDict = worldData[typeof(T)];

                                return tmpDict.Values.Select(x => (T)x);
                        }
                        catch (Exception)
                        {
                                return new List<T>();
                        }

                }

                /// <summary>
                ///   Try to replace a value of 'T' with a new one
                /// </summary>
                public bool Update<T>(T oldValue, T newValue)
                        where T : IEnumerable<IWrapper>
                {
                        try
                        {
                                // get the right dict
                                var tmpDict = worldData[typeof(T)];

                                // remove the old value and add the new one
                                return tmpDict.RemoveAll(oldValue) && tmpDict.AddAll(newValue);
                        }
                        catch (Exception)
                        {
                                return false;
                        }
                }

                /// <summary>
                ///   Try to kick a value that implements IIdentifiableData and IHasNetworkData
                /// </summary>
                public bool Kick<TData>(IIdentifiableData<TData> value)
                        where TData : class, IHasNetworkData
                {
                        var netID = value.Data.NetID;

                        try
                        {
                                // get the right dict
                                var tmpDict = worldData[value.GetType()];

                                // remove the old value
                                tmpDict.RemoveAll(value as IEnumerable<IWrapper>);

                                // kick the network interface
                                NetworkManager.Instance.RemoveClient(netID);

                                // message
                                Debug.WriteLine("{0}[{1}] kicked.", value.GetType(), netID.Value);

                                return true;
                        }
                        catch (Exception)
                        {
                                Debug.WriteLine("Error: {0}[{1}] could not be kicked.", value.GetType(), netID.Value);
                                return false;
                        }
                }
        }
}
