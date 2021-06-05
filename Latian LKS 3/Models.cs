using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Latian_LKS_3
{
    class Connection
    {
       public static string connectionString = @"Data Source=LAPTOP-QK9HLMP4\HOYIRULSQL;Initial Catalog=perpustakaan_smk_nasional;Integrated Security=True";
    }

    class ColourModel
    {
        public static Color primary = Color.FromArgb(255, 4, 50, 110);
        public static Color secondary = Color.FromArgb(255, 5, 75, 150);
    }

    class EncryptionModel
    {
        public static string encrypt(string data)
        {
            using (SHA256Managed manage = new SHA256Managed())
            {
                byte[] unencrypted = Encoding.UTF8.GetBytes(data);
                byte[] encrypted = manage.ComputeHash(unencrypted);
                string result = Convert.ToBase64String(encrypted);

                return result;
            }
        }
    }
}
