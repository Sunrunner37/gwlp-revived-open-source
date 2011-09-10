using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoginServer.Enums
{
        public enum SyncState
        {
                ConnectionEstablished,
                EncryptionEstablished,
                UpdateClientLogin,
                AtCharView,
                TriesToLoadInstance,
                InGame,
                InCharCreation,
                PossibleQuit,
        }
}
