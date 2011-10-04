using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LoginServer.DataBase;
using MySql.Data.MySqlClient;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer
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
                                        Console.ForegroundColor = ConsoleColor.Blue;
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

                                // Init the server tasks
                                Debug.Write("Registering server tasks...           ");
                                serverTasks = new List<Action>
                                                      {
                                                              // core features:
                                                              packetMan.ProcessPackets,
                                                              NetworkManager.Instance.MainTask,
                                                      };

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