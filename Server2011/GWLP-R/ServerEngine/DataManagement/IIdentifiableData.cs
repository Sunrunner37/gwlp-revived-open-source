using System.Collections.Generic;

namespace ServerEngine.DataManagement
{
        public interface IIdentifiableData<TData> : IEnumerable<IWrapper>
                where TData: class
        {
                TData Data { get; set; }
        }
}