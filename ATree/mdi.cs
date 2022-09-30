using System;
using System.Windows.Forms;

namespace ATree
{
    public partial class mdi : Form
    {
        public mdi()
        {
            InitializeComponent();
        }

        private void diagramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.MdiParent = this;
            f.Show();
        }

        private void checklistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChecklistViewer f = new ChecklistViewer();
            f.Init();
            f.MdiParent = this;
            f.Show();
        }

        private void abourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.ShowDialog();
        }
    }
}
