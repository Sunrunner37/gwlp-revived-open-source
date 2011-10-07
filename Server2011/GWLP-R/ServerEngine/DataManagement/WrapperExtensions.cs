using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerEngine.DataManagement
{
        public static class WrapperExtensions
        {
                /// <summary>
                ///   More reasonable ToString method for wrapper classes
                /// </summary>
                public static string ToString<TWrapper, TKey>(this TWrapper wrapper)
                        where TWrapper: IWrapper<TKey>
                {
                        return wrapper.Value.ToString();
                }

                /// <summary>
                ///   IEqualityComparer provider for wrapper classes
                /// </summary>
                public static IEqualityComparer<TWrapper> Comparer<TWrapper>(this TWrapper wrapper)
                        where TWrapper: IWrapper
                {
                        return new KeyEqualityComparer<TWrapper>(x => x.Hash());
                }

                public static bool Get<TWrapper>(this IEnumerable<IWrapper> ident, out TWrapper wrapper)
                        where TWrapper : IWrapper
                {
                        try
                        {
                                wrapper = (TWrapper)ident
                                        .TakeWhile(x => (x.GetType() == typeof(TWrapper)))
                                        .FirstOrDefault();

                                return true;
                        }
                        catch (Exception)
                        {
                                wrapper = default(TWrapper);
                                return false;
                        }
                }
        }
}