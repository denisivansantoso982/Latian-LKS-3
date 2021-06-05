using Latian_LKS_3.Template;
using Latian_LKS_3.Transaction;
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
    public partial class AnggotaDashboard : Form
    {
        public AnggotaDashboard()
        {
            InitializeComponent();
            label3.Text = Properties.Settings.Default.nama;
            label3.Location = new Point(label3.Location.X - label3.Size.Width / 2, label3.Location.Y);
            label1.Location = new Point(label3.Location.X + label3.Width / 2 - label1.Size.Width / 2, label1.Location.Y);
            timer.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            labelTimer.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogTemplate dialog = new DialogTemplate("Informasi", "Apakah anda yakin?", MessageBoxButtons.YesNo);
            dialog.ShowDialog();
            if (dialog.DialogResult == DialogResult.Yes)
            {
                Properties.Settings.Default.username = "";
                Properties.Settings.Default.password = "";
                Properties.Settings.Default.nama = "";
                Properties.Settings.Default.nik = "";
                Properties.Settings.Default.alamat = "";
                Properties.Settings.Default.level = "";
                Properties.Settings.Default.Save();

                Login login = new Login();
                login.Show();
                this.Close();
            }
        }

        private void panelCari_Click(object sender, EventArgs e)
        {
            CariBuku cariBuku = new CariBuku();
            cariBuku.ShowDialog();
        }

        private void panelHistory_Click(object sender, EventArgs e)
        {
            RiwayatPeminjaman riwayat = new RiwayatPeminjaman();
            riwayat.ShowDialog();
        }
    }
}
