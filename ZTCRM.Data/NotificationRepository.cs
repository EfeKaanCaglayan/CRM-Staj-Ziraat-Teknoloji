using Oracle.ManagedDataAccess.Client;
using ZTCRM.Models;

namespace ZTCRM.Data;

public  class NotificationRepository
{
    public List<Notification> GetByCustomer(int customerId)
    {
        var list = new List<Notification>();
        using var conn = DbConnection.GetConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandType=System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "ZTCRM.sp_Notification_GetByCustomer";
        cmd.Parameters.Add("p_CustomerId", OracleDbType.Int32).Value = customerId;
        cmd.Parameters.Add("p_Result",OracleDbType.RefCursor).Direction=System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Notification
            {
                NotifId   = reader.GetInt32(reader.GetOrdinal("NotifId")),
                RequestId = reader.GetInt32(reader.GetOrdinal("RequestId")),
                Message   = reader.GetString(reader.GetOrdinal("Message")),
                IsSent    = reader.GetInt32(reader.GetOrdinal("IsSent")) == 1,
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }
        return list;
    }
    public int GetUnreadCount(int customerId)
    {
        using var conn = DbConnection.GetConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "ZTCRM.sp_Notification_GetUnreadCount";
        cmd.Parameters.Add("p_CustomerId", OracleDbType.Int32).Value = customerId;
        var outParam = cmd.Parameters.Add("p_Count", OracleDbType.Int32);
        outParam.Direction = System.Data.ParameterDirection.Output;
        cmd.ExecuteNonQuery();
        return Convert.ToInt32(outParam.Value.ToString());
    }
    public void MarkAsRead(int customerId)
    {
        using var conn = DbConnection.GetConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "ZTCRM.sp_Notification_MarkAsRead";
        cmd.Parameters.Add("p_CustomerId", OracleDbType.Int32).Value = customerId;
        cmd.ExecuteNonQuery();
    }
    public List<Notification> GetByStaff(int staffId)
    {
        var list = new List<Notification>();
        using var conn = DbConnection.GetConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "ZTCRM.sp_Notification_GetByStaff";
        cmd.Parameters.Add("p_StaffId", OracleDbType.Int32).Value = staffId;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Notification
            {
                NotifId   = reader.GetInt32(reader.GetOrdinal("NotifId")),
                RequestId = reader.GetInt32(reader.GetOrdinal("RequestId")),
                Message   = reader.GetString(reader.GetOrdinal("Message")),
                IsSent    = reader.GetInt32(reader.GetOrdinal("IsSent")) == 1,
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }
        return list;
    }

    public int GetUnreadCountByStaff(int staffId)
    {
        using var conn = DbConnection.GetConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "ZTCRM.sp_Notification_GetUnreadCountByStaff";
        cmd.Parameters.Add("p_StaffId", OracleDbType.Int32).Value = staffId;
        var outParam = cmd.Parameters.Add("p_Count", OracleDbType.Int32);
        outParam.Direction = System.Data.ParameterDirection.Output;
        cmd.ExecuteNonQuery();
        return Convert.ToInt32(outParam.Value.ToString());
    }

    public void MarkAsReadByStaff(int staffId)
    {
        using var conn = DbConnection.GetConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "ZTCRM.sp_Notification_MarkAsReadByStaff";
        cmd.Parameters.Add("p_StaffId", OracleDbType.Int32).Value = staffId;
        cmd.ExecuteNonQuery();
    }
}