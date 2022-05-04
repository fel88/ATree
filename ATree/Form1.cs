using Dagre;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ATree
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Config.Load();
            if (Config.QuickLoadOnStartup)
            {
                LoadTree("tree.xml");
            }
            Load += Form1_Load;

            MouseWheel += Form1_MouseWheel;
            pictureBox1.MouseMove += PictureBox1_MouseMove;

            pictureBox1.MouseUp += PictureBox1_MouseUp;
            pictureBox1.MouseDown += PictureBox1_MouseDown;

            //SampleTree();
            tb.Width = 100;
            tb.Height = 20;
            tb.KeyUp += Tb_KeyUp;

            ctx.Init(pictureBox1);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            mf = new MessageFilter();
            Application.AddMessageFilter(mf);
        }

        MessageFilter mf = null;
        private void Tb_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(tb.Text))
                {
                    MessageBox.Show("Empty string.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tb.Parent.Controls.Remove(tb);
                if (ctx.selected is TodoList td)
                {
                    td.Items.Add(new TodoItem() { Text = tb.Text });
                }
            }
        }

        TextBox tb = new TextBox();

        public void SampleTree()
        {
            AItem Root = new AItem() { Name = "Happy life" };
            Root.AddChild(new AItem() { Name = "child1", Position = new PointF(200, 0), Progress = 45, DrawProgress = true });

            AllItems.Add(Root);
            AllItems.AddRange(Root.Childs);
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //pictureBox1.Focus();
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            var temp = captured;
            isDrag = false;
            isDrag2 = false;
            captured = null;
            if (e.Button == MouseButtons.Left)
            {
                var hover = ctx.hovered;
                if (hover != null && temp != null && temp != hover && !hover.Parents.Contains(temp) && !temp.Parents.Contains(hover))
                {
                    if (MessageBox.Show($"Connect {temp.Name} to {hover.Name}?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        temp.Detach();
                        hover.AddChild(temp);
                    }
                }
            }
        }

        float startx, starty;
        float origsx, origsy;
        bool isDrag = false;
        bool isDrag2 = false;
        public void UpdateDrawParams()
        {
            if (isDrag)
            {
                var p = pictureBox1.PointToClient(Cursor.Position);

                ctx.sx = origsx + ((p.X - startx) / ctx.zoom);
                ctx.sy = origsy + (-(p.Y - starty) / ctx.zoom);
            }
        }

        PointF startDragPos;
        PointF startCapturedPos;
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

            var pos = pictureBox1.PointToClient(Cursor.Position);
            var p = ctx.Transform(pos);

            if (e.Button == MouseButtons.Middle)
            {
                if (selected != ctx.hovered && selected != null && ctx.hovered != null)
                {
                    if (selected.Parents.Contains(ctx.hovered))
                    {
                        selected.Parents.Remove(ctx.hovered);
                        ctx.hovered.Childs.Remove(selected);
                    }
                    else
                    if (selected.Childs.Contains(ctx.hovered))
                    {
                        selected.Childs.Remove(ctx.hovered);
                        ctx.hovered.Parents.Remove(selected);
                    }
                    else
                    {
                        ctx.hovered.AddChild(selected);
                    }
                }


            }
            if (e.Button == MouseButtons.Left)
            {
                var ev = new UiMouseEvent() { Position = ctx.GetPos() };
                foreach (var item in AllItems)
                {
                    item.Event(ev);
                    if (ev.Handled) break;
                }
                if (!ev.Handled)
                {
                    captured = ctx.hovered;
                    selected = ctx.hovered;
                    startDragPos = ctx.GetPos();
                    if (captured != null)
                        startCapturedPos = captured.Position;
                    propertyGrid1.SelectedObject = captured;
                    isDrag2 = true;
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                isDrag = true;
                startx = pos.X;
                starty = pos.Y;
                origsx = ctx.sx;
                origsy = ctx.sy;
            }
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            float zold = ctx.zoom;
            if (e.Delta > 0) { ctx.zoom *= 1.5f; ; }
            else { ctx.zoom *= 0.5f; }
            if (ctx.zoom < 0.08) { ctx.zoom = 0.08f; }
            if (ctx.zoom > 10) { ctx.zoom = 10f; }

            var pos = pictureBox1.PointToClient(Cursor.Position);

            ctx.sx = -(pos.X / zold - ctx.sx - pos.X / ctx.zoom);
            ctx.sy = (pos.Y / zold + ctx.sy - pos.Y / ctx.zoom);
        }

        public List<AItem> AllItems = new List<AItem>();
        AItem captured = null;

        PointF lastCenter;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            ctx.gr.SmoothingMode = SmoothingMode.AntiAlias;

            var pos = ctx.GetPos();

            if (captured != null && isDrag2)
            {
                var sx = -startDragPos.X + pos.X;
                var sy = -startDragPos.Y + pos.Y;
                captured.Position = new PointF(sx + startCapturedPos.X, sy + startCapturedPos.Y);
            }

            #region hovered check
            ctx.hovered = null;
            foreach (var item in AllItems)
            {
                if (item == captured) continue;
                if (item.IsHovered(pos))
                {
                    ctx.hovered = item;
                    break;
                }
            }

            #endregion
            UpdateDrawParams();
            ctx.gr.Clear(Color.White);
            ctx.gr.DrawString("x: " + Math.Round(pos.X, 2) + "   y: " + Math.Round(pos.Y, 2), SystemFonts.DefaultFont, Brushes.Red, 5, 5);
            lastCenter = ctx.Transform(new PointF(pictureBox1.Width / 2, pictureBox1.Height / 2));
            ctx.gr.SmoothingMode = SmoothingMode.AntiAlias;
            foreach (var item in AllItems.Where(z => z.Parents.Count == 0))
            {
                item.Draw(ctx);
            }

            pictureBox1.Image = ctx.bmp;
        }

        public DrawingContext ctx = new DrawingContext();


        public AItem selected
        {
            get
            {
                return ctx.selected;
            }
            set
            {
                ctx.selected = value;
            }
        }

        public void DeleteSelected()
        {
            if (selected == null) return;
            if (MessageBox.Show("Delete node: " + selected.Name + "?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            AllItems.Remove(selected);
            foreach (var item in selected.Parents)
            {
                item.Childs.Remove(selected);
            }
            foreach (var item in selected.Childs)
            {
                item.Parents.Remove(selected);
            }
        }
        private void DeleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelected();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (pictureBox1.Focused)
            {
                if (keyData == Keys.Delete)
                {
                    DeleteSelected();
                }
                if (keyData == Keys.Insert)
                {
                    AddChildToSelected();
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void AddChildToSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddChildToSelected();
        }

        public void AddChildToSelected()
        {
            if (selected == null) return;
            var a = new AItem() { Name = "new node1", Position = new PointF(selected.Position.X + selected.Radius * 2, selected.Position.Y) };
            selected.AddChild(a);
            AllItems.Add(a);
        }

        void SaveTree(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");

            sb.AppendLine($"<root sx=\"{ctx.sx.ToString().Replace(",", ".")}\" sy=\"{ctx.sy.ToString().Replace(",", ".")}\" zoom=\"{ctx.zoom.ToString().Replace(",", ".")}\">");
            foreach (var item in AllItems)
            {
                item.ToXml(sb);
            }
            sb.AppendLine("</root>");


            File.WriteAllText(path, sb.ToString());
        }

        void LoadTree(string path)
        {
            var doc = XDocument.Load(path);
            AllItems.Clear();
            var root = doc.Descendants("root").First();
            ctx.sx = float.Parse(root.Attribute("sx").Value, CultureInfo.InvariantCulture);
            ctx.sy = float.Parse(root.Attribute("sy").Value, CultureInfo.InvariantCulture);
            ctx.zoom = float.Parse(root.Attribute("zoom").Value, CultureInfo.InvariantCulture);

            foreach (var item in doc.Descendants("item"))
            {
                if (item.Attribute("type") != null)
                {
                    var tp = item.Attribute("type").Value;
                    if (tp == "todo")
                    {
                        AllItems.Add(TodoList.ParseXml(item));
                        continue;
                    }
                }
                int id = int.Parse(item.Attribute("id").Value);
                var nm = item.Attribute("name").Value;
                var progress = int.Parse(item.Attribute("progress").Value);


                var pos = (item.Attribute("pos").Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(z => ParseFloat(z))).ToArray();
                var rad = ParseFloat(item.Attribute("radius").Value);
                var r = new AItem() { Id = id, Name = nm, Progress = progress, DrawProgress = true, Position = new PointF(pos[0], pos[1]), Radius = rad };
                AllItems.Add(r);
                if (item.Attribute("autoProgress") != null)
                {
                    r.AutoProgress = bool.Parse(item.Attribute("autoProgress").Value);
                }
                if (item.Attribute("drawProgress") != null)
                {
                    r.DrawProgress = bool.Parse(item.Attribute("drawProgress").Value);
                }

                if (item.Attribute("shape") != null)
                {
                    r.Shape = (ShapeType)Enum.Parse(typeof(ShapeType), item.Attribute("shape").Value);
                    if (r.Shape == ShapeType.Rectangle)
                    {
                        r.Width = ParseFloat(item.Attribute("width").Value);
                        r.Height = ParseFloat(item.Attribute("height").Value);
                    }
                }
            }

            AItem.NewId = AllItems.Max(z => z.Id) + 1;
            foreach (var item in doc.Descendants("item"))
            {
                int id = int.Parse(item.Attribute("id").Value);
                if (!item.Descendants("childs").Any()) continue;
                var val = item.Descendants("childs").First().Value;
                var aa = val.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                var nd = AllItems.First(z => z.Id == id);
                foreach (var zitem in aa)
                {
                    var nd2 = AllItems.First(z => z.Id == zitem);
                    nd.AddChild(nd2);
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.xml|*.xml";
            if (sfd.ShowDialog() != DialogResult.OK) return;

            SaveTree(sfd.FileName);
        }


        public static float ParseFloat(string str)
        {
            return float.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);
        }
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "*.xml|*.xml";
            if (sfd.ShowDialog() != DialogResult.OK) return;

            LoadTree(sfd.FileName);
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllItems.Clear();
            var a = new AItem() { Name = "new root1" };
            AllItems.Add(a);
            fitAll();
        }

        private void AddRootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var a = new AItem() { Name = "new root1" };
            AllItems.Add(a);
        }

        private void QuickLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTree("tree.xml");
        }

        private void QuickSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTree("tree.xml");
        }

        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            SettingsForm s = new SettingsForm();
            s.StartPosition = FormStartPosition.CenterParent;
            s.ShowDialog();
        }

        private void addTodoListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var t = ctx.BackTransform(new PointF(Width / 2, Height / 2));
            var a = new TodoList() { Name = "new todo1" };
            a.Position = t;
            AllItems.Add(a);
        }

        private void addItemToTodoListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ctx.selected is TodoList td)
            {
                pictureBox1.Controls.Add(tb);
                tb.Text = "";
                tb.BringToFront();
            }
        }

        private void storeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = 1;
            var ff = Directory.GetFiles(".", "tree_backup*.xml").ToArray();
            if (ff.Any())
            {
                index = ff.Select(x => int.Parse(Path.GetFileNameWithoutExtension(x).Replace("tree_backup", ""))).Max() + 1;
            }
            var pp = "tree_backup" + index + ".xml";
            File.Copy("tree.xml", pp);
            MessageBox.Show("Stored to " + pp, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Config.QuickSaveOnClosing)
            {
                SaveTree("tree.xml");
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            DagreInputGraph dg = new DagreInputGraph();
            foreach (var item in AllItems)
            {
                dg.AddNode(item, item.Rect.Width, item.Rect.Height);
            }
            foreach (var item in AllItems)
            {
                foreach (var ch in item.Childs)
                {
                    var nd1 = dg.GetNode(item);
                    var nd2 = dg.GetNode(ch);
                    dg.AddEdge(nd1, nd2);
                }
            }


            dg.Layout();

            foreach (var item in dg.Nodes())
            {
                var tag = (item.Tag as AItem);
                tag.X = item.X;
                tag.Y = -item.Y;
            }

            fitAll();
        }

        private void detachSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selected == null) return;
            if (MessageBox.Show("Detach node: " + selected.Name + "?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            selected.Detach();
        }

        void fitAll()
        {
            List<PointF> pp = new List<PointF>();
            foreach (var item in AllItems)
            {
                pp.Add(item.Rect.Location);
                pp.Add(new PointF(item.Rect.Right, item.Rect.Bottom));
            }

            ctx.FitToPoints(pp.ToArray(), 5);
        }


        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            fitAll();
        }
    }

    public class UiEvent
    {
        public bool Handled;
    }

    public class UiMouseEvent : UiEvent
    {
        public PointF Position;
    }
}

