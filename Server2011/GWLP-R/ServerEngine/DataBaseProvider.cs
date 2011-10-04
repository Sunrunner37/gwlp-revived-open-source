using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace ServerEngine
{
        public static class DataBaseProvider
        {
                static DataBaseProvider()
                {
                        connection = new MySqlConnection();
                }

                private static IDbConnection connection;
                private static Type dataBaseType;
                private static bool isInitialized;

                public static void InitProvider(IDbConnection dbConnection, Type dataBaseContextType)
                {
                        connection = dbConnection;
                        dataBaseType = dataBaseContextType;

                        isInitialized = true;
                }

                public static object GetDataBase()
                {
                        if (isInitialized)
                        {
                                return Activator.CreateInstance(dataBaseType, new[] {(object) connection});
                        }
                        throw new Exception("Connection was not initialized! Call InitProvider() first!");
                }
        }
}

