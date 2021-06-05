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
    public partial class CariBuku : Form
    {
        SqlConnection connection = new SqlConnection(Connection.connectionString);
        SqlCommand command;
        SqlDataAdapter adapter;
        DataTable dt;

        public CariBuku()
        {
            InitializeComponent();
            loadDataBuku();
        }

        void loadDataBuku()
        {
            try
            {
                connection.Open();

                adapter = new SqlDataAdapter("select b.*, k.nama_kategori from buku as b inner join kategori as k on b.id_kategori = k.id_kategori", connection);
                dt = new DataTable();
                adapter.Fill(dt);

                gridBuku.DataSource = dt;

                gridBuku.Columns[0].Visible = false;
                gridBuku.Columns[1].Visible = false;
                gridBuku.Columns[2].Visible = false;
                gridBuku.Columns[6].Visible = false;
                
                gridBuku.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridBuku.Columns[7].DefaultCellStyle.Font = new Font(gridBuku.Font, FontStyle.Bold);
                gridBuku.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CariBuku_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, ColourModel.primary, ButtonBorderStyle.Solid);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            loadDataBuku();
            DataView dv = new DataView(dt);
            dv.RowFilter = String.Format("judul LIKE '%{0}%' OR penerbit LIKE '%{0}%' OR penulis LIKE '%{0}%' OR nama_kategori LIKE '%{0}%'", textBox1.Text);
            gridBuku.DataSource = dv;
        }
    }
}
