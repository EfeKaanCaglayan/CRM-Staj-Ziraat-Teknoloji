using Oracle.ManagedDataAccess.Client;
using ZTCRM.Models;

namespace ZTCRM.Data;

public class OperatorRepository
{
    private readonly string _connectionString = DbConnection.ConnectionString;

    public List<ServiceRequest> GetPending()
    {
        var list = new List<ServiceRequest>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_GetPending", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
    
        Console.WriteLine($"Reader açıldı, veri var mı: {reader.HasRows}");
    
        while (reader.Read())
        {
            Console.WriteLine($"Satır okundu: {reader.GetInt32(0)}");
            list.Add(new ServiceRequest
            {
                RequestId     = reader.GetInt32(reader.GetOrdinal("RequestId")),
                CustomerId    = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                CustomerName  = reader.GetString(reader.GetOrdinal("CustomerName")),
                Description   = reader.GetString(reader.GetOrdinal("Description")),
                CurrentStatus = reader.GetString(reader.GetOrdinal("CurrentStatus")),
                CreatedAt     = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }
    
        Console.WriteLine($"Toplam kayıt: {list.Count}");
        return list;
    }
    public List<ServiceRequest> GetPool(int staffId)
    {
        var list = new List<ServiceRequest>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Operator_GetPool", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
      
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ServiceRequest
            {
                RequestId = reader.GetInt32(reader.GetOrdinal("RequestId")),
                CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                CurrentStatus = reader.GetString(reader.GetOrdinal("CurrentStatus")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? null : reader.GetString(reader.GetOrdinal("CategoryName"))
            });
        }
        return list;
    }
    
    public Customer? FindCustomerByNationalId(string nationalId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Customer_Login", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_NationalId", OracleDbType.Varchar2).Value = nationalId;
        cmd.Parameters.Add("p_PassportNo", OracleDbType.Varchar2).Value = DBNull.Value;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Customer
            {
                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                FullName = reader.GetString(reader.GetOrdinal("FullName"))
            };
        }
        return null;
    }

    public void CreateRequest(int customerId, string requestType, string description, string channel)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_Create", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_CustomerId",  OracleDbType.Int32).Value    = customerId;
        cmd.Parameters.Add("p_RequestType", OracleDbType.Varchar2).Value = requestType;
        cmd.Parameters.Add("p_Description", OracleDbType.Varchar2).Value = description;
        cmd.Parameters.Add("p_CategoryId",  OracleDbType.Int32).Value    = DBNull.Value;
        cmd.Parameters.Add("p_Channel",     OracleDbType.Varchar2).Value = channel;
        var outParam = cmd.Parameters.Add("p_RequestId", OracleDbType.Int32);
        outParam.Direction = System.Data.ParameterDirection.Output;
        cmd.ExecuteNonQuery();
    }
    public void UpdateCategory(int requestId, int categoryId, string priority)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_UpdateCategory", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_RequestId", OracleDbType.Int32).Value = requestId;
        cmd.Parameters.Add("p_CategoryId", OracleDbType.Int32).Value = categoryId;
        cmd.Parameters.Add("p_Priority", OracleDbType.Varchar2).Value = priority;
        cmd.ExecuteNonQuery();
    }
    public void ReturnToPending(int requestId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_ReturnToPending", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_RequestId", OracleDbType.Int32).Value = requestId;
        cmd.ExecuteNonQuery();
    }

    public List<Category> GetCategories()
    {
        var list = new List<Category>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Category_GetAll", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Category
            {
                CategoryId       = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                CategoryName     = reader.GetString(reader.GetOrdinal("CategoryName")),
                DefaultUnitId    = reader.IsDBNull(reader.GetOrdinal("DefaultUnitId")) ? null : reader.GetInt32(reader.GetOrdinal("DefaultUnitId")),
                ParentCategoryId = reader.IsDBNull(reader.GetOrdinal("ParentCategoryId")) ? null : reader.GetInt32(reader.GetOrdinal("ParentCategoryId"))
            });
        }
        return list;
    }

    public void CategorizeAndPool(int requestId, int categoryId, string priority, int operatorId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_CategorizeAndPool", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_RequestId",  OracleDbType.Int32).Value    = requestId;
        cmd.Parameters.Add("p_CategoryId", OracleDbType.Int32).Value    = categoryId;
        cmd.Parameters.Add("p_Priority",   OracleDbType.Varchar2).Value = priority;
        cmd.Parameters.Add("p_OperatorId", OracleDbType.Int32).Value    = operatorId;
        cmd.ExecuteNonQuery();
    }
    public void Reject(int requestId, string rejectionType, string rejectionReason, int operatorId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_ServiceRequest_Reject", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_RequestId",       OracleDbType.Int32).Value    = requestId;
        cmd.Parameters.Add("p_RejectionType",   OracleDbType.Varchar2).Value = rejectionType;
        cmd.Parameters.Add("p_RejectionReason", OracleDbType.Varchar2).Value = rejectionReason;
        cmd.Parameters.Add("p_OperatorId",      OracleDbType.Int32).Value    = operatorId;
        cmd.ExecuteNonQuery();
    }

}