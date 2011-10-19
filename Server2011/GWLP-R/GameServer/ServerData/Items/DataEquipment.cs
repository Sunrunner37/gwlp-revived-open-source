using System;
using System.Collections;
using System.Collections.Generic;

namespace GameServer.ServerData.Items
{
    public class DataEquipment : IEnumerable<Item>
    {
        // holds many items

        #region Implementation of IEnumerable

        public IEnumerator<Item> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}