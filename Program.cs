using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using Microsoft.Data.Sqlite;

namespace DatabaseConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Verifica os argumentos de linha de comando
            if (args.Length < 4 || args[0] != "converter" || args[1] != "-i" || args[3] != "-o")
            {
                Console.WriteLine("Uso incorreto! Exemplo: dotnet run converter -i banco.mdb -o banco.db");
                return;
            }

            string inputFilePath = args[2];
            string outputFilePath = args[4];

            // Estabelece a conexão com o banco de dados MDB
            string mdbConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={inputFilePath};";
            using (var mdbConnection = new OleDbConnection(mdbConnectionString))
            {
                mdbConnection.Open();

                // Recupera as tabelas do banco de dados MDB
                DataTable schema = mdbConnection.GetSchema("Tables");
                foreach (DataRow row in schema.Rows)
                {
                    string tableName = row["TABLE_NAME"].ToString();

                    // Recupera o esquema da tabela do banco de dados MDB
                    DataTable tableSchema = mdbConnection.GetSchema("Columns", new string[] { null, null, tableName });

                    // Cria a tabela correspondente no banco de dados SQLite
                    string sqliteConnectionString = $"Data Source={outputFilePath};";
                    using (var sqliteConnection = new SqliteConnection(sqliteConnectionString))
                    {
                        sqliteConnection.Open();

                        using (var createTableCommand = new SqliteCommand(GetCreateTableStatement(tableName, tableSchema), sqliteConnection))
                        {
                            createTableCommand.ExecuteNonQuery();
                        }

                        // Copia os dados da tabela do banco de dados MDB para o banco de dados SQLite
                        using (var selectCommand = new OleDbCommand($"SELECT * FROM [{tableName}]", mdbConnection))
                        {
                            using (var reader = selectCommand.ExecuteReader())
                            {
                                using (var transaction = sqliteConnection.BeginTransaction())
                                {
                                    using (var insertCommand = new SqliteCommand(GetInsertStatement(tableName, tableSchema), sqliteConnection))
                                    {
                                        while (reader.Read())
                                        {
                                            for (int i = 0; i < reader.FieldCount; i++)
                                            {
                                                insertCommand.Parameters.AddWithValue($"@{i}", reader.GetValue(i));
                                            }

                                            insertCommand.ExecuteNonQuery();
                                            insertCommand.Parameters.Clear();
                                        }

                                        transaction.Commit();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Conversão concluída com sucesso!");
        }

        static string GetCreateTableStatement(string tableName, DataTable tableSchema)
        {
            List<string> columns = new List<string>();

            foreach (DataRow row in tableSchema.Rows)
            {
                string columnName = row["COLUMN_NAME"].ToString();
                string dataType = GetSQLiteDataType(row["DATA_TYPE"].ToString());
                bool isNullable = (bool)row["IS_NULLABLE"];

                string columnDefinition = $"{columnName} {dataType}";

                if (!isNullable)
                {
                    columnDefinition += " NOT NULL";
                }

                columns.Add(columnDefinition);
            }

            string createTableStatement = $"CREATE TABLE {tableName} ({string.Join(", ", columns)})";
            return createTableStatement;
        }

        static string GetInsertStatement(string tableName, DataTable tableSchema)
        {
            List<string> columnNames = new List<string>();

            foreach (DataRow row in tableSchema.Rows)
            {
                string columnName = row["COLUMN_NAME"].ToString();
                columnNames.Add(columnName);
            }

            string insertStatement = $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES ({GetInsertValuePlaceholders(tableSchema)})";
            return insertStatement;
        }

        static string GetInsertValuePlaceholders(DataTable tableSchema)
        {
            List<string> valuePlaceholders = new List<string>();

            foreach (DataRow row in tableSchema.Rows)
            {
                valuePlaceholders.Add("@");
            }

            return string.Join(", ", valuePlaceholders);
        }

        static string GetSQLiteDataType(string mdbDataType)
        {
            switch (mdbDataType.ToLower())
            {
                case "bigint":
                case "integer":
                    return "INTEGER";
                case "binary":
                case "varbinary":
                    return "BLOB";
                case "bit":
                case "boolean":
                    return "BOOLEAN";
                case "char":
                case "nchar":
                case "nvarchar":
                case "varchar":
                case "text":
                    return "TEXT";
                case "date":
                case "datetime":
                case "datetime2":
                case "time":
                case "timestamp":
                    return "DATETIME";
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney":
                case "float":
                case "real":
                    return "REAL";
                case "int":
                case "smallint":
                case "tinyint":
                    return "INTEGER";
                case "image":
                    return "BLOB";
                default:
                    return "TEXT";
            }
        }
    }
}
