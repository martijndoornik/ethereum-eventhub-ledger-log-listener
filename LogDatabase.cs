using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using AzureEventhubProtocol.Receive;

namespace LogListener
{
    public sealed class LogDatabase
    {
        private readonly string _connectionString;

        private static LogDatabase _instance;

        public static LogDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new NullReferenceException("Instance of LogDatabase has not been set yet. Use LogDatabase.Connect(args) before calling for instance");
                }
                return _instance;
            }
        }

        private SqlConnection SqlConnection
        {
            get => new SqlConnection(_connectionString);
        }

        private LogDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static LogDatabase Connect(string connectionString)
        {
            if (_instance != null)
            {
                _instance = new LogDatabase(connectionString);
            }
            return _instance;
        }

        public static LogDatabase Connect(string[] args)
        {
            if (_instance == null)
            {
                string connectionString = null;
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i] != null
                        && (args[i].Equals("-dbcs")
                            || args[i].Equals("--databaseConnectionString"))
                        && i + 1 < args.Length)
                    {
                        connectionString = args[i + 1];
                        i++;
                    }
                }
                if (connectionString == null)
                {
                    throw new ArgumentNullException("connectionString", "Could not connect to LogDatabase without ConnectionString");
                }
                _instance = new LogDatabase(connectionString);
            }
            return _instance;
        }

        public async Task SaveEvent(EventMessage message)
        {
            var query = "BEGIN " +
                "IF NOT EXISTS (SELECT * FROM [dbo.Events] " +
                $"WHERE id = {message.Id}) " +
                "BEGIN " +
                "INSERT INTO [dbo.Events] (id, name, body, date, listener, listener_version) " +
                $"VALUES ({message.Id}, '{message.EventName}', '{message.RawBody}', '{message.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}', '{message.Listener}', '{message.ListenerVersion}') " +
                "END END";
            using (var connection = SqlConnection)
            {
                await connection.OpenAsync();
                var transaction = connection.BeginTransaction();
                var command = new SqlCommand(query, connection, transaction);
                await command.ExecuteNonQueryAsync();
                transaction.Commit();
            }
        }
    }
}
