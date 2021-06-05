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
    public partial class Petugas : Form
    {
        SqlConnection connection = new SqlConnection(Connection.connectionString);
        SqlCommand command;
        SqlDataAdapter adapter;
        SqlDataReader reader;
        DataTable dt, dtUsername;
        string id;
        int id_user;

        public Petugas()
        {
            InitializeComponent();
            loadDataPetugas();
            loadUsername();
            hideButton();
        }

        void loadDataPetugas()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("select p.*, u.username from [user] u inner join petugas p on u.id_user = p.id_user where u.level = 'Admin'", connection);
                dt = new DataTable();
                adapter.Fill(dt);

                gridPetugas.DataSource = dt;

                gridPetugas.Columns[0].Visible = false;
                gridPetugas.Columns[4].Visible = false;

                gridPetugas.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                gridPetugas.Columns[1].HeaderText = "Nama";
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

        void loadUsername()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("select u.id_user, u.username from [user] u left join petugas p on u.id_user = p.id_user where p.id_petugas is null and u.level = 'Admin' or u.id_user = " + id_user, connection);
                dtUsername = new DataTable();
                adapter.Fill(dtUsername);

                comboBox.DataSource = dtUsername;
                comboBox.DisplayMember = "username";
                comboBox.ValueMember = "id_user";
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

        bool validation(bool isForInsert)
        {
            if (textBox1.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Nama harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (textBox2.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "NIK harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (richTextBox.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Alamat harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (comboBox.SelectedValue == null)
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Akun pengguna tidak! Harap mengisi di form Pengguna!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }

            else if (isForInsert)
            {
                if (!sameNIK())
                {
                    DialogTemplate dialog = new DialogTemplate("Informasi", "NIK yang anda gunakan harus berbeda dengan pengguna lainnya!", MessageBoxButtons.OK);
                    dialog.ShowDialog();
                    return false;
                }
            }

            return true;
        }

        bool sameNIK()
        {
            try
            {
                connection.Open();

                command = new SqlCommand("SELECT * FROM petugas WHERE nik = '" + textBox2.Text + "'", connection);
                reader = command.ExecuteReader();
                reader.Read();

                if (reader.HasRows)
                {
                    reader.Close();
                    connection.Close();
                    return false;
                }
                else
                {
                    reader.Close();

                    command = new SqlCommand("SELECT * FROM anggota WHERE nik = '" + textBox2.Text + "'", connection);
                    reader = command.ExecuteReader();
                    reader.Read();

                    if (reader.HasRows)
                    {
                        reader.Close();
                        connection.Close();
                        return false;
                    }

                    connection.Close();
                    return true;
                }
            }
            catch (Exception)
            {
                DialogTemplate dialog = new DialogTemplate("Error", "Maaf, terjadi kesalahan!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void buttonTambah_Click(object sender, EventArgs e)
        {
            try
            {
                if (validation(true))
                {
                    connection.Open();

                    command = new SqlCommand("EXEC insert_petugas @id_user = '" + comboBox.SelectedValue.ToString() + "', @nama = '" + textBox1.Text + "', @nik = '" + textBox2.Text + "', @alamat = '" + richTextBox.Text + "'", connection);
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
                loadDataPetugas();
                loadUsername();
            }
        }

        private void buttonUbah_Click(object sender, EventArgs e)
        {
            try
            {
                if (validation(false))
                {
                    connection.Open();

                    DialogTemplate dialogKonfirmasi = new DialogTemplate("Peringatan", "Apakah anda yakin ingin mengubah data ini!", MessageBoxButtons.YesNo);
                    dialogKonfirmasi.ShowDialog();

                    if (dialogKonfirmasi.DialogResult == DialogResult.Yes)
                    {
                        command = new SqlCommand("EXEC update_petugas @id_petugas = '" + id + "', @id_user = '" + comboBox.SelectedValue.ToString() + "', @nama = '" + textBox1.Text + "', @nik = '" + textBox2.Text + "', @alamat = '" + richTextBox.Text + "'", connection);
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
                loadDataPetugas();
                loadUsername();
            }
        }

        private void buttonHapus_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Open();

                DialogTemplate dialogKonfirmasi = new DialogTemplate("Peringatan", "Apakah anda yakin ingin menghapus data ini!", MessageBoxButtons.YesNo);
                dialogKonfirmasi.ShowDialog();

                if (dialogKonfirmasi.DialogResult == DialogResult.Yes)
                {
                    command = new SqlCommand("EXEC delete_petugas @id_petugas = '" + id + "'", connection);
                    command.ExecuteNonQuery();

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
                loadDataPetugas();
                loadUsername();
            }
        }

        private void gridPetugas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                id_user = Convert.ToInt32(gridPetugas.SelectedRows[0].Cells[4].Value);
                loadUsername();

                id = gridPetugas.SelectedRows[0].Cells[0].Value.ToString();
                textBox1.Text = gridPetugas.SelectedRows[0].Cells[1].Value.ToString();
                textBox2.Text = gridPetugas.SelectedRows[0].Cells[2].Value.ToString();
                richTextBox.Text = gridPetugas.SelectedRows[0].Cells[3].Value.ToString();
                comboBox.SelectedValue = id_user;
            }
            catch (Exception)
            {

            }
        }

        private void Petugas_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Solid);
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            loadDataPetugas();
            DataView dataView = new DataView(dt);
            dataView.RowFilter = String.Format("nama_petugas LIKE '%{0}%' OR nik LIKE '%{0}%' OR username LIKE '%{0}%'", textBox6.Text);
            gridPetugas.DataSource = dataView;
        }
    }
}
