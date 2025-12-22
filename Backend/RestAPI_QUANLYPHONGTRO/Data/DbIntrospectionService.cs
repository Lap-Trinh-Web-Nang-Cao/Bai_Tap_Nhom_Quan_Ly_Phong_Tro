using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RestAPI_QUANLYPHONGTRO.Data
{
 public interface IDbIntrospectionService
 {
 Task<bool> TableExistsAsync(string tableName);
 Task<string?> GetCurrentDatabaseNameAsync();
 }

 public class DbIntrospectionService : IDbIntrospectionService
 {
 private readonly string _connectionString;

 public DbIntrospectionService(IConfiguration configuration)
 {
 _connectionString = configuration.GetConnectionString("DefaultConnection")
 ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
 }

 public async Task<bool> TableExistsAsync(string tableName)
 {
 const string sql = @"SELECT 1
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
 AND TABLE_NAME = @TableName";

 await using var conn = new SqlConnection(_connectionString);
 await conn.OpenAsync();

 await using var cmd = new SqlCommand(sql, conn);
 cmd.Parameters.AddWithValue("@TableName", tableName);

 var result = await cmd.ExecuteScalarAsync();
 return result != null;
 }

 public async Task<string?> GetCurrentDatabaseNameAsync()
 {
 await using var conn = new SqlConnection(_connectionString);
 await conn.OpenAsync();
 return conn.Database;
 }
 }
}
