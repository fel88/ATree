using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ATree
{
    public partial class ChecklistViewer : Form
    {
        public ChecklistViewer()
        {
            InitializeComponent();
            FormClosing += ChecklistViewer_FormClosing;
            treeListView1.CellEditStarting += TreeListView1_CellEditStarting;
            treeListView1.MouseDoubleClick += TreeListView1_MouseDoubleClick;

            (treeListView1.Columns[1] as BrightIdeasSoftware.OLVColumn).AspectToStringConverter = (x) =>
        {
            if (x == null) return "(not setted)";
            var dt = (DateTime)(x);
            return dt.ToLongDateString();
        };
            treeListView1.CanExpandGetter = (x) =>
            {
                if (x is CheckListItem c)
                {
                    return c.Childs.Count > 0;
                }
                return false;
            };
            treeListView1.ChildrenGetter = (x) =>
            {
                if (x is CheckListItem c)
                {
                    return c.Childs.ToArray();
                }
                return null;
            };
            LoadChecklist();
        }

        private void TreeListView1_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {

            e.Control.Bounds = e.CellBounds;
        }

        private void TreeListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = treeListView1.GetItemAt(e.X, e.Y);
            var iii = (item as BrightIdeasSoftware.OLVListItem);


            var sub = iii.GetSubItemAt(e.X, e.Y);
            var clmn = iii.SubItems.IndexOf(sub);
            if (clmn == 1 && (iii.RowObject as CheckListItem).PlannedFinishDate == null)
            {
                (iii.RowObject as CheckListItem).PlannedFinishDate = new DateTime();
            }
        }

        private void ChecklistViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveChecklist();
        }

        public static Checklist Current = new Checklist();
        public void Init()
        {
            treeListView1.SetObjects(Current.Items);
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeListView1.SelectedObject == null)
            {
                Current.Items.Add(new CheckListItem() { Name = "new1" });
            }
            else
            {
                var ci = (treeListView1.SelectedObject as CheckListItem);
                ci.AddChild(new CheckListItem() { Name = "new1" });
            }
            UpdateTreeList();
        }

        void UpdateTreeList()
        {
            treeListView1.SetObjects(Current.Items);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeListView1.SelectedObject == null) return;
            var ci = (treeListView1.SelectedObject as CheckListItem);
            ci.Detach();
            Current.Items.Remove(ci);
            UpdateTreeList();
        }

        public void LoadChecklist()
        {
            if (!File.Exists("checklist.xml")) return;
            var doc = XDocument.Load("checklist.xml");
            Current = new Checklist();
            var root = doc.Element("root");
            foreach (var item in root.Elements("item"))
            {
                var cc = new CheckListItem();
                Current.Items.Add(cc);
                cc.ParseFromXml(item);

            }
        }

        void appendNode(StringBuilder sb, CheckListItem item)
        {
            sb.AppendLine($"<item status=\"{item.Status}\" plannedFinish=\"{item.PlannedFinishDate}\">");
            sb.AppendLine($"<name><![CDATA[{item.Name}]]>");
            sb.AppendLine("</name>");
            sb.AppendLine("<childs>");
            foreach (var citem in item.Childs)
            {
                appendNode(sb, citem);
            }
            sb.AppendLine("</childs>");
            sb.AppendLine("</item>");
        }
        public void SaveChecklist()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<root>");
            foreach (var item in Current.Items)
            {
                appendNode(sb, item);
            }
            sb.AppendLine("</root>");
            File.WriteAllText("checklist.xml", sb.ToString());
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Calendar cc = new Calendar();
            cc.Init(Current);
            cc.ShowDialog();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            treeListView1.Font = new System.Drawing.Font(treeListView1.Font.FontFamily, treeListView1.Font.SizeInPoints + 1);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            treeListView1.Font = new System.Drawing.Font(treeListView1.Font.FontFamily, treeListView1.Font.SizeInPoints - 1);
        }
    }
}
