using System.Text;
using CfrSolver.Model;
using CfrSolver.Utils;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace CfrSolver.Datalayer
{
    public class DbWriter
    {
        static DbWriter()
        {
            Batteries_V2.Init();
        }

        const string connectionString = "Data Source=cfr.db;";

        private const int MaxInsertsPerQuery = 500;

        public string DbName { get; }

        public DbWriter(string dbName, Func<bool> dropPrompt = null)
        {
            DbName = dbName;
            using var connection = ConnectToDatabase();
            if (DropTable(connection, DbName, dropPrompt))
            {
                CreateTable(connection, DbName);
            }
            else
            {
                DbName = "nodes" + DateTime.Now.Ticks;
                CreateTable(connection, DbName);
            }
        }

        public void WriteToDb(int hand, Node rootNode, Action<int> nodesWritten = null)
        {
            using SqliteConnection c = ConnectToDatabase();
            using SqliteCommand cmd = new SqliteCommand("", c);
            string initialString =
                $"INSERT INTO {DbName} (Player, Hand, Actions, Round, NextPlayer, Pay, PossibleActions, Cfr) VALUES ";
            StringBuilder sb = new StringBuilder(initialString);
            int i = 0;
            int total = 0;

            try
            {
                // Begin transaction outside the loop
                cmd.CommandText = "BEGIN TRANSACTION;";
                cmd.ExecuteNonQuery();

                NodeTraverser.TraverseWithAction(rootNode, (node, actions) =>
                {
                    string nextPlayer = node.Children.Length > 0 ? node.Children[0].Pos.ToString() : "NULL";
                    string pay = node.IsTerminal() ? node.PayOff.ToString() : "NULL";
                    float[] avStrategy = node.GetAverageStrategy(hand);
                    string cfr = avStrategy.Any()
                        ? $"'{string.Join(";", avStrategy.Select(x => x.ToString("0.0000")))}'"
                        : "NULL";
                    string possibleActions = node.IsTerminal()
                        ? "NULL"
                        : $"'{string.Join(",", node.Children.Select(x => x.Action.ToShortString()))}'";

                    sb.Append(
                        $"({node.Pos}, {hand}, '{actions}', {(int) node.Round}, {nextPlayer}, {pay}, {possibleActions}, {cfr}),");

                    i++;
                    total++;
                    if (i > MaxInsertsPerQuery)
                    {
                        sb[sb.Length - 1] = ';';
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                        sb.Clear();
                        sb.Append(initialString);
                        i = 0;
                        nodesWritten?.Invoke(total);
                    }
                });

                if (sb.Length > 0)
                {
                    sb[sb.Length - 1] = ';';
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                // Commit the transaction at the end
                cmd.CommandText = "COMMIT;";
                cmd.ExecuteNonQuery();
            }

            nodesWritten?.Invoke(total);
        }

        private static void CreateTable(SqliteConnection connection, string dbName)
        {
            using (SqliteCommand cmd = new SqliteCommand($"""
                                                          CREATE TABLE "{dbName}" (
                                                          Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                          Player INTEGER NOT NULL,
                                                          Hand INTEGER NOT NULL,
                                                          Actions TEXT NOT NULL,
                                                          Round INTEGER NOT NULL,
                                                          NextPlayer INTEGER,
                                                          Pay INTEGER,
                                                          PossibleActions TEXT,
                                                          Cfr TEXT

                                                          );
                                                          """, connection))

                //PRIMARY KEY (Player, Hand, Actions, Round)
            {
                cmd.ExecuteNonQuery();
            }

            // Create index separately
            using (SqliteCommand cmd =
                   new SqliteCommand($"""CREATE INDEX IX_Actions_{dbName} ON "{dbName}" (Actions);""", connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private static bool DropTable(SqliteConnection connection, string tableName, Func<bool> dropPrompt)
        {
            var tableExists = TableExists(connection, tableName);
            bool drop = tableExists;

            if (tableExists && dropPrompt != null && !dropPrompt())
            {
                drop = false;
            }

            if (drop)
            {
                using SqliteCommand cmd = new SqliteCommand($"DROP TABLE IF EXISTS `{tableName}`", connection);
                cmd.ExecuteNonQuery();
                return true;
            }

            return !tableExists;
        }

        private static SqliteConnection ConnectToDatabase()
        {
            var mySqlConnection = new SqliteConnection(connectionString);
            mySqlConnection.Open();
            return mySqlConnection;
        }

        private static bool TableExists(SqliteConnection connection, string tableName)
        {
            string query = @"SELECT COUNT(*) FROM sqlite_master 
                            WHERE type='table' AND name=@tableName;";

            using SqliteCommand cmd = new SqliteCommand(query, connection);
            cmd.Parameters.Add(new SqliteParameter("@tableName", SqliteType.Text)).Value = tableName;
            long count = (long) cmd.ExecuteScalar()!;
            return count > 0;
        }
    }
}

/*

CREATE DATABASE IF NOT EXISTS cfr;
USE cfr;

CREATE TABLE `Nodes1` (
`Player` TINYINT NOT NULL,
`Hand` INT (11) NOT NULL,
`Actions` VARCHAR (100) NOT NULL,
`Round` TINYINT NOT NULL,
`NextPlayer` TINYINT,
`Pay` SMALLINT,
`PossibleActions` VARCHAR (100),
`Cfr` VARCHAR (255),
PRIMARY KEY (`Player`, `Hand`, `Actions`, `Round`) ,
INDEX `IX_Actions` (`Actions`)
);

select player, HEX(hand), actions, round, pay, possibleactions, cfr from nodes1
*/