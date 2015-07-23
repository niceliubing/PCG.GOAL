using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccess.Common.Services
{
    public class SqlDataAccess :  IDisposable
    {
        private const string SqlConnectionKey = "SQLConn";
        public SqlDataAccess()
        {
            try
            {
                Connection = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlConnectionKey].ToString());
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Failed to read connection string '{0}'", SqlConnectionKey), e);
            }
        }

        public SqlDataAccess(SqlConnection connection)
        {
            this.Connection = connection;
        }
        public SqlConnection Connection { get; set; }
        protected SqlTransaction Transaction { get; set; }

        public void OpenConnection()
        {
            if (this.Connection == null)
            {
                throw new Exception("Connection has not been initialized!");
            }

            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
            }
        }

        private SqlCommand NewCommand(string storedPorc, CommandType commandType = CommandType.StoredProcedure)
        {
            var command = new SqlCommand
            {
                Connection = this.Connection,
                CommandType = commandType,
                CommandText = storedPorc,
                CommandTimeout = 400000
            };
            if (this.Transaction != null)
                command.Transaction = this.Transaction;
            return command;
        }

        public SqlDataReader GetReader(string storedProcedureKey, List<SqlParameter> parameters, bool closeConnection = true)
        {
            this.OpenConnection();

            using (SqlCommand command = this.NewCommand(storedProcedureKey))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }

                return closeConnection ? command.ExecuteReader(CommandBehavior.CloseConnection) : command.ExecuteReader();
            }
        }

        public SqlDataReader GetReaderBySql(string sqlStatement, bool closeConnection = true)
        {
            this.OpenConnection();

            using (SqlCommand command = this.NewCommand(sqlStatement, CommandType.Text))
            {
                return closeConnection ? command.ExecuteReader(CommandBehavior.CloseConnection) : command.ExecuteReader();
            }
        }

        public DataTable GetTableBySql(string sqlStatement, bool closeConnection = true)
        {
            this.OpenConnection();

            using (SqlCommand command = NewCommand(sqlStatement, CommandType.Text))
            {
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                var table = new DataTable();
                table.Load(reader);
                return table;
            }
        }
        public object GetScalar(string storedProcedureKey, List<SqlParameter> parameters, bool closeConnection = true)
        {
            this.OpenConnection();

            using (SqlCommand command = this.NewCommand(storedProcedureKey))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }

                var ret = command.ExecuteScalar();

                if (closeConnection)
                {
                    this.Connection.Close();
                }

                return ret;
            }
        }
        public object GetScalar(string sqlStatement, bool closeConnection = true)
        {
            this.OpenConnection();

            using (SqlCommand command = this.NewCommand(sqlStatement, CommandType.Text))
            {
                var ret = command.ExecuteScalar();

                if (closeConnection)
                {
                    this.Connection.Close();
                }

                return ret;
            }

        }
        public int ExecuteNonQuery(string storedProcedureKey, List<SqlParameter> parameters, bool closeConnection = true)
        {
            this.OpenConnection();

            using (SqlCommand command = this.NewCommand(storedProcedureKey))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }

                int ret = command.ExecuteNonQuery();

                if (parameters != null)
                {
                    foreach (SqlParameter p in command.Parameters)
                    {
                        if (p.Direction == ParameterDirection.Output)
                        {
                            parameters.First(pr => pr.ParameterName == p.ParameterName).Value = p.Value;
                        }
                    }
                }

                if (closeConnection)
                {
                    this.Connection.Close();
                }

                return ret;
            }
        }

        public int ExecuteNonQuery(string sqlStatement, bool closeConnection = true)
        {
            this.OpenConnection();

            using (SqlCommand command = this.NewCommand(sqlStatement, CommandType.Text))
            {
                int ret = command.ExecuteNonQuery();

                if (closeConnection)
                {
                    this.Connection.Close();
                }

                return ret;
            }
        }

        public void BulkCopy(string targetTable, DataTable data, bool closeConnection = true, Dictionary<string, string> mappings = null)
        {
            this.OpenConnection();

            using (var bulkCopy = new SqlBulkCopy(this.Connection))
            {
                bulkCopy.DestinationTableName = targetTable;
                if (mappings == null)
                {
                    for (var i = 0; i < data.Columns.Count; ++i)
                    {
                        bulkCopy.ColumnMappings.Add(data.Columns[i].ColumnName, data.Columns[i].ColumnName);
                    }
                }
                else
                {
                    foreach (var pair in mappings)
                    {
                        bulkCopy.ColumnMappings.Add(pair.Value, pair.Key);
                    }
                }


                bulkCopy.WriteToServer(data);
            }
            if (closeConnection)
                this.Connection.Close();
        }

        protected T GetSafeValue<T>(IDataRecord record, string field)
        {
            if (record[field] is DBNull)
                return default(T);
            return (T)record[field];
        }

        protected T? GetSafeNullable<T>(IDataRecord record, string field) where T : struct
        {
            if (record[field] is DBNull)
                return null;
            return (T?)record[field];
        }

        public static string GetSafeString(IDataRecord record, string field)
        {
            if (record[field] is DBNull)
                return null;
            return record[field].ToString();
        }

        public void Dispose()
        {
            if (this.Connection != null)
            {
                if (this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                }

                this.Connection.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
