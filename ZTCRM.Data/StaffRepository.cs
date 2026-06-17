
using Oracle.ManagedDataAccess.Client;
using ZTCRM.Models;
namespace ZTCRM.Data;

public class StaffRepository
{
    public Staff? Login(string username, string password)
    {
        using var conn = DbConnection.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Staff_Login", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;

        cmd.Parameters.Add("p_Username", OracleDbType.Varchar2).Value = username;
        cmd.Parameters.Add("p_Password", OracleDbType.Varchar2).Value = password;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Staff
            {

                StaffId = reader.GetInt32(reader.GetOrdinal("StaffId")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                IsActive = reader.GetInt32(reader.GetOrdinal("IsActive")) == 1

            };
        }

        return null;
        }
    }