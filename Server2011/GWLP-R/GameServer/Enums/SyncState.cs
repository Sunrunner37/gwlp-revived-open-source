namespace GameServer.Enums
{
        public enum SyncState
        {
                Unauthorized,
                ConnectionEstablished,
                EncryptionEstablished,
                TriesToLoadInstance,
                Playing,
                Dispatching
        }
}
