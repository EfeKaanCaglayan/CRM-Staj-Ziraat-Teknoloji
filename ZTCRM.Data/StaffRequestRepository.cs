using Oracle.ManagedDataAccess.Client;
using ZTCRM.Models;

namespace ZTCRM.Data;

public class StaffRequestRepository
{ 
    private readonly string _connectionString = DbConnection.ConnectionString;
   

    public List<ServiceRequest> GetPool(int staffId)
    {
        var list = new List<ServiceRequest>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_GetPool", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_StaffId", OracleDbType.Int32).Value = staffId;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ServiceRequest
            {
                RequestId     = reader.GetInt32(reader.GetOrdinal("RequestId")),
                CustomerId    = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                Description   = reader.GetString(reader.GetOrdinal("Description")),
                Priority      = reader.GetString(reader.GetOrdinal("Priority")),
                CurrentStatus = reader.GetString(reader.GetOrdinal("CurrentStatus")),
                CreatedAt     = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CustomerName  = reader.GetString(reader.GetOrdinal("CustomerName")),
                CategoryName  = reader.GetString(reader.GetOrdinal("CategoryName")),
                OperatorName  = reader.IsDBNull(reader.GetOrdinal("OperatorName")) ? null : reader.GetString(reader.GetOrdinal("OperatorName"))
            });
        }
        return list;
    }

    public List<ServiceRequest> GetMyRequests(int staffId)
    {
        var list = new List<ServiceRequest>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_GetMyRequests", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_StaffId", OracleDbType.Int32).Value = staffId;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ServiceRequest
            {
                RequestId     = reader.GetInt32(reader.GetOrdinal("RequestId")),
                CustomerId    = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                Description   = reader.GetString(reader.GetOrdinal("Description")),
                Priority      = reader.GetString(reader.GetOrdinal("Priority")),
                CurrentStatus = reader.GetString(reader.GetOrdinal("CurrentStatus")),
                CreatedAt     = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CustomerName  = reader.GetString(reader.GetOrdinal("CustomerName")),
                CategoryName  = reader.GetString(reader.GetOrdinal("CategoryName"))
            });
        }
        return list;
    }

    public void AssignToSelf(int requestId, int staffId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_AssignToSelf", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_RequestId", OracleDbType.Int32).Value = requestId;
        cmd.Parameters.Add("p_StaffId",   OracleDbType.Int32).Value = staffId;
        cmd.ExecuteNonQuery();
    }

    public void UpdateStatus(int requestId, string newStatus, int staffId, string? resolutionNote)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_UpdateStatus", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_RequestId",      OracleDbType.Int32).Value    = requestId;
        cmd.Parameters.Add("p_NewStatus",      OracleDbType.Varchar2).Value = newStatus;
        cmd.Parameters.Add("p_StaffId",        OracleDbType.Int32).Value    = staffId;
        cmd.Parameters.Add("p_ResolutionNote", OracleDbType.Varchar2).Value = resolutionNote ?? string.Empty;
        cmd.ExecuteNonQuery();
    }

    public void ReturnToPool(int requestId, int staffId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_ReturnToPool", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_RequestId", OracleDbType.Int32).Value = requestId;
        cmd.Parameters.Add("p_StaffId",   OracleDbType.Int32).Value = staffId;
        cmd.ExecuteNonQuery();
    }
}