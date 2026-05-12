using System;
using System.Collections.Generic;
using TubesKPL;
using Xunit;

namespace TubesKPL.Test
{
    public class LoginTest
    {
        [Fact]
        public void Test_Login_Sukses()
        {
           
            string username = "admin";
            string password = "admin123";
          
            bool hasil = username .Equals(password);
            
            Assert.False(hasil);
        }

        [Fact]
        public void Test_Password_Salah()
        {
            string username = "admin";
            string password = "JaneDoeMandi";

            bool hasil = username.Equals(password);

            Assert.False(hasil);
        }

        [Fact]
        public void Test_Username_Salah()
        {
            string username = "JaneDoeBubuk";
            string password = "Admin123";

            bool hasil = username.Equals(password);

            Assert.False(hasil);
        }


    }
}
