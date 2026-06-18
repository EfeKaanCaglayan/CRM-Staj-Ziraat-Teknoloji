using Oracle.ManagedDataAccess.Client;
using ZTCRM.Models;

namespace ZTCRM.Data;

public class CustomerRepository
{
    public Customer? Login(string? nationalId, string? passportNo = null)
    {
        using var conn = DbConnection.GetConnection();
        conn.Open();

        using var cmd = new OracleCommand("ZTCRM.sp_Customer_Login", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;

        cmd.Parameters.Add("p_NationalId", OracleDbType.Varchar2).Value =
            (object?)nationalId ?? DBNull.Value;
        cmd.Parameters.Add("p_PassportNo", OracleDbType.Varchar2).Value =
            (object?)passportNo ?? DBNull.Value;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction =
            System.Data.ParameterDirection.Output;

        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new Customer
            {
                CustomerId    = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                FullName      = reader.GetString(reader.GetOrdinal("FullName")),
                CustomerType  = reader.GetString(reader.GetOrdinal("CustomerType")),
                NotifyChannel = reader.GetString(reader.GetOrdinal("NotifyChannel")),
                IsActive      = reader.GetInt32(reader.GetOrdinal("IsActive")) == 1
            };
        }

        return null;
    }
}