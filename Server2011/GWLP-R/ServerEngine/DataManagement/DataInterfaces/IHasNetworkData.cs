using ServerEngine.DataManagement.DataWrappers;

namespace ServerEngine.DataManagement.DataInterfaces
{
        public interface IHasNetworkData
        {
                NetID NetID { get; set; }
                IPAddress IPAddress { get; set; }
                Port Port { get; set; }
        }
}