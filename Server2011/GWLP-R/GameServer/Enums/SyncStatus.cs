namespace GameServer.Enums
{
        public enum SyncStatus
        {
                Unauthorized,
                ConnectionEstablished,
                EncryptionEstablished,
                TriesToLoadInstance,
                Playing,
                Dispatching
        }
}
