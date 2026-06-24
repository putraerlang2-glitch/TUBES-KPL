using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TubesKPL.Test
{
    public class AddObatTest
    {
        [Fact]
        public void Factory_CreateObat_Berhasil()
        {
            string nama = "Vitamin C";
            int stok = 20;
            decimal harga = 13000;
            DateTime expired = DateTime.Now.AddDays(30);
            KategoriObat kategori = KategoriObat.Vitamin;

            Obat obat = ObatFactory.Create(
                nama,
                stok,
                harga,
                expired,
                kategori);

            Assert.NotNull(obat);
            Assert.Equal("Vitamin C", obat.Nama);
            Assert.Equal(20, obat.Stok);
            Assert.Equal(13000, obat.Harga);
            Assert.Equal(kategori.ToString(), obat.Kategori);
        }

        [Fact]
        public void Validator_InputValid_ReturnTrue()
        {
            string nama = "Paracetamol";

            bool hasil = ObatValidator.ValidateObatInput(
                nama,
                15,
                5000);

            Assert.True(hasil);
        }

        [Fact]
        public void Validator_NamaKosong_ReturnFalse()
        {
            bool hasil = ObatValidator.ValidateObatInput(
                "",
                15,
                5000);

            Assert.False(hasil);
        }

        [Fact]
        public void Validator_StokNegatif_ReturnFalse()
        {
            bool hasil = ObatValidator.ValidateObatInput(
                "Vitamin C",
                -5,
                5000);

            Assert.False(hasil);
        }

        [Fact]
        public void Validator_HargaNegatif_ReturnFalse()
        {
            bool hasil = ObatValidator.ValidateObatInput(
                "Vitamin C",
                10,
                -1000);

            Assert.False(hasil);
        }
    }
}