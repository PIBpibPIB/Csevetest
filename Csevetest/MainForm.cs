using netDxf;
using netDxf.Entities;
using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Csevetest {
    public partial class MainForm : Form {
        readonly string Version = "unknown";

        public double A => (double)numericUpDown_A.Value;
        public double B => (double)numericUpDown_B.Value;
        public double C => (double)numericUpDown_C.Value;
        public double D => (double)numericUpDown_D.Value;
        public double E => (double)numericUpDown_E.Value;
        public double T => (double)numericUpDown_T.Value;
        public double R => (double)numericUpDown_R.Value;

        bool DEVMODE = false;

        public MainForm(string[] args) {
            InitializeComponent();

            if (args.Contains("-dev"))
                DEVMODE = true;

            if (ApplicationDeployment.IsNetworkDeployed) {
                Version = string.Format("{0}", ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4));
                this.Text = string.Format("Csévetest v" + Version);
            }
        }

        private void Button1_Click(object sender, EventArgs e) {
            var path = "";

            if (DEVMODE) {
                path = Application.StartupPath;
            } else {
                var fbd = new FolderBrowserDialog();
                fbd.ShowDialog();
                path = fbd.SelectedPath;
            }

            if (path == "") {
                MessageBox.Show("DXF generálás megszakítva!");
                return;
            }
            if (!Directory.Exists(path)) {
                MessageBox.Show("DXF generálás megszakítva!");
                return;
            }

            GeneratePartNo1(path + "\\1.dxf");
            GeneratePartNo2(path + "\\2.dxf");
            GeneratePartNo3(path + "\\3.dxf");

            if (MessageBox.Show("DXF generálás sikeres!\r\nMappa megnyitása?", "OK!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Process.Start(path);
        }

        void GeneratePartNo1(string fileName) {
            DxfDocument dxf = new DxfDocument();

            AddRectangle(ref dxf, new Vector2(0d, 0), new Vector2(E, B + 2 * T));

            dxf.Save(fileName);
        }
        void GeneratePartNo2(string fileName) {
            DxfDocument dxf = new DxfDocument();
            double y1 = (B + 2 * T) / 2;
            double x1 = (A + 2 * T) / 2;
            AddRectangle(ref dxf, new Vector2(-x1, -y1), new Vector2(x1, y1));

            double y2 = D / 2;
            double x2 = C / 2;
            AddRectangleRound(ref dxf, new Vector2(-x2, -y2), new Vector2(x2, y2), 10);

            dxf.Save(fileName);
            dxf.Save(fileName);
            dxf.Save(fileName);
            dxf.Save(fileName);
            dxf.Save(fileName);
            dxf.Save(fileName);
            dxf.Save(fileName);
        }
        void GeneratePartNo3(string fileName) {
            DxfDocument dxf = new DxfDocument();

            dxf.AddEntity(new Line(new Vector2(A / 2, -E / 2), new Vector2(A / 2, E / 2)));
            dxf.AddEntity(new Line(new Vector2(A / 2, E / 2), new Vector2(A / 2 + R, E / 2)));
            dxf.AddEntity(new Arc(new Vector2(A / 2, E / 2), R, 0, 90));
            dxf.AddEntity(new Line(new Vector2(A / 2, E / 2 + R), new Vector2(-A / 2, E / 2 + R)));
            dxf.AddEntity(new Arc(new Vector2(-A / 2, E / 2), R, 90, 180));
            dxf.AddEntity(new Line(new Vector2(-A / 2 - R, E / 2), new Vector2(-A / 2, E / 2)));
            dxf.AddEntity(new Line(new Vector2(-A / 2, E / 2), new Vector2(-A / 2, -E / 2)));
            dxf.AddEntity(new Line(new Vector2(-A / 2, -E / 2), new Vector2(-A / 2 - R, -E / 2)));
            dxf.AddEntity(new Arc(new Vector2(-A / 2, -E / 2), R, 180, 270));
            dxf.AddEntity(new Line(new Vector2(-A / 2, -E / 2 - R), new Vector2(A / 2, -E / 2 - R)));
            dxf.AddEntity(new Arc(new Vector2(A / 2, -E / 2), R, 270, 0));
            dxf.AddEntity(new Line(new Vector2(A / 2, -E / 2), new Vector2(A / 2 + R, -E / 2)));

            dxf.Save(fileName);
        }

        void AddRectangleRound(ref DxfDocument dxf, Vector2 a, Vector2 b, double r) {
            dxf.AddEntity(new Line(new Vector2(a.X + r, a.Y), new Vector2(b.X - r, a.Y)));
            dxf.AddEntity(new Line(new Vector2(b.X, b.Y - r), new Vector2(b.X, a.Y + r)));
            dxf.AddEntity(new Line(new Vector2(b.X - r, b.Y), new Vector2(a.X + r, b.Y)));
            dxf.AddEntity(new Line(new Vector2(a.X, a.Y + r), new Vector2(a.X, b.Y - r)));

            dxf.AddEntity(new Arc(new Vector2(a.X + r, a.Y + r), r, 180, 270));
            dxf.AddEntity(new Arc(new Vector2(a.X + r, b.Y - r), r, 90, 180));
            dxf.AddEntity(new Arc(new Vector2(b.X - r, a.Y + r), r, 270, 0));
            dxf.AddEntity(new Arc(new Vector2(b.X - r, b.Y - r), r, 0, 90));
        }
        void AddRectangle(ref DxfDocument dxf, Vector2 a, Vector2 b) {
            dxf.AddEntity(new Line(a, new Vector2(b.X, a.Y)));
            dxf.AddEntity(new Line(b, new Vector2(b.X, a.Y)));
            dxf.AddEntity(new Line(b, new Vector2(a.X, b.Y)));
            dxf.AddEntity(new Line(a, new Vector2(a.X, b.Y)));
        }

        //Esc-re kilép
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Escape) this.Close();
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.BackColor = Color.FromArgb(157, 163, 170);

            try {
                numericUpDown_A.Value = Properties.Settings.Default.A;
                numericUpDown_B.Value = Properties.Settings.Default.B;
                numericUpDown_D.Value = Properties.Settings.Default.C;
                numericUpDown_C.Value = Properties.Settings.Default.D;
                numericUpDown_E.Value = Properties.Settings.Default.E;
                numericUpDown_R.Value = Properties.Settings.Default.R;
                numericUpDown_T.Value = Properties.Settings.Default.Thickness;
            } catch {
                numericUpDown_A.Value = 35;
                numericUpDown_B.Value = 38;
                numericUpDown_D.Value = 92;
                numericUpDown_C.Value = 92;
                numericUpDown_E.Value = 71;
                numericUpDown_R.Value = 10;
                numericUpDown_T.Value = 2;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            Properties.Settings.Default.A = numericUpDown_A.Value;
            Properties.Settings.Default.B = numericUpDown_B.Value;
            Properties.Settings.Default.C = numericUpDown_D.Value;
            Properties.Settings.Default.D = numericUpDown_C.Value;
            Properties.Settings.Default.E = numericUpDown_E.Value;
            Properties.Settings.Default.R = numericUpDown_R.Value;
            Properties.Settings.Default.Thickness = numericUpDown_T.Value;
            Properties.Settings.Default.Save();
        }

        #region Help
        private void PictureBox2_Click(object sender, EventArgs e) {
            Help help = new Help(Version);
            help.FormClosed += Help_FormClosed;
            help.FormClosing += Help_FormClosing;
            help.Disposed += Help_Disposed;

            this.Enabled = false;
            help.ShowDialog();
        }
        private void Help_Disposed(object sender, EventArgs e) {
            this.Enabled = true;
        }
        private void Help_FormClosing(object sender, FormClosingEventArgs e) {
            this.Enabled = true;
        }
        private void Help_FormClosed(object sender, FormClosedEventArgs e) {
            this.Enabled = true;
        }
        #endregion
    }
}