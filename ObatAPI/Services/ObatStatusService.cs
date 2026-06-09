using ObatAPI.Models;

namespace ObatAPI.Services
{
    public interface IObatStatusService
    {
        void EvaluateStatus(Obat obat);
    }

    public class ObatStatusService : IObatStatusService
    {
        public void EvaluateStatus(Obat obat)
        {
            if (obat == null)
                return;

            if (obat.ExpiredDate.Date < DateTime.Now.Date)
                obat.Status = "Expired";
            else if (obat.Stok <= 5)
                obat.Status = "LowStock";
            else
                obat.Status = "Available";
        }
    }
}
