using Microsoft.Data.Sqlite;

namespace CfrSolver.Datalayer
{
    public class DbReader : IDisposable
    {
        const string connectionString = "Data Source=cfr.db;";
        
        private readonly string _dbName;
        private readonly SqliteConnection _connection;

        public DbReader(string dbName)
        {
            _dbName = dbName;
            _connection = ConnectToDatabase();
        }

        public bool GetPossibleActions(int pos, int hand, string actionHistory, out int? nextPos, out string actions, out int round, out string prob,
            out int? pay)
        {
            nextPos = null;
            actions = null;
            round = -1;
            prob = null;
            pay = null;

            using (SqliteCommand cmd =
                new SqliteCommand(
                    $"SELECT * FROM {_dbName} WHERE player = {pos} AND hand = {hand} AND actions = '{actionHistory}'",
                    _connection)
            )
            {
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nextPos = DbIntVal(reader, "NextPlayer");
                        actions = reader["PossibleActions"]?.ToString();
                        round = DbIntVal(reader, "Round").Value;
                        prob = reader["cfr"]?.ToString();
                        pay = DbIntVal(reader, "pay");
                        return true;
                    }
                }
            }
            return false;
        }

        private static int? DbIntVal(SqliteDataReader reader, string field)
        {
            object val = reader[field];
            if (val is DBNull) return null;
            return Convert.ToInt32(reader[field]);
        }


        private SqliteConnection ConnectToDatabase()
        {
            var mySqlConnection = new SqliteConnection(connectionString);
            mySqlConnection.Open();
            return mySqlConnection;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
