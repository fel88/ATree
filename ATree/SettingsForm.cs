using System;
using System.Windows.Forms;

namespace ATree
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            checkBox1.Checked = Config.QuickLoadOnStartup;
            checkBox2.Checked = Config.QuickSaveOnClosing;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Config.QuickLoadOnStartup = checkBox1.Checked;
            Config.QuickSaveOnClosing = checkBox2.Checked;
            Config.Save();
            Close();
        }
    }
}
