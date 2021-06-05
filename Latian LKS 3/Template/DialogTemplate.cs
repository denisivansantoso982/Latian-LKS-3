using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Latian_LKS_3.Template
{
    public partial class DialogTemplate : Form
    {
        public DialogTemplate(string title, string message, MessageBoxButtons dialogButton)
        {
            InitializeComponent();
            label4.Text = title;
            label1.Text = message;

            if (dialogButton.Equals(MessageBoxButtons.YesNo))
            {
                button2.Visible = false;
            }
            else if (dialogButton.Equals(MessageBoxButtons.OK))
            {
                button3.Visible = false;
                button4.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DialogTemplate_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.FromArgb(4, 50, 110), ButtonBorderStyle.Solid);
        }
    }
}
