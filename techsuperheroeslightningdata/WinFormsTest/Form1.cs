using System;
using System.Data;

using System.Windows.Forms;
using TechSuperHeroesLightningData;
using System.Reflection;

namespace WinFormsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string queryString = "Select top 20 * from Person.Person";
            var dbt = new DbTask("aw", queryString, MethodBase.GetCurrentMethod());
            DbWorker.GetDataTable(ref dbt, delegate (DataTable p1, Exception e1)
            {
                dgVw1.DataSource = p1;
                dgVw1.Refresh();
            }
            );
        }
    }
}
