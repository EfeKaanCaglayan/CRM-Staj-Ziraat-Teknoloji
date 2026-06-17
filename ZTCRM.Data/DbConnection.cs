using Oracle.ManagedDataAccess.Client;
namespace ZTCRM.Data;

public class DbConnection
{
    private const string ConnectionString=  "User Id=ztcrm;Password=Ziraat123;Data Source=localhost:1521/FREEPDB1;";

    public static OracleConnection GetConnection()
    {
        return new OracleConnection(ConnectionString);
    }
}