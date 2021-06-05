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
    public partial class Login : Form
    {
        SqlConnection connection = new SqlConnection(Connection.connectionString);
        SqlCommand command;
        SqlDataReader reader;
        string username, password, nama, alamat, nik, level, id_petugas;
        int salah = 0;

        public Login()
        {
            InitializeComponent();
            panel1.BackColor = ColourModel.primary;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox2.UseSystemPasswordChar = false;
            else
                textBox2.UseSystemPasswordChar = true;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = char.IsWhiteSpace(e.KeyChar);
        }

        private void panelC_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panelC.ClientRectangle, Color.FromArgb(5, 75, 180), ButtonBorderStyle.Dashed);
        }
        

        private void textBoxC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                button3.PerformClick();
        }

        private void textBoxC_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = char.IsWhiteSpace(e.KeyChar);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBoxC.Text == labelC.Text && DateTime.Now >= Properties.Settings.Default.waktu)
            {
                Properties.Settings.Default.username = username;
                Properties.Settings.Default.password = password;
                Properties.Settings.Default.nama = nama;
                Properties.Settings.Default.nik = nik;
                Properties.Settings.Default.alamat = alamat;
                Properties.Settings.Default.level = level;
                Properties.Settings.Default.id = id_petugas;
                Properties.Settings.Default.Save();

                if (level == "Admin" || level == "Petugas")
                {
                    AdminDashboard dashboard = new AdminDashboard();
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    AnggotaDashboard dashboard = new AnggotaDashboard();
                    dashboard.Show();
                    this.Close();
                }

                salah = 0;
            }
            else if (DateTime.Now < Properties.Settings.Default.waktu)
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Anda telah melakukan kesalahan hingga 3 kali! Tunggu 10 detik dari terakhir kali anda melakukan kesalahan!", MessageBoxButtons.OK);
                dialog.ShowDialog();
            }
            else if (textBoxC.Text != labelC.Text)
            {
                salah++;
                if (salah >= 3)
                {
                    Properties.Settings.Default.waktu = DateTime.Now.AddSeconds(10);
                }

                DialogTemplate dialog = new DialogTemplate("Peringatan", "Captcha salah!", MessageBoxButtons.OK);
                dialog.ShowDialog();
            }
        }

        private void randomCaptcha()
        {
            Guid guid = Guid.NewGuid();
            string captcha = Convert.ToBase64String(guid.ToByteArray());
            captcha = captcha.Replace("+", "");
            captcha = captcha.Replace("+", "");
            captcha = captcha.Replace("=", "");
            captcha = captcha.Replace("$", "");
            captcha = captcha.Substring(0, 6);

            Random random = new Random();
            int randomFont = random.Next(1, 5);
            int randomForeColour = random.Next(1, 5);

            switch (randomFont)
            {
                case 1:
                    labelC.Font = new Font("Modern No. 20", 16, FontStyle.Bold);
                    break;
                case 2:
                    labelC.Font = new Font("OptimusPrinceps", 16, FontStyle.Bold);
                    break;
                case 3:
                    labelC.Font = new Font("OCR-A BT", 16, FontStyle.Bold);
                    break;
                case 4:
                    labelC.Font = new Font("Niagara Solid", 16, FontStyle.Bold);
                    break;
                case 5:
                    labelC.Font = new Font("NewsGoth Cn BT", 16, FontStyle.Bold);
                    break;
            }

            switch (randomFont)
            {
                case 1:
                    labelC.ForeColor = Color.Navy;
                    break;
                case 2:
                    labelC.ForeColor = Color.Teal;
                    break;
                case 3:
                    labelC.ForeColor = Color.Magenta;
                    break;
                case 4:
                    labelC.ForeColor = Color.ForestGreen;
                    break;
                case 5:
                    labelC.ForeColor = Color.Purple;
                    break;
            }

            labelC.Text = captcha;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            randomCaptcha();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (validation())
                {
                    connection.Open();

                    string pass = EncryptionModel.encrypt(textBox2.Text);
                    command = new SqlCommand("SELECT * FROM [user] WHERE username = '" + textBox1.Text + "' AND password LIKE '" + pass + "'", connection);
                    reader = command.ExecuteReader();
                    reader.Read();

                    if (reader.HasRows)
                    {
                        int id = Convert.ToInt32(reader[0]);
                        this.level = reader[3].ToString();

                        reader.Close();

                        if (level == "Admin")
                        {
                            command = new SqlCommand("SELECT * FROM petugas WHERE id_user = " + id + "", connection);
                            reader = command.ExecuteReader();
                            reader.Read();

                            this.username = textBox1.Text;
                            this.password = pass;
                            this.id_petugas = reader[0].ToString();
                            this.nama = reader[1].ToString();
                            this.nik = reader[2].ToString();
                            this.alamat = reader[3].ToString();

                            reader.Close();
                            salah = 0;
                        } else
                        {
                            command = new SqlCommand("SELECT * FROM anggota WHERE id_user = " + id + "", connection);
                            reader = command.ExecuteReader();
                            reader.Read();

                            this.username = textBox1.Text;
                            this.password = pass;
                            this.id_petugas = reader[0].ToString();
                            this.nama = reader[1].ToString();
                            this.nik = reader[2].ToString();
                            this.alamat = reader[3].ToString();

                            reader.Close();
                            salah = 0;
                        }
                        
                        panelC.Visible = true;
                        randomCaptcha();
                        textBoxC.Focus();
                    }
                    else
                    {
                        salah++;
                        if (salah >= 3)
                        {
                            Properties.Settings.Default.waktu = DateTime.Now.AddSeconds(10);
                        }

                        DialogTemplate dialog = new DialogTemplate("Peringatan", "Pengguna tidak ditemukan!", MessageBoxButtons.OK);
                        dialog.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                DialogTemplate dialog = new DialogTemplate("Error", "Terjadi Kesalahan!", MessageBoxButtons.OK);
                dialog.ShowDialog();
            }
            finally
            {
                connection.Close();
            }
        }

        private bool validation()
        {
            if (textBox1.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Username tidak boleh kosong!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (textBox2.Text == "")
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Password tidak boleh kosong!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                return false;
            }
            else if (DateTime.Now < Properties.Settings.Default.waktu)
            {
                DialogTemplate dialog = new DialogTemplate("Peringatan", "Anda telah melakukan kesalahan hingga 3 kali! Tunggu 10 detik dari terakhir kali anda melakukan kesalahan!", MessageBoxButtons.OK);
                dialog.ShowDialog();
                salah = 0;
                return false;
            }

            return true;
        }
    }
}
