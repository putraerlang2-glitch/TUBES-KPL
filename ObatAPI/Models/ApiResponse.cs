using System.Text.Json.Serialization;

namespace ObatAPI.Models
{
    /// <summary>
    /// Standard response wrapper untuk semua API responses
    /// </summary>
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string[]>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, Dictionary<string, string[]>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }

        public static ApiResponse<T> NotFoundResponse(string message = "Data tidak ditemukan")
        {
            return ErrorResponse(message);
        }

        public static ApiResponse<T> ValidationErrorResponse(Dictionary<string, string[]> errors)
        {
            return ErrorResponse("Validasi data gagal", errors);
        }
    }

    /// <summary>
    /// Response untuk summary status obat
    /// </summary>
    public class ObatStatusSummary
    {
        [JsonPropertyName("available")]
        public int Available { get; set; }

        [JsonPropertyName("lowStock")]
        public int LowStock { get; set; }

        [JsonPropertyName("expired")]
        public int Expired { get; set; }

        [JsonPropertyName("soonToExpire")]
        public int SoonToExpire { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
