using Oracle.ManagedDataAccess.Client;
using ZTCRM.Models;

namespace ZTCRM.Data;

public class ManagerRepository
{
    private readonly string _connectionString = DbConnection.ConnectionString;

    public List<ServiceRequest> GetPendingApproval(int managerId)
    {
        var list = new List<ServiceRequest>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_GetPendingApproval", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_ManagerId", OracleDbType.Int32).Value = managerId;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ServiceRequest
            {
                RequestId      = reader.GetInt32(reader.GetOrdinal("RequestId")),
                CustomerId     = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                CustomerName   = reader.GetString(reader.GetOrdinal("CustomerName")),
                Description    = reader.GetString(reader.GetOrdinal("Description")),
                ResolutionNote = reader.IsDBNull(reader.GetOrdinal("ResolutionNote")) ? null : reader.GetString(reader.GetOrdinal("ResolutionNote")),
                Priority       = reader.GetString(reader.GetOrdinal("Priority")),
                CurrentStatus  = reader.GetString(reader.GetOrdinal("CurrentStatus")),
                StaffName      = reader.GetString(reader.GetOrdinal("StaffName")),
                CategoryName   = reader.GetString(reader.GetOrdinal("CategoryName")),
                CreatedAt      = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }
        return list;
    }

    public void Approve(int requestId, int managerId, string note)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_Approve", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_RequestId", OracleDbType.Int32).Value    = requestId;
        cmd.Parameters.Add("p_ManagerId", OracleDbType.Int32).Value    = managerId;
        cmd.Parameters.Add("p_Note",      OracleDbType.Varchar2).Value = note ?? string.Empty;
        cmd.ExecuteNonQuery();
    }

    public void RejectByManager(int requestId, int managerId, string note)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_RejectByManager", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_RequestId", OracleDbType.Int32).Value    = requestId;
        cmd.Parameters.Add("p_ManagerId", OracleDbType.Int32).Value    = managerId;
        cmd.Parameters.Add("p_Note",      OracleDbType.Varchar2).Value = note ?? string.Empty;
        cmd.ExecuteNonQuery();
    }
}