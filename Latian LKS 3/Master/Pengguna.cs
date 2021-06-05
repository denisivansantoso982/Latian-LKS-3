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
    public partial class Pengguna : Form
    {
        SqlConnection connection = new SqlConnection(Connection.connectionString);
        SqlCommand command;
        SqlDataAdapter adapter;
        SqlDataReader reader;
        DataTable dt, dtUsername;
        string username;
        int id;

        public Pengguna()
        {
            InitializeComponent();
            loadDataPengguna();
            loadLevel();
            hideButton();
        }

        void loadDataPengguna()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("select * from [user] ORDER BY [level] ASC", connection);
                dt = new DataTable();
                adapter.Fill(dt);

                gridPetugas.DataSource = dt;

                gridPetugas.Columns[0].Visible = false;
                gridPetugas.Columns[2].Visible = false;
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

        void loadLevel()
        {
            comboBox.Items.Insert(0, "Admin");
            comboBox.Items.Insert(1, "Anggota");
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
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Username harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (comboBox.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Harap pilih level anda!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (textBox2.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Password harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (textBox3.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Konfirmasi Password harus diisi!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (textBox3.Text != textBox2.Text)
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Konfirmasi Password dan Password tidak sesuai!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (isForInsert)
            {
                if (sameUsername())
                {
                    DialogTemplate dialog = new DialogTemplate("Informasi", "Username yang anda gunakan harus berbeda dengan pengguna lainnya!", MessageBoxButtons.OK);
                    dialog.ShowDialog();
                    return false;
                }
            }
            else if (!isForInsert)
            {
                if (!sameUsernameWhileUpdate())
                {
                    if (sameUsername())
                    {
                        DialogTemplate dialog = new DialogTemplate("Informasi", "Username yang anda gunakan harus berbeda dengan pengguna lainnya!", MessageBoxButtons.OK);
                        dialog.ShowDialog();
                        return false;
                    }
                }
            }
            else if (!isForInsert)
            {
                if (!samePassword())
                {
                    DialogTemplate dialog = new DialogTemplate("Informasi", "Password yang anda masukkan salah!", MessageBoxButtons.OK);
                    dialog.ShowDialog();
                    return false;
                }
            }

            return true;
        }

        bool sameUsername()
        {
            try
            {
                connection.Open();

                command = new SqlCommand("SELECT * FROM [user] WHERE username = '" + textBox1.Text + "'", connection);
                reader = command.ExecuteReader();
                reader.Read();

                if (reader.HasRows)
                {
                    reader.Close();
                    connection.Close();
                    return true;
                }

                reader.Close();
                connection.Close();
                return false;
            }
            catch (Exception)
            {
                DialogTemplate dialog = new DialogTemplate("Error", "Maaf, terjadi kesalahan!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
        }

        bool sameUsernameWhileUpdate()
        {
            if (textBox1.Text != username)
            {
                return false;
            }

            return true;
        }

        bool samePassword()
        {
            try
            {
                connection.Open();

                string pass = EncryptionModel.encrypt(textBox3.Text);
                command = new SqlCommand("SELECT * FROM [user] WHERE password = '" + pass + "'", connection);
                reader = command.ExecuteReader();
                reader.Read();

                if (reader.HasRows)
                {
                    reader.Close();
                    connection.Close();
                    return false;
                }

                reader.Close();
                connection.Close();
                return true;
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

        private void buttonTambah_Click(object sender, EventArgs e)
        {
            try
            {
                if (validation(true))
                {
                    connection.Open();

                    string pass = EncryptionModel.encrypt(textBox3.Text);
                    command = new SqlCommand("EXEC insert_pengguna @username = '" + textBox1.Text + "', @password = '" + pass + "', @level = '" + comboBox.Text + "'", connection);
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
                loadDataPengguna();
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
                        string pass = EncryptionModel.encrypt(textBox3.Text);
                        command = new SqlCommand("EXEC update_pengguna @id_user = " + id + ", @username = '" + textBox1.Text + "', @password = '" + pass + "', @level = '" + comboBox.Text + "'", connection);
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
                loadDataPengguna();
            }
        }

        private void buttonHapus_Click(object sender, EventArgs e)
        {
            try
            {
                if (validation(true))
                {
                    connection.Open();

                    DialogTemplate dialogKonfirmasi = new DialogTemplate("Peringatan", "Apakah anda yakin ingin menghapus data ini!", MessageBoxButtons.YesNo);
                    dialogKonfirmasi.ShowDialog();

                    if (dialogKonfirmasi.DialogResult == DialogResult.Yes)
                    {

                        command = new SqlCommand("EXEC delete_pengguna @id_user = " + id, connection);
                        command.ExecuteNonQuery();

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
                loadDataPengguna();
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            loadDataPengguna();
            DataView dataView = new DataView(dt);
            dataView.RowFilter = String.Format("username LIKE '%{0}%' OR level LIKE '%{0}%'", textBox6.Text);
            gridPetugas.DataSource = dataView;
        }

        private void gridPetugas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                id = Convert.ToInt32(gridPetugas.SelectedRows[0].Cells[0].Value);
                username = gridPetugas.SelectedRows[0].Cells[1].Value.ToString();
                textBox1.Text = username;
                comboBox.Text = gridPetugas.SelectedRows[0].Cells[3].Value.ToString();

                showButton();
            }
            catch (Exception) { }
        }

        private void Pengguna_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Solid);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.UseSystemPasswordChar = false;
                textBox3.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
                textBox3.UseSystemPasswordChar = true;
            }
        }
    }
}
