using System;
using System.Windows.Forms;

namespace Csevetest {
    public partial class Help : Form {
        public Help(string version) {
            InitializeComponent();

            label1.Text += version;
        }

        private void Help_Load(object sender, EventArgs e) {
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Escape)
                this.Close();
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
