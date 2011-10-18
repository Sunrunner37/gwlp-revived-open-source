using ServerEngine.DataManagement.DataWrappers;

namespace ServerEngine.NetworkManagement
{
        /// <summary>
        ///   This delegate represents handler methods for network events concerning a client with the given netID
        /// </summary>
        /// <param name="netID">
        ///   This is to identify the client that triggered the event
        /// </param>
        public delegate void NetworkClientEventHandler(NetID netID);
}