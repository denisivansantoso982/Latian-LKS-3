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
    public partial class ReportTemplate : Form
    {
        DataTable data;

        public ReportTemplate(DataTable dt)
        {
            InitializeComponent();
            data = dt;   
        }

        private void ReportTemplate_Load(object sender, EventArgs e)
        {
            Report report = new Report();
            crystalReportViewer.ReportSource = report;
            report.SetDataSource(data);
            report.SetParameterValue("date", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
        }
    }
}
