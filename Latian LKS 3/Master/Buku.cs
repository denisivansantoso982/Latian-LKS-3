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

namespace Latian_LKS_3.Master
{
    public partial class Buku : Form
    {
        SqlConnection connection = new SqlConnection(Connection.connectionString);
        SqlCommand command;
        SqlDataAdapter adapter;
        DataTable dt, dtLokasi, dtKategori;
        string kode;

        public Buku()
        {
            InitializeComponent();
            loadLokasi();
            loadKategori();
            loadDataBuku();
            hideButton();
        }

        void loadDataBuku()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("select b.*, k.nama_kategori, l.label, l.lantai from buku as b inner join lokasi as l on b.kode_lokasi = l.kode_lokasi inner join kategori as k on b.id_kategori = k.id_kategori", connection);
                dt = new DataTable();
                adapter.Fill(dt);

                gridBuku.DataSource = dt;

                gridBuku.Columns[0].Visible = false;
                gridBuku.Columns[1].Visible = false;
                gridBuku.Columns[2].Visible = false;
                gridBuku.Columns[6].Visible = false;

                gridBuku.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                gridBuku.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridBuku.Columns[7].DefaultCellStyle.Font = new Font(gridBuku.Font, FontStyle.Bold);
                gridBuku.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridBuku.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                gridBuku.Columns[9].HeaderText = "Kategori";
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

        void loadLokasi()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("SELECT * FROM lokasi", connection);
                dtLokasi = new DataTable();
                adapter.Fill(dtLokasi);

                comboBox2.DataSource = dtLokasi;
                comboBox2.DisplayMember = "label";
                comboBox2.ValueMember = "kode_lokasi";

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

        void loadKategori()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("SELECT * FROM kategori", connection);
                dtKategori = new DataTable();
                adapter.Fill(dtKategori);

                comboBox1.DataSource = dtKategori;
                comboBox1.DisplayMember = "nama_kategori";
                comboBox1.ValueMember = "id_kategori";
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

        void hideButton()
        {
            buttonUbah.Visible = false;
            buttonU.Visible = false;
            buttonHapus.Visible = false;
            buttonH.Visible = false;
        }

        void showButton()
        {
            buttonUbah.Visible = true;
            buttonU.Visible = true;
            buttonHapus.Visible = true;
            buttonH.Visible = true;
        }

        bool validation()
        {
            if (textBox1.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Judul harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (textBox2.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Penulis harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (textBox3.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Penerbit harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (textBox4.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Harga harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (textBox5.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Stok harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (richTextBox1.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Deskripsi harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }

            return true;
        }

        private void buttonTambah_Click(object sender, EventArgs e)
        {
            try
            {
                if (validation())
                {
                    connection.Open();

                    string kategori = comboBox1.SelectedValue.ToString();
                    string lokasi = comboBox2.SelectedValue.ToString();
                    int harga = Convert.ToInt32(textBox4.Text);
                    int stok = Convert.ToInt32(textBox5.Text);
                    command = new SqlCommand("EXEC insert_buku @kode_lokasi = '" + lokasi + "', @id_kategori = '" + kategori + "', @judul = '" + textBox1.Text + "', @penulis = '" + textBox2.Text + "', @penerbit = '" + textBox3.Text + "', @deskripsi = '" + richTextBox1.Text + "', @harga = " + harga + ", @stok = " + stok, connection);
                    command.ExecuteNonQuery();

                    hideButton();

                    DialogTemplate dialog = new DialogTemplate("Berhasil", "Berhasil ditambahkan!", MessageBoxButtons.OK);
                    dialog.ShowDialog();

                }
            }
            catch (Exception)
            {
                DialogTemplate dialog = new DialogTemplate("Error", "Maaf, terjadi kesalahan!", MessageBoxButtons.OK);
                dialog.ShowDialog();
            }
            finally
            {
                connection.Close();
                loadDataBuku();
            }
        }

        private void buttonUbah_Click(object sender, EventArgs e)
        {
            try
            {
                if (validation())
                {
                    connection.Open();

                    DialogTemplate dialogKonfirmasi = new DialogTemplate("Peringatan", "Apakah anda yakin ingin mengubah data ini!", MessageBoxButtons.YesNo);
                    dialogKonfirmasi.ShowDialog();

                    if (dialogKonfirmasi.DialogResult == DialogResult.Yes)
                    {
                        string kategori = comboBox1.SelectedValue.ToString();
                        string lokasi = comboBox2.SelectedValue.ToString();
                        int harga = Convert.ToInt32(textBox4.Text);
                        int stok = Convert.ToInt32(textBox5.Text);
                        command = new SqlCommand("EXEC update_buku @kode_buku = '" + kode + "', @kode_lokasi = '" + lokasi + "', @id_kategori = '" + kategori + "', @judul = '" + textBox1.Text + "', @penulis = '" + textBox2.Text + "', @penerbit = '" + textBox3.Text + "', @deskripsi = '" + richTextBox1.Text + "', @harga = " + harga + ", @stok = " + stok, connection);
                        command.ExecuteNonQuery();

                        kode = null;
                        hideButton();

                        DialogTemplate dialog = new DialogTemplate("Berhasil", "Berhasil diubah!", MessageBoxButtons.OK);
                        dialog.ShowDialog();
                    }

                }
            }
            catch (Exception)
            {
                DialogTemplate dialog = new DialogTemplate("Error", "Maaf, terjadi kesalahan!", MessageBoxButtons.OK);
                dialog.ShowDialog();
            }
            finally
            {
                connection.Close();
                loadDataBuku();
            }
        }

        private void buttonHapus_Click(object sender, EventArgs e)
        {
            try
            {
                if (validation())
                {
                    connection.Open();

                    DialogTemplate dialogKonfirmasi = new DialogTemplate("Peringatan", "Apakah anda yakin ingin menghapus data ini!", MessageBoxButtons.YesNo);
                    dialogKonfirmasi.ShowDialog();

                    if (dialogKonfirmasi.DialogResult == DialogResult.Yes)
                    {
                        command = new SqlCommand("EXEC delete_buku @kode_buku = '" + kode + "'", connection);
                        command.ExecuteNonQuery();

                        kode = null;
                        hideButton();

                        DialogTemplate dialog = new DialogTemplate("Berhasil", "Berhasil dihapus!", MessageBoxButtons.OK);
                        dialog.ShowDialog();
                    }
                }
            }
            catch (Exception)
            {
                DialogTemplate dialog = new DialogTemplate("Error", "Maaf, terjadi kesalahan!", MessageBoxButtons.OK);
                dialog.ShowDialog();
            }
            finally
            {
                connection.Close();
                loadDataBuku();
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            loadDataBuku();
            DataView dv = new DataView(dt);
            dv.RowFilter = String.Format("judul LIKE '%{0}%' OR penerbit LIKE '%{0}%' OR penulis LIKE '%{0}%' OR nama_kategori LIKE '%{0}%'", textBox6.Text);
            gridBuku.DataSource = dv;
        }

        private void gridBuku_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                kode = gridBuku.SelectedRows[0].Cells[0].Value.ToString();
                comboBox1.SelectedValue = gridBuku.SelectedRows[0].Cells[2].Value;
                comboBox2.SelectedValue = gridBuku.SelectedRows[0].Cells[1].Value;
                textBox1.Text = gridBuku.SelectedRows[0].Cells[3].Value.ToString();
                textBox2.Text = gridBuku.SelectedRows[0].Cells[4].Value.ToString();
                textBox3.Text = gridBuku.SelectedRows[0].Cells[5].Value.ToString();
                richTextBox1.Text = gridBuku.SelectedRows[0].Cells[6].Value.ToString();
                textBox4.Text = gridBuku.SelectedRows[0].Cells[7].Value.ToString();
                textBox5.Text = gridBuku.SelectedRows[0].Cells[8].Value.ToString();

                showButton();
            }
            catch (Exception)
            {

            }
        }

        private void Buku_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Solid);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
