using System.Data;
using Oracle.ManagedDataAccess.Client;
using ZTCRM.Models;

namespace ZTCRM.Data;

public class DashboardRepository
{
 
 private readonly string _connectionString=DbConnection.ConnectionString;
 
 public DashboardSummary GetSummary()
 {
  var summary = new DashboardSummary();
  using var conn = new OracleConnection(_connectionString);
  conn.Open();
  using var cmd = new OracleCommand("ZTCRM.sp_Dashboard_GetSummary", conn);
  cmd.CommandType = CommandType.StoredProcedure;
  cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
  using var reader = cmd.ExecuteReader();
  if (reader.Read())
  {
   summary.OpenCount=reader.GetInt32(reader.GetOrdinal("OpenCount"));
   summary.ClosedCount = reader.GetInt32((reader.GetOrdinal("ClosedCount")));
   summary.AvgResolutionHours=reader.GetDecimal(reader.GetOrdinal("AvgResolutionHours"));
   summary.TodayCount = reader.GetInt32(reader.GetOrdinal("TodayCount"));
  }

  return summary;
 }

 public List<CategoryDistributionItem> GetCategorySummary()
 {
  var list = new List<CategoryDistributionItem>();
  using var conn = new OracleConnection(_connectionString);
  conn.Open();
  using var cmd = new OracleCommand("ZTCRM.sp_Dashboard_GetCategoryDistribution",conn);
  cmd.CommandType = CommandType.StoredProcedure;
  cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
  using var reader = cmd.ExecuteReader();
  while (reader.Read())
  {
   list.Add(new CategoryDistributionItem
   {
    CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) 
     ? null 
     : reader.GetString(reader.GetOrdinal("CategoryName")),
    RequestCount = reader.GetInt32(reader.GetOrdinal("RequestCount"))
   });
  }
  return list;
 }
 public List<StatusDistributionItem> GetStatusSummary()
    {
        var list = new List<StatusDistributionItem>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Dashboard_GetStatusDistribution", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new StatusDistributionItem
            {
                StatusName = reader.IsDBNull(reader.GetOrdinal("StatusName")) ? null : reader.GetString(reader.GetOrdinal("StatusName")),
                RequestCount = reader.GetInt32(reader.GetOrdinal("RequestCount"))
            });
        }
        return list;
    }


    public List<ChannelDistributionItem> GetChannelSummary()
    {
        var list = new List<ChannelDistributionItem>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Dashboard_GetChannelDistribution", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ChannelDistributionItem
            {
                ChannelName = reader.IsDBNull(reader.GetOrdinal("ChannelName")) ? null : reader.GetString(reader.GetOrdinal("ChannelName")),
                RequestCount = reader.GetInt32(reader.GetOrdinal("RequestCount"))
            });
        }
        return list;
    }


    public List<DailyTrendItem> GetDailyTrend()
    {
        var list = new List<DailyTrendItem>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Dashboard_GetDailyTrend", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new DailyTrendItem
            {
                RequestDate = reader.GetDateTime(reader.GetOrdinal("RequestDate")),
                RequestCount = reader.GetInt32(reader.GetOrdinal("RequestCount"))
            });
        }
        return list;
    }

   
    public List<UnitResolutionTimeItem> GetUnitResolutionTime()
    {
        var list = new List<UnitResolutionTimeItem>();
        using var conn = new OracleConnection(_connectionString);
        conn.Open();
        using var cmd = new OracleCommand("ZTCRM.sp_Dashboard_GetUnitResolutionTime", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("p_Result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new UnitResolutionTimeItem
            {
                UnitName = reader.IsDBNull(reader.GetOrdinal("UnitName")) ? null : reader.GetString(reader.GetOrdinal("UnitName")),
                AvgResolutionHours = reader.GetDecimal(reader.GetOrdinal("AvgResolutionHours"))
            });
        }
        return list;
    }
 
}