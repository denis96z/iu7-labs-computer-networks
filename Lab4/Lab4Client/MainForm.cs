using System;
using System.Windows.Forms;

namespace Lab4Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void ShowProgress(long numBytes, long sentBytes)
        {
            progressBar.Maximum = (int)numBytes;
            progressBar.Value = (int)sentBytes;
        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar.Value = 0;
                new ClientManager().SendFile(tbFileName.Text, ShowProgress);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                tbFileName.Text = openFileDialog.FileName;
            }
        }
    }
}
