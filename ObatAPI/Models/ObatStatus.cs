namespace ObatAPI.Models
{
    public enum ObatStatus
    {
        Available,
        LowStock,
        OutOfStock,
        Expired
    }

    public static class ObatConstants
    {
        public const int LOW_STOCK_THRESHOLD = 10;
    }
}
