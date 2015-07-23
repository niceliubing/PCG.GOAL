using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PCG.GOAL.Common.Interface
{
    public interface ISqlDataAccess
    {
        SqlDataReader GetReaderBySql(string sqlStatement, bool closeConnection = true);
        SqlDataReader GetReader(string storedProcedureKey, List<SqlParameter> parameters, bool closeConnection = true);
        object GetScalar(string storedProcedureKey, List<SqlParameter> parameters, bool closeConnection = true);
        object GetScalar(string sqlStatement, bool closeConnection = true);
        int ExecuteNonQuery(string storedProcedureKey, List<SqlParameter> parameters, bool closeConnection = true);
        int ExecuteNonQuery(string sqlStatement, bool closeConnection = true);
        DataTable GetTableBySql(string sqlStatement, bool closeConnection = true);
    }
}