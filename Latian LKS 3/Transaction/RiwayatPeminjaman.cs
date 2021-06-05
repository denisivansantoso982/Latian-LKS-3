using Latian_LKS_3.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Latian_LKS_3.Transaction
{
    public partial class RiwayatPeminjaman : Form
    {
        SqlConnection connection = new SqlConnection(Connection.connectionString);
        SqlCommand command;
        SqlDataAdapter adapter;
        DataTable dt;

        public RiwayatPeminjaman()
        {
            InitializeComponent();
            loadDataRiwayat();
        }

        void loadDataRiwayat()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("select p.id_pinjam, b.judul, k.nama_kategori, a.nama_lengkap, pe.nama_petugas, pb.* from peminjaman p inner join petugas pe on p.id_petugas = pe.id_petugas inner join anggota a on p.id_anggota = a.id_anggota inner join peminjaman_buku pb on p.id_pinjam = pb.id_pinjam inner join buku b on pb.kode_buku = b.kode_buku inner join kategori k on b.id_kategori = k.id_kategori where pb.tgl_kembali_real is not null and a.id_anggota = '" + Properties.Settings.Default.id + "' order by pb.tgl_pinjam desc", connection);
                dt = new DataTable();
                adapter.Fill(dt);

                gridRiwayat.DataSource = dt;

                gridRiwayat.Columns[0].Visible = false;
                gridRiwayat.Columns[5].Visible = false;
                gridRiwayat.Columns[6].Visible = false;
                gridRiwayat.Columns[7].Visible = false;
                gridRiwayat.Columns[3].Visible = false;
                gridRiwayat.Columns[10].Visible = false;

                gridRiwayat.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                gridRiwayat.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                gridRiwayat.Columns[1].HeaderText = "Judul";
                gridRiwayat.Columns[2].HeaderText = "Kategori";
                gridRiwayat.Columns[4].HeaderText = "Petugas";
                gridRiwayat.Columns[8].HeaderText = "Tanggal Pinjam";
                gridRiwayat.Columns[9].HeaderText = "Tanggal Kembali";
                gridRiwayat.Columns[11].HeaderText = "Denda (Rp.)";
                gridRiwayat.Columns[12].HeaderText = "Durasi Terlambat";

                gridRiwayat.Columns[8].DefaultCellStyle.Format = "dd MMMM yyyy";
                gridRiwayat.Columns[9].DefaultCellStyle.Format = "dd MMMM yyyy";
            }
            catch (Exception)
            {
                DialogTemplate dialog = new DialogTemplate("Error", "Maaf, terjadi kesalahan!", MessageBoxButtons.OK);
                dialog.ShowDialog();
            }
            finally
            {
                connection.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RiwayatPeminjaman_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Solid);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            loadDataRiwayat();
            DataView dv = new DataView(dt);
            dv.RowFilter = String.Format("judul LIKE '%{0}%' OR nama_kategori LIKE '%{0}%'", textBox1.Text);
            gridRiwayat.DataSource = dv;
        }
    }
}
