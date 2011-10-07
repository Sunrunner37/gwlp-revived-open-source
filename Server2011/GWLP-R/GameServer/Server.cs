using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using GameServer.DataBase;
using GameServer.Modules;
using GameServer.Packets.ToLoginServer;
using GameServer.ServerData;
using MySql.Data.MySqlClient;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer
{
        static class Server
        {
                private static readonly ConfigFile localConfig;

                private static readonly PacketManager packetMan;

                private static readonly List<Action> serverTasks;

                private static readonly string initFail = "";

                /// <summary>
                ///   Creates a new instance of the class
                /// </summary>
                static Server()
                {
                        try
                        {
                                // Change console stats
                                #if !MONO_STRICT
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.Clear();
                                        Console.SetWindowSize(Console.WindowWidth + 25, Console.WindowHeight);
                                #endif


                                // Init the debug writers
                                Debug.AutoFlush = true;
                                Debug.IndentSize = 4;
                                var consoleWriter = new TextWriterTraceListener(Console.Out);
                                var fileWriter = new TextWriterTraceListener(Properties.Settings.Default.LogFile);
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
                                Debug.Write("Gathering local config file data...   ");
                                localConfig = new ConfigFile(Properties.Settings.Default.ConfigFile);
                                GameServerWorld.Instance.LocalConfig = localConfig;

                                Debug.WriteLine("[done]");


                                // Init the db connection
                                Debug.Write("Initializing database provider...     ");
                                DataBaseProvider.InitProvider(new MySqlConnection(
                                        "server=" + localConfig.DataBaseIP +
                                        ";database=" + localConfig.DataBaseName +
                                        ";uid=" + localConfig.DataBaseUid +
                                        ";pwd=" + localConfig.DataBasePwd + ";"),
                                        typeof(MySQL));

                                Debug.WriteLine("[done]");


                                // Load the packet manager
                                Debug.Write("Creating packet manager...            ");
                                var packets = Assembly.GetExecutingAssembly().GetTypes().Where(type => ((typeof(IPacket)).IsAssignableFrom(type))).ToList();
                                packetMan = new PacketManager(packets.ToArray());

                                Debug.WriteLine("[done]");
                                

                                // Init the network manager
                                Debug.Write("Creating network manager...           ");
                                NetworkManager.Instance.Init(localConfig.SrvMaxClients);
                                NetworkManager.Instance.StartListeners(localConfig.SrvPort);

                                Debug.WriteLine("[done]");

                                // Try to create a login server connection
                                Debug.Write("Creating login server connection...   ");
                                var netID = NetworkManager.Instance.CreateConnection(localConfig.LoginSrvIP, localConfig.LoginSrvPort);
                                
                                // Note: HANDSHAKE REQUEST
                                var handshake = new NetworkMessage(netID)
                                {
                                        PacketTemplate = new P65281_HandshakeRequest.PacketSt65281()
                                        {
#warning SECURITY: Not implemented server security keys:
                                                SecurityKey1 = new byte[8],
                                                SecurityKey2 = new byte[8],
                                                Port = (uint)localConfig.SrvPort,
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(handshake);

                                // dont forget to save the netID for communication later on
                                GameServerWorld.Instance.LoginSrvNetID = netID;

                                Debug.WriteLine("[done]");


                                // Try to load the pmaps (init movement)
                                Debug.WriteLine("Loading pathing maps...               ");
                                var movement = new Movement(Properties.Settings.Default.PathingMapsDir);
                                Debug.WriteLine("All maps processed");


                                // Init the server tasks
                                Debug.Write("Registering server tasks...           ");
                                serverTasks = new List<Action>
                                {
                                        // core features:
                                        packetMan.ProcessPackets,
                                        NetworkManager.Instance.MainTask,
                                        // modules:
                                        new ActionQueue().Execute,
                                        new HeartBeat().Execute,
                                        new Ping().Execute,
                                        movement.Execute
                                };

                                Debug.WriteLine("[done]");


                                // Load the message of the day, if there is any
                                Debug.Write("Loading the message of the day...     ");

                                var defaultMessage = new string[]
                                {
                                        "You'r playing on:",
                                        "< --[GWLP:R alpha v." + Assembly.GetExecutingAssembly().GetName().Version + "]-- >",
                                        "< --[Credits] - [ _rusty ] [ ACB ] [ miracle444 ] [ onyxphase ]-- >"
                                };

                                if (File.Exists(Properties.Settings.Default.MotdFile))
                                {
                                        var lines = File.ReadAllLines(Properties.Settings.Default.MotdFile);

                                        // check the length of each line
                                        var tooLong = from s in lines
                                                      where s.Length >= 56
                                                      select s;

                                        // we only take 5 lines, each max 56 characters!
                                        if ((lines.Length <= 5) && (tooLong.Count() == 0))
                                        {
                                                GameServerWorld.Instance.MessageOfTheDay = lines;
                                        }
                                        else
                                        {
                                                Debug.WriteLine("[error in msg format, using default]");
                                                GameServerWorld.Instance.MessageOfTheDay = defaultMessage;
                                        }
                                }
                                else
                                {
                                        GameServerWorld.Instance.MessageOfTheDay = defaultMessage;
                                }

                                Debug.WriteLine("[done]");

                        }
                        catch (Exception e)
                        {
                                initFail = e.ToString();
                        }
                }

                static void Main(string[] args)
                {
                        // Check cmd line params

                        // Init-Failcheck
                        if (initFail != "")
                        {
                                Debug.WriteLine(" ");
                                Debug.WriteLine("ERROR: " + initFail);
                                Debug.WriteLine(" ");
                                Debug.WriteLine("Terminating application");
                                Debug.WriteLine("- press any key to continue -");
                                Console.ReadKey();
                                Environment.Exit(1);
                        }
                        Debug.WriteLine(" ");

                        // Main loop here...);
                        while (true)
                        {
                                try
                                {
                                        // execute all subscribers in the server task list
#warning PERFORMANCE This is blocking! All threads are being executed only once per cycle!
                                        serverTasks.AsParallel().ForAll(action => action());

#warning PERFORMANCE This may be left out depending on the system.
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