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
    public partial class Pengembalian : Form
    {
        SqlConnection connection = new SqlConnection(Connection.connectionString);
        SqlCommand command;
        SqlDataAdapter adapter;
        DataTable dt;
        int id, telat;

        public Pengembalian()
        {
            InitializeComponent();
            loadDataPeminjaman();
            toolTip.SetToolTip(buttonKembali, "Kembalikan Buku");
            toolTip.SetToolTip(buttonK, "Kembalikan Buku");
            toolTip.SetToolTip(buttonRiwayat, "Riwayat Peminjaman");
            toolTip.SetToolTip(buttonR, "Riwayat Peminjaman");
        }

        void loadDataPeminjaman()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("select p.id_pinjam, b.kode_buku, b.judul, k.nama_kategori, a.nama_lengkap, pe.nama_petugas, pb.tgl_pinjam, pb.tgl_kembali, pb.id from peminjaman p inner join petugas pe on p.id_petugas = pe.id_petugas inner join anggota a on p.id_anggota = a.id_anggota inner join peminjaman_buku pb on p.id_pinjam = pb.id_pinjam inner join buku b on pb.kode_buku = b.kode_buku inner join kategori k on b.id_kategori = k.id_kategori where pb.tgl_kembali_real is null order by pb.tgl_pinjam desc", connection);
                dt = new DataTable();
                adapter.Fill(dt);

                gridPengembalian.DataSource = dt;

                gridPengembalian.Columns[0].Visible = false;
                gridPengembalian.Columns[3].Visible = false;
                gridPengembalian.Columns[5].Visible = false;
                gridPengembalian.Columns[8].Visible = false;

                gridPengembalian.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                gridPengembalian.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                gridPengembalian.Columns[1].HeaderText = "Kode Buku";
                gridPengembalian.Columns[2].HeaderText = "Judul";
                gridPengembalian.Columns[3].HeaderText = "Kategori";
                gridPengembalian.Columns[4].HeaderText = "Anggota";
                gridPengembalian.Columns[6].HeaderText = "Tanggal Peminjaman";
                gridPengembalian.Columns[7].HeaderText = "Tanggal Pengembalian";

                gridPengembalian.Columns[6].DefaultCellStyle.Format = "dddd, dd MMMM yyyy";
                gridPengembalian.Columns[7].DefaultCellStyle.Format = "dddd, dd MMMM yyyy";
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

        private void Pengembalian_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Solid);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gridPengembalian_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DateTime peminjaman = Convert.ToDateTime(gridPengembalian.SelectedRows[0].Cells[6].Value);
                DateTime pengembalian = Convert.ToDateTime(gridPengembalian.SelectedRows[0].Cells[7].Value);

                textBoxp.Text = gridPengembalian.SelectedRows[0].Cells[0].Value.ToString();
                textBoxj.Text = gridPengembalian.SelectedRows[0].Cells[2].Value.ToString();
                textBox3.Text = gridPengembalian.SelectedRows[0].Cells[4].Value.ToString();
                textBoxTp.Text = peminjaman.ToString("dddd, dd MMMM yyyy");
                textBoxTk.Text = pengembalian.ToString("dddd, dd MMMM yyyy");
                id = Convert.ToInt32(gridPengembalian.SelectedRows[0].Cells[8].Value);

                if (pengembalian.Date < DateTime.Now.Date)
                    telat = Convert.ToInt32((DateTime.Now.Date - pengembalian.Date).TotalDays);
                else
                    telat = 0;

                textBox2.Text = (telat * 2250).ToString();
            }
            catch (Exception) { }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            loadDataPeminjaman();
            DataView dv = new DataView(dt);
            dv.RowFilter = String.Format("nama_lengkap LIKE '%{0}%' OR nama_petugas LIKE '%{0}%' OR judul LIKE '%{0}%' OR nama_kategori LIKE '%{0}%' OR penulis LIKE '%{0}%'", textBox1.Text);
            gridPengembalian.DataSource = dv;
        }

        private void buttonKembali_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Open();

                if (id == 0)
                {
                    DialogTemplate dialog = new DialogTemplate("Peringatan", "Pilih terlebih dahulu data yang ingin dikembalikan!", MessageBoxButtons.OK);
                    dialog.ShowDialog();
                }
                else
                {
                    command = new SqlCommand("EXEC pengembalian @id = " + id + ", @tgl_kembali = '" + DateTime.Now + "', @denda = " + Convert.ToInt32(textBox2.Text) + ", @jml_hari_telat = '" + telat + "'", connection);
                    command.ExecuteNonQuery();

                    DialogTemplate dialog = new DialogTemplate("Berhasil", "Buku telah dikembalikan!", MessageBoxButtons.OK);
                    dialog.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                DialogTemplate dialog = new DialogTemplate("Error", "Maaf, terjadi kesalahan!", MessageBoxButtons.OK);
                dialog.ShowDialog();
            }
            finally
            {
                connection.Close();
                loadDataPeminjaman();
            }
        }

        private void buttonRiwayat_Click(object sender, EventArgs e)
        {
            RiwayatPengembalian riwayat = new RiwayatPengembalian();
            riwayat.ShowDialog();
        }
    }
}
