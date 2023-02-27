using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PowerBiMonitor_Scheduler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void CollectUserActivity_Click(object sender, EventArgs e)
        {
            Scheduler scheduler = new Scheduler();
            scheduler.CollectUserActivities();


        }

        private void SendTestMessage(object sender, EventArgs e)
        {
            Scheduler scheduler = new Scheduler();
            scheduler.SendEmailWIthGeneratedReport( 2);
        }
    }
}
