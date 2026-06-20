
using System;
using MySql.Data.MySqlClient;

namespace TubesKPL
{
    public static class ActivityHistoryService
    {
        public static void LogActivity(string activityType, string description = null, int? userId = null, int? transaksiId = null, int? obatId = null)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    const string query = @"
                        INSERT INTO activity_history 
                        (user_id, transaksi_id, obat_id, activity_type, activity_description, created_at) 
                        VALUES 
                        (@userId, @transaksiId, @obatId, @activityType, @description, NOW())";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId.HasValue ? (object)userId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@transaksiId", transaksiId.HasValue ? (object)transaksiId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@obatId", obatId.HasValue ? (object)obatId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@activityType", activityType);
                        cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                // If logging fails, just swallow the error to not break the main flow
            }
        }
    }
}
