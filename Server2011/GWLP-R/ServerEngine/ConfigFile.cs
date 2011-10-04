using System.IO;
using System.Xml.Serialization;

namespace ServerEngine
{
        /// <summary>
        ///   This represents the local server config file.
        /// </summary>
        public sealed class ConfigFile
        {
                /// <summary>
                ///   Initializes a new, empty instance of the class.
                /// </summary>
                public ConfigFile()
                {
                }

                /// <summary>
                ///   Initializes a new instance of the class and loads all values from the given config file.
                /// </summary>
                public ConfigFile(string filePath)
                {
                        // Create the XML serializer that will load the config file
                        var xmlSer = new XmlSerializer(typeof(ConfigFile));
                        // Ceate an empty config file that the data will be loaded into
                        var tmpConfig = new ConfigFile();

                        // Get the Config file's data stream
                        using (var strRdr = new StreamReader(filePath))
                        {
                                // Build a temporary config file from the data stream
                                tmpConfig = (ConfigFile)xmlSer.Deserialize(strRdr);

                                // Close the stream and release this object.
                                strRdr.Close();
                        }

                        // Copy the values of the temporary object to this object
                        SrvType = tmpConfig.SrvType;
                        SrvPort = tmpConfig.SrvPort;
                        SrvMaxClients = tmpConfig.SrvMaxClients;
                        LoginSrvIP = tmpConfig.LoginSrvIP;
                        LoginSrvPort = tmpConfig.LoginSrvPort;
                        DataBaseType = tmpConfig.DataBaseType;
                        DataBaseIP = tmpConfig.DataBaseIP;
                        DataBaseName = tmpConfig.DataBaseName;
                        DataBaseUid = tmpConfig.DataBaseUid;
                        DataBasePwd = tmpConfig.DataBasePwd;
                }

                /// <summary>
                ///  This property contains the server type.
                ///  This must not be confused with the type of internal messages!
                ///  For more information see <see c= "ServerTypes"/>.
                /// </summary>
                public byte SrvType { get; set; }

                /// <summary>
                ///   This property contains the port that the server will listen on for incoming connections.
                /// </summary>
                public int SrvPort { get; set; }

                /// <summary>
                ///   This property determines how much connections (of any type) the server will accept.
                /// </summary>
                public int SrvMaxClients { get; set; }

                /// <summary>
                ///   This property contains the IP of the login server that the game server will try to connect to.
                /// </summary>
                public string LoginSrvIP { get; set; }

                /// <summary>
                ///   This property contains the port of the login server that the game server will try to connect to.
                /// </summary>
                public int LoginSrvPort { get; set; }

                /// <summary>
                ///   This property contains the data base server type
                /// </summary>
                public string DataBaseType { get; set; }

                /// <summary>
                ///   This property contains the data base server IP
                /// </summary>
                public string DataBaseIP { get; set; }

                /// <summary>
                ///   This property contains the data base server DataBase name the the server will read from
                /// </summary>
                public string DataBaseName { get; set; }

                /// <summary>
                ///   This property contains the data base server UserID (a. k. a user name)
                /// </summary>
                public string DataBaseUid { get; set; }

                /// <summary>
                ///   This property contains the data base server user password
                /// </summary>
                public string DataBasePwd { get; set; }
        }
}
