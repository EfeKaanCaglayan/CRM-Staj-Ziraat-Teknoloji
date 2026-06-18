using Oracle.ManagedDataAccess.Client;
using ZTCRM.Models;
namespace ZTCRM.Data;


public class ServiceRequestRepository
{
    public int Create(int customerId, string requestType, string description, int? categoryId)
    {
        using var conn = DbConnection.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_Create", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_CustomerId", OracleDbType.Int32).Value = customerId;
        cmd.Parameters.Add("p_requestType", OracleDbType.Varchar2).Value = requestType;
        cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = description;
        cmd.Parameters.Add("p_CategoryId", OracleDbType.Int32).Value = (object?)categoryId ?? DBNull.Value;
        var outParam = cmd.Parameters.Add("p_requestId", OracleDbType.Int32);
        outParam.Direction = System.Data.ParameterDirection.Output;
        cmd.ExecuteNonQuery();
        return Convert.ToInt32(outParam.Value.ToString());
    }

    public List<ServiceRequest> GetByCustomer(int customerId)
    {
        var list = new List<ServiceRequest>();
        using var conn = DbConnection.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_GetByCustomer", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_CustomerId", OracleDbType.Int32).Value = customerId;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ServiceRequest
            {
                RequestId = reader.GetInt32(reader.GetOrdinal("RequestId")),
                RequestType = reader.GetString(reader.GetOrdinal("RequestType")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                CurrentStatus = reader.GetString(reader.GetOrdinal("CurrentStatus")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                ResolvedAt = reader.IsDBNull(reader.GetOrdinal("ResolvedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("ResolvedAt")),
                ClosedAt = reader.IsDBNull(reader.GetOrdinal("ClosedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("ClosedAt")),
                CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("CategoryName"))
            });
        }

        return list;
    }
}