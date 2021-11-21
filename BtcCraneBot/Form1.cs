using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BtcCraneBot
{
    public partial class Form1 : Form
    {
        BackgroundWorker bw;
        public static Boolean botRun = false;
        public Form1()
        {
            InitializeComponent();
            var botWorker = new ApiConnect();
            this.bw = new BackgroundWorker();
            this.bw.DoWork += botWorker.MessageReader;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var apiKey = apiKeyBox.Text;

            if (this.bw.IsBusy != true)
            {
                this.bw.RunWorkerAsync(apiKey);
            }
        }
    }
}
