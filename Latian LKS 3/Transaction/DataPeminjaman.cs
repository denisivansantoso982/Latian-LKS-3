using Latian_LKS_3.Template;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Latian_LKS_3.Transaction
{
    public partial class DataPeminjaman : Form
    {
        SqlConnection connection = new SqlConnection(Connection.connectionString);
        SqlCommand command;
        SqlDataAdapter adapter;
        DataTable dt;

        public DataPeminjaman()
        {
            InitializeComponent();
            loadDataPeminjaman();
            toolTip.SetToolTip(buttonTambah, "Tambah Peminjaman");
            toolTip.SetToolTip(buttonT, "Tambah Peminjaman");;
        }

        public void loadDataPeminjaman()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("select p.id_pinjam, b.kode_buku, b.judul, k.nama_kategori, a.nama_lengkap, pe.nama_petugas, pb.tgl_pinjam, pb.tgl_kembali from peminjaman p inner join petugas pe on p.id_petugas = pe.id_petugas inner join anggota a on p.id_anggota = a.id_anggota inner join peminjaman_buku pb on p.id_pinjam = pb.id_pinjam inner join buku b on pb.kode_buku = b.kode_buku inner join kategori k on b.id_kategori = k.id_kategori where pb.tgl_kembali_real is null order by pb.tgl_pinjam desc", connection);
                dt = new DataTable();
                adapter.Fill(dt);

                gridDataPeminjaman.DataSource = dt;

                gridDataPeminjaman.Columns[0].Visible = false;

                gridDataPeminjaman.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                gridDataPeminjaman.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                gridDataPeminjaman.Columns[1].HeaderText = "Kode Buku";
                gridDataPeminjaman.Columns[2].HeaderText = "Judul";
                gridDataPeminjaman.Columns[3].HeaderText = "Kategori";
                gridDataPeminjaman.Columns[4].HeaderText = "Anggota";
                gridDataPeminjaman.Columns[5].HeaderText = "Petugas";
                gridDataPeminjaman.Columns[6].HeaderText = "Tanggal Peminjaman";
                gridDataPeminjaman.Columns[7].HeaderText = "Tanggal Pengembalian";

                gridDataPeminjaman.Columns[6].DefaultCellStyle.Format = "dddd, dd MMMM yyyy";
                gridDataPeminjaman.Columns[7].DefaultCellStyle.Format = "dddd, dd MMMM yyyy";
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

        private void buttonTambah_Click(object sender, EventArgs e)
        {
            TambahPeminjaman tambahPeminjaman = new TambahPeminjaman(this);
            tambahPeminjaman.ShowDialog();
        }

        private void DataPeminjaman_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Solid);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            loadDataPeminjaman();
            DataView dv = new DataView(dt);
            dv.RowFilter = String.Format("nama_lengkap LIKE '%{0}%' OR nama_petugas LIKE '%{0}%' OR judul LIKE '%{0}%' OR nama_kategori LIKE '%{0}%' OR penulis LIKE '%{0}%'", textBox6.Text);
            gridDataPeminjaman.DataSource = dv;
        }
    }
}
