using Latian_LKS_3.Template;
using System;
using System.IO;
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
    public partial class Anggota : Form
    {
        SqlConnection connection = new SqlConnection(Connection.connectionString);
        SqlCommand command;
        SqlDataAdapter adapter;
        SqlDataReader reader;
        DataTable dt, dtUsername;
        string id;
        int id_user;

        public Anggota()
        {
            InitializeComponent();
            loadDataAnggota();
            loadUsername();
            dateTimePicker.MaxDate = DateTime.Now;
            hideButton();
        }

        void loadDataAnggota()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("select p.*, u.username from [user] u inner join anggota p on u.id_user = p.id_user where u.level = 'Anggota'", connection);
                dt = new DataTable();
                adapter.Fill(dt);

                gridAnggota.DataSource = dt;

                gridAnggota.Columns[0].Visible = false;
                gridAnggota.Columns[3].Visible = false;
                gridAnggota.Columns[5].Visible = false;
                gridAnggota.Columns[6].Visible = false;

                gridAnggota.Columns[4].DefaultCellStyle.Format = "dddd, dd MMMM yyyy";

                gridAnggota.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                gridAnggota.Columns[1].HeaderText = "Nama";
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

        void loadUsername()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("select u.id_user, u.username from [user] u left join anggota p on u.id_user = p.id_user where p.id_anggota is null and u.level = 'Anggota'  or u.id_user = " + id_user, connection);
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
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Akun pengguna tidak ditemukan! Harap mengisi di form Pengguna!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (pictureBox.Image == null)
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Foto harus diisi!", MessageBoxButtons.OK);
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

                command = new SqlCommand("SELECT * FROM anggota  WHERE nik = '" + textBox2.Text + "'", connection);
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

                    command = new SqlCommand("SELECT * FROM petugas WHERE nik = '" + textBox2.Text + "'", connection);
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
                connection.Close();
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

                    ImageConverter converter = new ImageConverter();
                    byte[] imageVal = (byte[])converter.ConvertTo(pictureBox.Image, typeof(byte[]));

                    command = new SqlCommand("EXEC insert_anggota @id_user = '" + comboBox.SelectedValue.ToString() + "', @nama_lengkap = '" + textBox1.Text + "', @nik = '" + textBox2.Text + "', @alamat = '" + richTextBox.Text + "', @tgl_daftar = '" + dateTimePicker.Value + "', @foto = @foto_a", connection);
                    command.Parameters.AddWithValue("@foto_a", imageVal);
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
                loadDataAnggota();
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
                        ImageConverter converter = new ImageConverter();
                        byte[] imageVal = (byte[])converter.ConvertTo(pictureBox.Image, typeof(byte[]));

                        command = new SqlCommand("EXEC update_anggota @id_anggota = '" + id + "', @id_user = '" + comboBox.SelectedValue.ToString() + "', @nama_lengkap = '" + textBox1.Text + "', @nik = '" + textBox2.Text + "', @alamat = '" + richTextBox.Text + "', @tgl_daftar = '" + dateTimePicker.Value + "', @foto = @foto_a", connection);
                        command.Parameters.AddWithValue("@foto_a", imageVal);
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
                loadDataAnggota();
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
                    command = new SqlCommand("EXEC delete_anggota @id_anggota = '" + id + "'", connection);
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
                loadDataAnggota();
                loadUsername();
            }
        }

        private void buttonCari_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                if (fileDialog.ShowDialog() == DialogResult.OK)
                    pictureBox.Image = Image.FromFile(fileDialog.FileName);
            }
        }

        private void Anggota_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Solid);
        }

        private void gridAnggota_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                id_user = Convert.ToInt32(gridAnggota.SelectedRows[0].Cells[6].Value);
                loadUsername();

                id = gridAnggota.SelectedRows[0].Cells[0].Value.ToString();
                textBox1.Text = gridAnggota.SelectedRows[0].Cells[1].Value.ToString();
                textBox2.Text = gridAnggota.SelectedRows[0].Cells[2].Value.ToString();
                richTextBox.Text = gridAnggota.SelectedRows[0].Cells[3].Value.ToString();
                dateTimePicker.Value = Convert.ToDateTime(gridAnggota.SelectedRows[0].Cells[4].Value);
                comboBox.SelectedValue = id_user;

                byte[] rawImage = (byte[])gridAnggota.SelectedRows[0].Cells[5].Value;
                MemoryStream stream = new MemoryStream(rawImage);
                pictureBox.Image = Image.FromStream(stream);

                showButton();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            loadDataAnggota();
            DataView dv = new DataView(dt);
            dv.RowFilter = String.Format("nama_lengkap LIKE '%{0}%' OR nik LIKE '%{0}%'", textBox6.Text);
            gridAnggota.DataSource = dv;
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, pictureBox.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Dashed);
        }
    }
}
