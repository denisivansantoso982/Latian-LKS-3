using Latian_LKS_3.Master;
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
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
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

        private void timer_Tick(object sender, EventArgs e)
        {
            labelTimer.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panelKategori_Click(object sender, EventArgs e)
        {
            Kategori kategori = new Kategori();
            kategori.ShowDialog();
        }

        private void panelLokasi_Click(object sender, EventArgs e)
        {
            Lokasi lokasi = new Lokasi();
            lokasi.ShowDialog();
        }

        private void panelBuku_Click(object sender, EventArgs e)
        {
            Buku buku = new Buku();
            buku.ShowDialog();
        }

        private void panelPetugas_Click(object sender, EventArgs e)
        {
            Petugas petugas = new Petugas();
            petugas.ShowDialog();
        }

        private void panelAnggota_Click(object sender, EventArgs e)
        {
            Anggota anggota = new Anggota();
            anggota.ShowDialog();
        }

        private void panelPengguna_Click(object sender, EventArgs e)
        {
            Pengguna pengguna = new Pengguna();
            pengguna.ShowDialog();
        }

        private void panelPinjam_Click(object sender, EventArgs e)
        {
            DataPeminjaman dataPeminjaman = new DataPeminjaman();
            dataPeminjaman.ShowDialog();
        }

        private void panelKembali_Click(object sender, EventArgs e)
        {
            Pengembalian pengembalian = new Pengembalian();
            pengembalian.ShowDialog();
        }
    }
}
