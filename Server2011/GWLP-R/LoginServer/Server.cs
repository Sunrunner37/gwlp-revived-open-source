using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LoginServer.DataBase;
using LoginServer.ServerData;
using MySql.Data.MySqlClient;
using ServerEngine;
using ServerEngine.DataBase;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement;
using ServerEngine.OfflineSettings;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer
{

        class Server
        {
                private ConfigFile localConfig;

                private static PacketManager packetMan;

                const string ConfigFile = "LoginConfig.xml";
                const string LogFile = "LoginLog.xml";

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
                                Debug.WriteLine("##################################");
                                Debug.WriteLine("#       --[GWLP: Revived]--      #");
                                Debug.WriteLine("# GAME Server, v3.0.1            #");
                                Debug.WriteLine("##################################");
                                Debug.WriteLine("credits to _rusty, ACB, onyxphase");
                                Debug.WriteLine("visit gamerevision.com");
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
                                NetworkManager.Instance.MaxClients = localConfig.SrvMaxClients;
                                NetworkManager.Instance.StartListeners(localConfig.SrvPort);

                                Debug.WriteLine("\t\t\t[done]");

                                // Init the server tasks
                                Debug.Write("Registering server tasks...");
                                serverTasks = new List<Action>
                                                      {
                                                              // core features:
                                                              packetMan.ProcessPackets,
                                                              NetworkManager.Instance.MainTask,
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
                        Console.ForegroundColor = ConsoleColor.Blue;
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

                        // Main loop here...
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
                                        Debug.WriteLine(e.ToString());
                                }
                        }
                }
        }
}