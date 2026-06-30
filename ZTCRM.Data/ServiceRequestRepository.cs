using Oracle.ManagedDataAccess.Client;
using ZTCRM.Models;
namespace ZTCRM.Data;


public class ServiceRequestRepository
{



    public void Cancel(int requestId,int customerId)
    {
        using var conn =DbConnection.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_Cancel", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_requestId", OracleDbType.Int32).Value = requestId;
        cmd.Parameters.Add("p_CustomerId", OracleDbType.Int32).Value = customerId;
        cmd.ExecuteNonQuery();
    
    }
    public NotificationInfo? GetNotificationInfo(int requestId)
    {
        using var conn = DbConnection.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_GetNotificationInfo", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_RequestId", OracleDbType.Int32).Value = requestId;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new NotificationInfo
            {
                Channel = reader.IsDBNull(reader.GetOrdinal("Channel")) ? "App" : reader.GetString(reader.GetOrdinal("Channel")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                RejectionReason = reader.IsDBNull(reader.GetOrdinal("RejectionReason")) ? null : reader.GetString(reader.GetOrdinal("RejectionReason")),
                ApprovalNote = reader.IsDBNull(reader.GetOrdinal("ApprovalNote")) ? null : reader.GetString(reader.GetOrdinal("ApprovalNote"))
            };
        }
        return null;
    }
    
    public int Create(int customerId, string requestType, string description, int? categoryId,string channel="App")
    {
        using var conn = DbConnection.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_Create", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_CustomerId", OracleDbType.Int32).Value = customerId;
        cmd.Parameters.Add("p_requestType", OracleDbType.Varchar2).Value = requestType;
        cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = description;
        cmd.Parameters.Add("p_CategoryId", OracleDbType.Int32).Value = (object?)categoryId ?? DBNull.Value;
        cmd.Parameters.Add("p_Channel", OracleDbType.Varchar2).Value = channel;
        var outParam = cmd.Parameters.Add("p_requestId", OracleDbType.Int32);
        outParam.Direction = System.Data.ParameterDirection.Output;
        cmd.ExecuteNonQuery();
        return Convert.ToInt32(outParam.Value.ToString());
    }
    public List<ServiceRequest> GetAllByCustomer(int customerId)
    {
        var list = new List<ServiceRequest>();
        using var conn = DbConnection.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_GetAllByCustomer", conn);
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