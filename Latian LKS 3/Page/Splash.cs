using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Latian_LKS_3
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
            this.BackColor = ColourModel.primary;
        }

        private async void Splash_Load(object sender, EventArgs e)
        {
            await Task.Delay(2000);
            
            if (Properties.Settings.Default.level == "Admin" || Properties.Settings.Default.level == "Petugas")
            {
                AdminDashboard admin = new AdminDashboard();
                admin.Show();
            }  
            else if (Properties.Settings.Default.level == "Anggota")
            {
                AnggotaDashboard anggota = new AnggotaDashboard();
                anggota.Show();
            }
            else
            {
                Login login = new Login();
                login.Show();
            }


            this.Hide();
        }
    }
}
