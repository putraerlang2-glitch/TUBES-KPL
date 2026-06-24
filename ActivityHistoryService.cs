
using System;
using MySql.Data.MySqlClient;

namespace TubesKPL
{
    public static class ActivityHistoryService
    {
        public static void LogActivity(string activityType, string description = null, int? userId = null, int? transaksiId = null, int? obatId = null, int? batchId = null)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    
                    // Cek apakah kolom batch_id ada di tabel activity_history
                    bool hasBatchId = false;
                    string checkColumnQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_SCHEMA = DATABASE() 
                        AND TABLE_NAME = 'activity_history' 
                        AND COLUMN_NAME = 'batch_id'";
                    
                    using (var checkCmd = new MySqlCommand(checkColumnQuery, conn))
                    {
                        hasBatchId = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                    }
                    
                    string query;
                    if (hasBatchId)
                    {
                        query = @"
                            INSERT INTO activity_history 
                            (user_id, transaksi_id, obat_id, batch_id, activity_type, activity_description, created_at) 
                            VALUES 
                            (@userId, @transaksiId, @obatId, @batchId, @activityType, @description, NOW())";
                    }
                    else
                    {
                        query = @"
                            INSERT INTO activity_history 
                            (user_id, transaksi_id, obat_id, activity_type, activity_description, created_at) 
                            VALUES 
                            (@userId, @transaksiId, @obatId, @activityType, @description, NOW())";
                    }
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId.HasValue ? (object)userId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@transaksiId", transaksiId.HasValue ? (object)transaksiId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@obatId", obatId.HasValue ? (object)obatId.Value : DBNull.Value);
                        
                        if (hasBatchId)
                        {
                            cmd.Parameters.AddWithValue("@batchId", batchId.HasValue ? (object)batchId.Value : DBNull.Value);
                        }
                        
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
