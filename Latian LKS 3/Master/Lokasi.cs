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
    public partial class Lokasi : Form
    {
        SqlConnection connection = new SqlConnection(Connection.connectionString);
        SqlCommand command;
        SqlDataAdapter adapter;
        SqlDataReader reader;
        DataTable dt;
        string kode;

        public Lokasi()
        {
            InitializeComponent();
            loadDataLokasi();
            hideButton();
        }

        void loadDataLokasi()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("SELECT * FROM Lokasi", connection);
                dt = new DataTable();
                adapter.Fill(dt);

                gridLokasi.DataSource = dt;
                gridLokasi.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                gridLokasi.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                gridLokasi.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridLokasi.Columns[0].HeaderText = "Kode Lokasi";

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
             if (textBox2.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Label harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (textBox3.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Lantai harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (textBox4.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Rak harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = char.IsLetter(e.KeyChar) && char.IsWhiteSpace(e.KeyChar);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void buttonTambah_Click(object sender, EventArgs e)
        {
            try
            {
                if (validation())
                {
                    connection.Open();
                    
                    command = new SqlCommand("EXEC insert_lokasi @label = @label_lokasi, @lantai = @lantai_lokasi, @rak = @rak_lokasi", connection);
                    command.Parameters.AddWithValue("@label_lokasi", textBox2.Text);
                    command.Parameters.AddWithValue("@lantai_lokasi", Convert.ToInt32(textBox3.Text));
                    command.Parameters.AddWithValue("@rak_lokasi", textBox4.Text);
                    command.CommandType = CommandType.Text;
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
                loadDataLokasi();
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
                        command = new SqlCommand("EXEC update_lokasi @kode_lokasi = @kode, @label = @label_lokasi, @lantai = @lantai_lokasi, @rak = @rak_lokasi", connection);
                        command.Parameters.AddWithValue("@kode", kode);
                        command.Parameters.AddWithValue("@label_lokasi", textBox2.Text);
                        command.Parameters.AddWithValue("@lantai_lokasi", Convert.ToInt32(textBox3.Text));
                        command.Parameters.AddWithValue("@rak_lokasi", textBox4.Text);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();

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
                loadDataLokasi();
            }
        }

        private void buttonHapus_Click(object sender, EventArgs e)
        {
            try
            {
                DialogTemplate dialogKonfirmasi = new DialogTemplate("Peringatan", "Apakah anda yakin ingin mengubah data ini!", MessageBoxButtons.YesNo);
                dialogKonfirmasi.ShowDialog();

                if (dialogKonfirmasi.DialogResult == DialogResult.Yes)
                {
                    connection.Open();

                    command = new SqlCommand("EXEC delete_lokasi @kode_lokasi = @kode", connection);
                    command.Parameters.AddWithValue("@kode", kode);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();

                    hideButton();

                    DialogTemplate dialog = new DialogTemplate("Berhasil", "Berhasil diubah!", MessageBoxButtons.OK);
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
                loadDataLokasi();
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            loadDataLokasi();
            DataView dataView = new DataView(dt);
            dataView.RowFilter = String.Format("kode_lokasi LIKE '%{0}%' OR label LIKE '%{0}%' OR rak LIKE '%{0}%'", textBox5.Text);
            gridLokasi.DataSource = dataView;
        }

        private void gridLokasi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                kode = Convert.ToString(gridLokasi.SelectedRows[0].Cells[0].Value);
                textBox2.Text = Convert.ToString(gridLokasi.SelectedRows[0].Cells[1].Value);
                textBox3.Text = Convert.ToString(gridLokasi.SelectedRows[0].Cells[2].Value);
                textBox4.Text = Convert.ToString(gridLokasi.SelectedRows[0].Cells[3].Value);
                showButton();
            }
            catch (Exception) { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Guid guid = Guid.NewGuid();
            string kode = Convert.ToBase64String(guid.ToByteArray());
            kode = kode.Replace("/", "");
            kode = kode.Replace("+", "");
            kode = kode.Replace("=", "");
            kode = kode.Replace("-", "");
            kode = kode.Substring(0, 9);
        }

        private void Lokasi_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Solid);
        }
    }
}
