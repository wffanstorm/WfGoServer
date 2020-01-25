using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wfGoServer
{
    public partial class Form1 : Form
    {
        wfGoServer server;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ListBox1.Items.Clear();
            ListView1.Items.Clear();
            LblState.Text = "False";
            LblOnLineNum.Text = "0";

            TxtInfo.Text = "Being happy with the GO！";
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            server = new wfGoServer(this);
            server.Start();
        }

        private void BtnEnd_Click(object sender, EventArgs e)
        {
            server.End();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(LblState.Text=="True")
            {
                MessageBox.Show("请点击【关闭服务器】按钮！");
                e.Cancel = true;
            }
        }
    }
}
