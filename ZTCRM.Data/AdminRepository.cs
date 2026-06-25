using Oracle.ManagedDataAccess.Client;
using ZTCRM.Models;

namespace ZTCRM.Data;

public class AdminRepository
{
    private readonly string _connectionString = DbConnection.ConnectionString;

    public List<OrgUnit> GetAllUnits()
    {
        var list = new List<OrgUnit>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Admin_GetAllUnits", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new OrgUnit
            {
                UnitId       = reader.GetInt32(reader.GetOrdinal("UnitId")),
                UnitName     = reader.GetString(reader.GetOrdinal("UnitName")),
                UnitType     = reader.GetString(reader.GetOrdinal("UnitType")),
                ParentUnitId = reader.IsDBNull(reader.GetOrdinal("ParentUnitId")) ? null : reader.GetInt32(reader.GetOrdinal("ParentUnitId")),
                IsActiveText = reader.IsDBNull(reader.GetOrdinal("IsActiveText")) ? null : reader.GetString(reader.GetOrdinal("IsActiveText")),
                CreatedAt    = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }
        return list;
    }

    public int CreateUnit(string unitName, string unitType, int? parentUnitId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Admin_CreateUnit", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_UnitName",     OracleDbType.Varchar2).Value = unitName;
        cmd.Parameters.Add("p_UnitType",     OracleDbType.Varchar2).Value = unitType;
        cmd.Parameters.Add("p_ParentUnitId", OracleDbType.Int32).Value    = parentUnitId.HasValue ? parentUnitId.Value : DBNull.Value;
        cmd.Parameters.Add("p_IsActive",     OracleDbType.Int32).Value    = 1;
        var outParam = cmd.Parameters.Add("p_UnitId", OracleDbType.Int32);
        outParam.Direction = System.Data.ParameterDirection.Output;
        cmd.ExecuteNonQuery();
        return ((Oracle.ManagedDataAccess.Types.OracleDecimal)outParam.Value).ToInt32();
    }

    public void DeactivateUnit(int unitId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Admin_DeactivateUnit", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_UnitId", OracleDbType.Int32).Value = unitId;
        cmd.ExecuteNonQuery();
    }

    public List<Staff> GetAllStaff()
    {
        var list = new List<Staff>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Admin_GetAllStaff", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Staff
            {
                StaffId      = reader.GetInt32(reader.GetOrdinal("StaffId")),
                RoleId       = reader.GetInt32(reader.GetOrdinal("RoleId")),
                FullName     = reader.GetString(reader.GetOrdinal("FullName")),
                Username     = reader.GetString(reader.GetOrdinal("Username")),
                UnitName = reader.IsDBNull(reader.GetOrdinal("UnitName")) ? null : reader.GetString(reader.GetOrdinal("UnitName")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                IsActiveText = reader.GetString(reader.GetOrdinal("IsActiveText")),
                UnitIsActiveText = reader.IsDBNull(reader.GetOrdinal("UnitIsActiveText")) ? string.Empty : reader.GetString(reader.GetOrdinal("UnitIsActiveText"))          });
        }
        return list;
    }

    public int CreateStaff(int roleId, string fullName, string username, string passwordHash)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Admin_CreateStaff", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_RoleId",       OracleDbType.Int32).Value    = roleId;
        cmd.Parameters.Add("p_FullName",     OracleDbType.Varchar2).Value = fullName;
        cmd.Parameters.Add("p_UserName",     OracleDbType.Varchar2).Value = username;
        cmd.Parameters.Add("p_PasswordHash", OracleDbType.Varchar2).Value = passwordHash;
        cmd.Parameters.Add("p_IsActive",     OracleDbType.Int32).Value    = 1;
        var outParam = cmd.Parameters.Add("p_StaffId", OracleDbType.Int32);
        outParam.Direction = System.Data.ParameterDirection.Output;
        cmd.ExecuteNonQuery();
        return ((Oracle.ManagedDataAccess.Types.OracleDecimal)outParam.Value).ToInt32();
    }

    public void UpdateStaffRole(int staffId, int roleId)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Admin_UpdateStaffRole", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_StaffId", OracleDbType.Int32).Value = staffId;
        cmd.Parameters.Add("p_RoleId",  OracleDbType.Int32).Value = roleId;
        cmd.ExecuteNonQuery();
    }

    public List<ActivityLog> GetActivityLog()
    {
        var list = new List<ActivityLog>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Admin_GetActivityLog", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ActivityLog
            {
                LogId           = reader.GetInt32(reader.GetOrdinal("LogId")),
                RequestId       = reader.GetInt32(reader.GetOrdinal("RequestId")),
                OldStatus       = reader.GetString(reader.GetOrdinal("OldStatus")),
                NewStatus       = reader.GetString(reader.GetOrdinal("NewStatus")),
                ChangedAt       = reader.GetDateTime(reader.GetOrdinal("ChangedAt")),
                SystemGenerated = reader.GetInt32(reader.GetOrdinal("SystemGenerated")) == 1,
                ChangedBy       = reader.IsDBNull(reader.GetOrdinal("ChangedBy")) ? null : reader.GetString(reader.GetOrdinal("ChangedBy"))
            });
        }
        return list;
    }
    public void AssignStaffToUnit(int staffId, int unitId, int isPrimary)
    {
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Admin_AssignStaffToUnit", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.Add("p_StaffId",   OracleDbType.Int32).Value = staffId;
        cmd.Parameters.Add("p_UnitId",    OracleDbType.Int32).Value = unitId;
        cmd.Parameters.Add("p_IsPrimary", OracleDbType.Int32).Value = isPrimary;
        cmd.ExecuteNonQuery();
    }
}