using ObatAPI.Enums;
using ObatAPI.Models;

namespace ObatAPI.Services
{
    /// <summary>
    /// Service untuk mengelola logika bisnis obat
    /// </summary>
    public interface IObatService
    {
        void UpdateObatStatus(Obat obat);
        bool ValidateObatData(Obat obat, out Dictionary<string, string[]> errors);
        IEnumerable<Obat> GetExpiredObat();
        IEnumerable<Obat> GetSoonToExpireObat(int daysThreshold = 30);
    }

    public class ObatService : IObatService
    {
        private const int LOW_STOCK_THRESHOLD = 5;
        private const int SOON_TO_EXPIRE_DAYS = 30;

        private readonly ILogger<ObatService> _logger;

        public ObatService(ILogger<ObatService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Memperbarui status obat berdasarkan stok dan tanggal kadaluarsa
        /// </summary>
        public void UpdateObatStatus(Obat obat)
        {
            if (obat == null)
            {
                _logger.LogWarning("Attempted to update status of null obat");
                throw new ArgumentNullException(nameof(obat), "Obat tidak boleh null");
            }

            try
            {
                var today = DateTime.Now.Date;

                if (obat.ExpiredDate.Date < today)
                {
                    obat.Status = ObatStatus.Expired;
                    _logger.LogDebug($"Obat {obat.Id} marked as Expired");
                }
                else if (IsExpiringSoon(obat.ExpiredDate))
                {
                    obat.Status = ObatStatus.SoonToExpire;
                    _logger.LogDebug($"Obat {obat.Id} marked as SoonToExpire");
                }
                else if (obat.Stok <= LOW_STOCK_THRESHOLD)
                {
                    obat.Status = ObatStatus.LowStock;
                    _logger.LogDebug($"Obat {obat.Id} marked as LowStock");
                }
                else
                {
                    obat.Status = ObatStatus.Available;
                    _logger.LogDebug($"Obat {obat.Id} marked as Available");
                }

                obat.UpdatedAt = DateTime.Now;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating status for obat {obat.Id}");
                throw;
            }
        }

        /// <summary>
        /// Validasi data obat sebelum disimpan
        /// </summary>
        public bool ValidateObatData(Obat obat, out Dictionary<string, string[]> errors)
        {
            errors = new Dictionary<string, string[]>();

            if (obat == null)
            {
                errors.Add("obat", new[] { "Data obat tidak boleh null" });
                return false;
            }

            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obat);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                obat, validationContext, validationResults, validateAllProperties: true);

            if (!isValid)
            {
                foreach (var result in validationResults)
                {
                    var key = result.MemberNames.FirstOrDefault() ?? "general";
                    if (!errors.ContainsKey(key))
                        errors[key] = Array.Empty<string>();

                    var messages = errors[key].ToList();
                    messages.Add(result.ErrorMessage ?? "Validasi gagal");
                    errors[key] = messages.ToArray();
                }

                _logger.LogWarning($"Validation failed for obat: {string.Join(", ", errors.Select(e => $"{e.Key}: {string.Join("; ", e.Value)}"))}");
                return false;
            }

            // Additional business logic validation
            if (obat.ExpiredDate.Date <= DateTime.Now.Date)
            {
                errors.Add("expiredDate", new[] { "Tanggal kadaluarsa harus di masa depan" });
                _logger.LogWarning($"Validation failed: ExpiredDate is in the past for obat '{obat.Nama}'");
                return false;
            }

            if (string.IsNullOrWhiteSpace(obat.Nama))
            {
                errors.Add("nama", new[] { "Nama obat tidak boleh kosong atau hanya spasi" });
                return false;
            }

            if (string.IsNullOrWhiteSpace(obat.Kategori))
            {
                errors.Add("kategori", new[] { "Kategori tidak boleh kosong atau hanya spasi" });
                return false;
            }

            return true;
        }

        /// <summary>
        /// Mengambil semua obat yang sudah kadaluarsa
        /// </summary>
        public IEnumerable<Obat> GetExpiredObat()
        {
            _logger.LogInformation("Fetching expired obat");
            return Enumerable.Empty<Obat>();
        }

        /// <summary>
        /// Mengambil obat yang akan kadaluarsa dalam periode tertentu
        /// </summary>
        public IEnumerable<Obat> GetSoonToExpireObat(int daysThreshold = 30)
        {
            if (daysThreshold < 0)
            {
                _logger.LogWarning($"Invalid daysThreshold: {daysThreshold}");
                throw new ArgumentException("daysThreshold tidak boleh negatif", nameof(daysThreshold));
            }

            _logger.LogInformation($"Fetching soon-to-expire obat (threshold: {daysThreshold} days)");
            return Enumerable.Empty<Obat>();
        }

        /// <summary>
        /// Helper: Cek apakah obat akan kadaluarsa dalam waktu singkat
        /// </summary>
        private bool IsExpiringSoon(DateTime expiredDate)
        {
            var today = DateTime.Now.Date;
            var daysUntilExpiry = (expiredDate.Date - today).Days;
            return daysUntilExpiry > 0 && daysUntilExpiry <= SOON_TO_EXPIRE_DAYS;
        }
    }
}
