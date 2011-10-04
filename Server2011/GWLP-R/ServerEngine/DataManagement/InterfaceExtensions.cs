using System.Linq;

namespace ServerEngine.DataManagement
{
        public static class InterfaceExtensions
        {
                public static void Paste<T>(this T source, T dest)
                        where T : class
                {
                        var plist = from prop in typeof(T).GetProperties() 
                                    where prop.CanRead && prop.CanWrite 
                                    select prop;

                        foreach (var prop in plist)
                        {
                                prop.SetValue(source, prop.GetValue(dest, null), null);
                        }
                }
        }
}