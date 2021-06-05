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

namespace Latian_LKS_3
{
    public partial class Kategori : Form
    {
        SqlConnection connection = new SqlConnection(Connection.connectionString);
        SqlCommand command = new SqlCommand();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt;
        int id;

        public Kategori()
        {
            InitializeComponent();
            loadDataKategori();
            hideButton();
        }

        void loadDataKategori()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("SELECT * FROM kategori", connection);
                dt = new DataTable();
                adapter.Fill(dt);

                gridKat.DataSource = dt;

                gridKat.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                gridKat.Columns[0].HeaderText = "Kode Lokasi";

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
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Nama Kategori harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gridKat_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                id = Convert.ToInt32(gridKat.SelectedRows[0].Cells[0].Value);
                textBox1.Text = Convert.ToString(gridKat.SelectedRows[0].Cells[1].Value);
                showButton();
            }
            catch (Exception) { }
        }

        private void buttonTambah_Click(object sender, EventArgs e)
        {
            try
            {
                if (validation())
                {
                    connection.Open();

                    command = new SqlCommand("EXEC insert_kategori @nama_kategori = @nama", connection);
                    command.Parameters.AddWithValue("@nama", textBox1.Text);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                    id = 0;
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
                loadDataKategori();
            }
        }

        private void buttonUbah_Click(object sender, EventArgs e)
        {
            try
            {
                if (validation())
                {
                    DialogTemplate dialogKonfirmasi = new DialogTemplate("Peringatan", "Apakah anda yakin ingin mengubah data ini!", MessageBoxButtons.YesNo);
                    dialogKonfirmasi.ShowDialog();

                    if (dialogKonfirmasi.DialogResult == DialogResult.Yes)
                    {
                        connection.Open();

                        command = new SqlCommand("EXEC update_kategori  @id_kategori = @id, @nama_kategori = @nama", connection);
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@nama", textBox1.Text);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();

                        id = 0;
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
                loadDataKategori();
            }
        }

        private void buttonHapus_Click(object sender, EventArgs e)
        {
            try
            {

                DialogTemplate dialogKonfirmasi = new DialogTemplate("Peringatan", "Apakah anda yakin ingin manghapus data ini!", MessageBoxButtons.YesNo);
                dialogKonfirmasi.ShowDialog();

                if (dialogKonfirmasi.DialogResult == DialogResult.Yes)
                {
                    connection.Open();

                    command = new SqlCommand("EXEC delete_kategori @id_kategori = @id", connection);
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                    id = 0;
                    hideButton();

                    DialogTemplate dialog = new DialogTemplate("Berhasil", "Berhasil dihapus!", MessageBoxButtons.OK);
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
                loadDataKategori();
            }
        }

        private void Kategori_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Solid);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                buttonTambah.PerformClick();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            loadDataKategori();
            DataView dv = new DataView(dt);
            dv.RowFilter = String.Format("nama_kategori LIKE '%{0}%'", textBox2.Text);
            gridKat.DataSource = dv;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = textBox1.TextLength > 100;
        }
    }
}
