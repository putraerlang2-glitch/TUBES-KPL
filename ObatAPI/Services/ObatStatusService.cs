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
                obat.Status = ObatStatus.Expired;
            else if (obat.Stok <= 0)
                obat.Status = ObatStatus.OutOfStock;
            else if (obat.Stok <= ObatConstants.LOW_STOCK_THRESHOLD)
                obat.Status = ObatStatus.LowStock;
            else
                obat.Status = ObatStatus.Available;
        }
    }
}
