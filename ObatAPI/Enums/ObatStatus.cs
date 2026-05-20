namespace ObatAPI.Enums
{
    /// <summary>
    /// Enum untuk status obat berdasarkan stok dan tanggal kadaluarsa
    /// </summary>
    public enum ObatStatus
    {
        /// <summary>Obat tersedia dengan stok normal</summary>
        Available = 0,

        /// <summary>Obat memiliki stok rendah (di bawah threshold)</summary>
        LowStock = 1,

        /// <summary>Obat telah melewati tanggal kadaluarsa</summary>
        Expired = 2,

        /// <summary>Obat akan kadaluarsa dalam waktu singkat (dalam 30 hari)</summary>
        SoonToExpire = 3
    }
}
