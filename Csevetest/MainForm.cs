using netDxf;
using netDxf.Entities;
using netDxf.Units;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Deployment.Application;

namespace Csevetest {
    public partial class MainForm : Form {
        string Version = "unknown";

        public decimal A { get { return numericUpDown_A.Value; } }
        public decimal B { get { return numericUpDown_B.Value; } }
        public decimal C { get { return numericUpDown_C.Value; } }
        public decimal D { get { return numericUpDown_D.Value; } }
        public decimal E { get { return numericUpDown_E.Value; } }
        public decimal T { get { return numericUpDown_T.Value; } }

        public MainForm(string[] args) {
            InitializeComponent();

            if (ApplicationDeployment.IsNetworkDeployed) {
                Version = string.Format("{0}", ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4));
                this.Text = string.Format("Csévetest v" + Version);
            }
        }

        string DefaultFolderName {
            get {
                string s = "";
                s += "A"; s += A.ToString(); s += "_";
                s += "B"; s += B.ToString(); s += "_";
                s += "C"; s += C.ToString(); s += "_";
                s += "D"; s += D.ToString(); s += "_";
                s += "E"; s += E.ToString(); s += "_";
                s += "T"; s += T.ToString();
                return s;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath == "") {
                MessageBox.Show("DXF generálás megszakítva!");
                return;
            }
            if (!Directory.Exists(fbd.SelectedPath)) {
                MessageBox.Show("DXF generálás megszakítva!");
                return;
            }

            GeneratePartNo1(fbd.SelectedPath + "\\1.dxf");
            GeneratePartNo2(fbd.SelectedPath + "\\2.dxf");
            GeneratePartNo3(fbd.SelectedPath + "\\3.dxf");

            if (MessageBox.Show("DXF generálás sikeres!\r\nMappa megnyitása?", "OK!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Process.Start(fbd.SelectedPath);
        }

        void GeneratePartNo1(string fileName) {
            DxfDocument dxf = new DxfDocument();

            AddRectangle(ref dxf, new Vector2(0d, 0), new Vector2(E, B + 2 * T));

            dxf.Save(fileName);
        }
        void GeneratePartNo2(string fileName) {
            DxfDocument dxf = new DxfDocument();
            decimal y1 = (B + 2 * T) / 2;
            decimal x1 = (A + 2 * T) / 2;
            AddRectangle(ref dxf, new Vector2(-x1, -y1), new Vector2(x1, y1));

            decimal y2 = D / 2;
            decimal x2 = C / 2;
            AddRectangleRound(ref dxf, new Vector2(-x2, -y2), new Vector2(x2, y2), 10);

            dxf.Save(fileName);
        }
        void GeneratePartNo3(string fileName) {
            DxfDocument dxf = new DxfDocument();

            dxf.AddEntity(new Line(new Vector2(A / 2, -E / 2), new Vector2(A / 2, E / 2)));
            dxf.AddEntity(new Line(new Vector2(A / 2, E / 2), new Vector2(A / 2 + 10, E / 2)));
            dxf.AddEntity(new Arc(new Vector2(A / 2, E / 2), 10, 0, 90));
            dxf.AddEntity(new Line(new Vector2(A / 2, E / 2 + 10), new Vector2(-A / 2, E / 2 + 10)));
            dxf.AddEntity(new Arc(new Vector2(-A / 2, E / 2), 10, 90, 180));
            dxf.AddEntity(new Line(new Vector2(-A / 2 - 10, E / 2), new Vector2(-A / 2, E / 2)));
            dxf.AddEntity(new Line(new Vector2(-A / 2, E / 2), new Vector2(-A / 2, -E / 2)));
            dxf.AddEntity(new Line(new Vector2(-A / 2, -E / 2), new Vector2(-A / 2 - 10, -E / 2)));
            dxf.AddEntity(new Arc(new Vector2(-A / 2, -E / 2), 10, 180, 270));
            dxf.AddEntity(new Line(new Vector2(-A / 2, -E / 2 - 10), new Vector2(A / 2, -E / 2 - 10)));
            dxf.AddEntity(new Arc(new Vector2(A / 2, -E / 2), 10, 270, 0));
            dxf.AddEntity(new Line(new Vector2(A / 2, -E / 2), new Vector2(A / 2 + 10, -E / 2)));

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
            try {
                numericUpDown_A.Value = Properties.Settings.Default.A;
                numericUpDown_B.Value = Properties.Settings.Default.B;
                numericUpDown_D.Value = Properties.Settings.Default.C;
                numericUpDown_C.Value = Properties.Settings.Default.D;
                numericUpDown_E.Value = Properties.Settings.Default.E;
                numericUpDown_T.Value = Properties.Settings.Default.Thickness;
            }
            catch {
                numericUpDown_A.Value = 35;
                numericUpDown_B.Value = 38;
                numericUpDown_D.Value = 92;
                numericUpDown_C.Value = 92;
                numericUpDown_E.Value = 71;
                numericUpDown_T.Value = 2;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            Properties.Settings.Default.A = numericUpDown_A.Value;
            Properties.Settings.Default.B = numericUpDown_B.Value;
            Properties.Settings.Default.C = numericUpDown_D.Value;
            Properties.Settings.Default.D = numericUpDown_C.Value;
            Properties.Settings.Default.E = numericUpDown_E.Value;
            Properties.Settings.Default.Thickness = numericUpDown_T.Value;
            Properties.Settings.Default.Save();
        }

        #region Help
        private void pictureBox2_Click(object sender, EventArgs e) {
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