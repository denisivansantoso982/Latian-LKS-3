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
    public partial class TambahPeminjaman : Form
    {
        SqlConnection connection = new SqlConnection(Connection.connectionString);
        SqlCommand command;
        SqlDataAdapter adapter;
        SqlDataReader reader;
        DataTable dtAnggota, dtBuku;
        DataPeminjaman parent;

        public TambahPeminjaman(DataPeminjaman parent)
        {
            InitializeComponent();
            toolTip.SetToolTip(buttonKanan, "Tambah Buku");
            toolTip.SetToolTip(buttonKiri, "Kurangi Buku");
            dateTimePicker.MinDate = DateTime.Now.AddDays(1);
            gridAnggota.ClearSelection();
            loadDataAnggota();
            loadDataBuku();
            configureGridPinjam();
            buttonKiri.Visible = false;
            this.parent = parent;
        }

        void loadDataAnggota()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("select * from anggota", connection);
                dtAnggota = new DataTable();
                adapter.Fill(dtAnggota);

                gridAnggota.DataSource = dtAnggota;

                gridAnggota.Columns[0].Visible = false;
                gridAnggota.Columns[5].Visible = false;
                gridAnggota.Columns[6].Visible = false;

                gridAnggota.Columns[4].DefaultCellStyle.Format = "dddd, dd MMMM yyyy";

                gridAnggota.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                gridAnggota.Columns[1].HeaderText = "Nama";
                gridAnggota.Columns[2].HeaderText = "NIK";
                gridAnggota.Columns[3].HeaderText = "Alamat";
                gridAnggota.Columns[4].HeaderText = "Tanggal Daftar";
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

        void loadDataBuku()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("select b.kode_buku, b.judul, b.penulis, k.nama_kategori from buku as b inner join lokasi as l on b.kode_lokasi = l.kode_lokasi inner join kategori as k on b.id_kategori = k.id_kategori", connection);
                dtBuku = new DataTable();
                adapter.Fill(dtBuku);

                gridBuku.DataSource = dtBuku;

                gridBuku.Columns[0].Visible = false;

                gridBuku.Columns[1].HeaderText = "Judul";
                gridBuku.Columns[2].HeaderText = "Penulis";
                gridBuku.Columns[3].HeaderText = "Kategori";
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

        void configureGridPinjam()
        {
            gridPinjam.Columns.Add("kode_buku", "Kode Buku");
            gridPinjam.Columns.Add("judul", "Judul");
            gridPinjam.Columns.Add("penulis", "Penulis");
            gridPinjam.Columns.Add("nama_kategori", "Kategori");

            gridPinjam.Columns[0].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TambahPeminjaman_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Solid);
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panel3.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Dashed);
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panel4.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Dashed);
        }

        private void buttonSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Open();

                if (gridPinjam.Rows.Count < 1)
                {
                    DialogTemplate dialogs = new DialogTemplate("Peringatan", "Pilih buku terlebih dahulu!", MessageBoxButtons.OK);
                    dialogs.ShowDialog();
                    return;
                }

                string id_anggota = gridAnggota.SelectedRows[0].Cells[0].Value.ToString();
                command = new SqlCommand("EXEC sp_peminjaman @id_anggota = '" + id_anggota + "', @id_petugas = '" + Properties.Settings.Default.id + "'", connection);
                command.ExecuteNonQuery();

                command = new SqlCommand("select TOP 1 * from peminjaman where id_anggota = '" + id_anggota + "' AND id_petugas = '" + Properties.Settings.Default.id + "' order by id_pinjam desc", connection);
                reader = command.ExecuteReader();
                reader.Read();

                int id_pinjam = Convert.ToInt32(reader[0]);
                reader.Close();
                foreach (DataGridViewRow row in gridPinjam.Rows)
                {
                    string kode_buku = row.Cells[0].Value.ToString();

                    command = new SqlCommand("EXEC sp_peminjaman_buku @kode_buku = '" + kode_buku + "', @id_pinjam = " + id_pinjam + ", @tgl_pinjam = '" + DateTime.Now + "', @tgl_kembali = '" + dateTimePicker.Value + "'", connection);
                    command.ExecuteNonQuery();
                }

                gridPinjam.Rows.Clear();

                DialogTemplate dialog = new DialogTemplate("Berhasil", "Data Peminjaman berhasil ditambahkan!", MessageBoxButtons.OK);
                dialog.ShowDialog();

                parent.loadDataPeminjaman();
            }
            catch (Exception)
            {
                DialogTemplate dialog = new DialogTemplate("Error", "Maaf, terjadi kesalahan!", MessageBoxButtons.OK);
                dialog.ShowDialog();
            }
            finally { connection.Close(); }
        }

        private void buttonKanan_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in gridPinjam.Rows)
            {
                if (gridBuku.SelectedRows[0].Cells[0].Value.Equals(row.Cells[0].Value))
                {
                    DialogTemplate dialog = new DialogTemplate("Informasi", "Anda telah meminjam buku yang sama!", MessageBoxButtons.OK);
                    dialog.ShowDialog();
                    return;
                }
            }
            
            foreach(DataGridViewRow row in gridBuku.SelectedRows)
            {
                gridPinjam.Rows.Add(row.Cells[0].Value, row.Cells[1].Value, row.Cells[2].Value, row.Cells[3].Value);
            }

            buttonKiri.Visible = true;
        }

        private void buttonKiri_Click(object sender, EventArgs e)
        {
            gridPinjam.Rows.RemoveAt(gridPinjam.SelectedRows[0].Index);

            if (gridPinjam.Rows.Count == 0)
                buttonKiri.Visible = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            loadDataAnggota();
            DataView dv = new DataView(dtAnggota);
            dv.RowFilter = String.Format("nama_lengkap LIKE '%{0}%' OR nik LIKE '%{0}%'", textBox1.Text);
            gridAnggota.DataSource = dv;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            loadDataBuku();
            DataView dv = new DataView(dtBuku);
            dv.RowFilter = String.Format("judul LIKE '%{0}%' OR penulis LIKE '%{0}%' OR nama_kategori LIKE '%{0}%'", textBox6.Text);
            gridBuku.DataSource = dv;
        }
    }
}
