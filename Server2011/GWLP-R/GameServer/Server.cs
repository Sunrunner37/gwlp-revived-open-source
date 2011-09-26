using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using GameServer.DataBase;
using GameServer.Modules;
using GameServer.Packets.ToLoginServer;
using GameServer.ServerData;
using MySql.Data.MySqlClient;
using ServerEngine;
using ServerEngine.DataBase;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement;
using ServerEngine.OfflineSettings;
using ServerEngine.PacketManagement.Definitions;
using ServerEngine.ProcessorQueues;

namespace GameServer
{

        class Server
        {
                private ConfigFile localConfig;

                private static PacketManager packetMan;

                const string ConfigFile = "GameConfig.xml";
                const string LogFile = "GameLog.xml";

                private static List<Action> serverTasks;

                /// <summary>
                ///   This creates the most necessary objects.
                /// </summary>
                /// <returns>
                ///   Returns false if something fails to initialize
                /// </returns>
                private bool Initialize()
                {
                        try
                        {
                                // Init the debug writers
                                Debug.AutoFlush = true;
                                Debug.IndentSize = 4;
                                var consoleWriter = new TextWriterTraceListener(Console.Out);
                                var fileWriter = new TextWriterTraceListener(LogFile);
                                Debug.Listeners.Add(consoleWriter);
                                Debug.Listeners.Add(fileWriter);

                                Debug.WriteLine(" ");
                                Debug.WriteLine(" ");
                                Debug.WriteLine("Initialising Server...");
                                Debug.WriteLine(DateTime.Now);
                                Debug.WriteLine(" ");

                                // Hello message ;)
                                Debug.WriteLine(@"  _______________________________________");
                                Debug.WriteLine(@" /                                       \");
                                Debug.WriteLine(@"<          --[ GWLP: Revived ]--          >");
                                Debug.WriteLine(@" \_______________________________________/");
                                Debug.WriteLine(@"  < Serv:    [" + Assembly.GetExecutingAssembly().GetName().Name + "] >");
                                Debug.WriteLine(@"  < Vers:    [" + Assembly.GetExecutingAssembly().GetName().Version + "] >");
                                Debug.WriteLine(@"  -");
                                Debug.WriteLine(@"  < By [ _rusty ] [ ACB ] [ miracle444 ] [ onyxphase ] >");
                                Debug.WriteLine(@"  -");
                                Debug.WriteLine(@"  < [www.GameRevision.com] >");
                                Debug.WriteLine(" ");

                                Debug.Indent();
                                Debug.WriteLine(" ");

                                // Init the local config class
                                //Debug.Listeners
                                Debug.Write("Gathering local config file data...");
                                localConfig = new ConfigFile(ConfigFile);

                                Debug.WriteLine("\t\t[done]");

                                // Init the db connection
                                Debug.Write("Initializing database provider...");
                                DataBaseProvider.InitProvider(new MySqlConnection(
                                        "server=" + localConfig.DataBaseIP +
                                        ";database=" + localConfig.DataBaseName +
                                        ";uid=" + localConfig.DataBaseUid +
                                        ";pwd=" + localConfig.DataBasePwd + ";"),
                                        typeof(MySQL));

                                Debug.WriteLine("\t\t[done]");

                                // Load the packet manager
                                Debug.Write("Creating packet manager...");
                                var packets = Assembly.GetExecutingAssembly().GetTypes().Where(type => ((typeof(IPacket)).IsAssignableFrom(type))).ToList();
                                packetMan = new PacketManager(packets.ToArray());

                                Debug.WriteLine("\t\t\t[done]");
                                

                                // Init the network manager
                                Debug.Write("Creating network manager...");
                                NetworkManager.Instance.Init(localConfig.SrvMaxClients);
                                NetworkManager.Instance.StartListeners(localConfig.SrvPort);

                                Debug.WriteLine("\t\t\t[done]");

                                // Try to create a login server connection
                                Debug.Write("Creating login server connection...");
                                var netID = NetworkManager.Instance.CreateConnection(localConfig.LoginSrvIP, localConfig.LoginSrvPort);
                                var handshake = new NetworkMessage(netID)
                                                        {
                                                                PacketTemplate = new P65281_HandshakeRequest.PacketSt65281()
                                                        };
#warning Not implemented server security keys:
                                ((P65281_HandshakeRequest.PacketSt65281)handshake.PacketTemplate).SecurityKey1 = new byte[8];
                                ((P65281_HandshakeRequest.PacketSt65281)handshake.PacketTemplate).SecurityKey2 = new byte[8];
                                ((P65281_HandshakeRequest.PacketSt65281)handshake.PacketTemplate).Port = (uint)localConfig.SrvPort;
                                QueuingService.PostProcessingQueue.Enqueue(handshake);

                                // dont forget to save the netID for communication later on
                                World.LoginSrvNetID = netID;

                                Debug.WriteLine("\t\t[done]");

                                // Init the server tasks
                                Debug.Write("Registering server tasks...");
                                serverTasks = new List<Action>
                                                      {
                                                              // core features:
                                                              packetMan.ProcessPackets,
                                                              NetworkManager.Instance.MainTask,
                                                              // modules:
                                                              new ActionQueue().Execute,
                                                              new HeartBeat().Execute,
                                                              new Ping().Execute,
                                                              new Movement(@"PMAPs\").Execute
                                                      };

                                Debug.WriteLine("\t\t\t[done]");

                        }
                        catch (Exception e)
                        {
                                Debug.Fail(e.ToString());

                                return false;
                        }

                        return true;
                }

                static void Main(string[] args)
                {
                        // Check cmd line params

                        // Change console stats
                        // Note:This must be left out when using >Mono<
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Clear();
                        Console.SetWindowSize(Console.WindowWidth + 25, Console.WindowHeight);

                        // Init the server
                        var server = new Server();
                        // Note: Server specific data here
                        if (!server.Initialize())
                        {
                                Debug.WriteLine(" ");
                                Debug.WriteLine("Terminating application");
                                Debug.WriteLine("- press any key to continue -");
                                Console.ReadKey();
                                Environment.Exit(1);
                        }

                        // Main loop here...);
                        while (true)
                        {
                                try
                                {
                                        // execute all subscribers in the server task list
                                        serverTasks.AsParallel().ForAll(action => action());

#warning Let the CPU have a pause
                                        System.Threading.Thread.Sleep(1);
                                }
                                catch (Exception e)
                                {
                                        Debug.WriteLine(e.Message);
                                }  
                        }
                }
        }
}