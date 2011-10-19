using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ServerEngine.DataManagement.DataInterfaces;
using ServerEngine.DataManagement.DataWrappers;
using ServerEngine.NetworkManagement;

namespace ServerEngine.DataManagement
{
        public class DataManager
        {
                protected Dictionary<Type, MultiKeyDictionary<IEnumerable<IWrapper>>> worldData;

                /// <summary>
                ///   Creates a new instance of the class
                /// </summary>
                protected DataManager()
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
                                var tmpDict = worldData[value.GetType()];

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
                                        worldData.Add(value.GetType(), tmpDict);

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
                                if (!oldValue.GetType().Equals(newValue.GetType())) throw new Exception("Values are not of the same type");

                                // get the right dict
                                var tmpDict = worldData[oldValue.GetType()];

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
                                // the event will trigger the handler later on
                                // so we just need to call:
                                // kick the network interface
                                NetworkManager.Instance.RemoveClient(netID);

                                // message
                                Debug.WriteLine("{0}[{1}] kicked.", value.GetType(), netID.Value);

                                return true;
                        }
                        catch (Exception e)
                        {
                                Debug.WriteLine("Error: {0}[{1}] could not be kicked. {2}", value.GetType(), netID.Value, e.Message);
                                return false;
                        }
                }

                /// <summary>
                ///   This handler should be attached to the NetworkManager's LostClient event, if necessary
                /// </summary>
                /// <param name="netID"></param>
                public virtual void LostNetworkClientHandler(NetID netID)
                {
                        try
                        {
                                foreach (var dict in worldData.Values)
                                {
                                        // get the old value
                                        IEnumerable<IWrapper> tmpValue;
                                        if (dict.TryGetValue(netID, out tmpValue))
                                        {
                                                // remove the old value
                                                dict.RemoveAll(tmpValue);
                                        }
                                }
                        }
                        catch (Exception e)
                        {
                                Debug.WriteLine(string.Format("Error: NetworkClient[{0}] could not be removed, although it has no connection. {1}", netID.Value, e.Message));
                        }
                }

                /// <summary>
                ///   Remove a value from the MultiKeyDictionaries. This will not kick the network interface, if it has any!
                /// </summary>
                protected bool Remove<T>(T value)
                        where T : IEnumerable<IWrapper>
                {
                        try
                        {
                                // get the right dict
                                var tmpDict = worldData[value.GetType()];

                                // remove the value
                                return tmpDict.RemoveAll(value);
                        }
                        // we've got no dict of it ;)
                        catch (KeyNotFoundException)
                        {
                                return false;
                        }
                }
        }
}
