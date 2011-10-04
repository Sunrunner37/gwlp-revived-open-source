using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerEngine.DataManagement
{
        public static class WrapperExtensions
        {
                public static string ToString<TWrapper, TKey>(this TWrapper wrapper)
                        where TWrapper: IWrapper<TKey>
                {
                        return wrapper.Value.ToString();
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