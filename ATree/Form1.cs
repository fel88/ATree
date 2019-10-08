using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gr = Graphics.FromImage(bmp);
            SizeChanged += Form1_SizeChanged;
            MouseWheel += Form1_MouseWheel;
            pictureBox1.MouseMove += PictureBox1_MouseMove;

            pictureBox1.MouseUp += PictureBox1_MouseUp;
            pictureBox1.MouseDown += PictureBox1_MouseDown;

            //SampleTree();
        }

        public void SampleTree()
        {
            AItem Root = new AItem() { Name = "Happy life" };
            Root.AddChild(new AItem() { Name = "child1", Position = new PointF(200, 0), Progress = 45, IsProgress = true });

            AllItems.Add(Root);
            AllItems.AddRange(Root.Childs);
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox1.Focus();
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDrag = false;
            isDrag2 = false;
            captured = null;
        }

        float startx, starty;
        float origsx, origsy;
        bool isDrag = false;
        bool isDrag2 = false;
        public void Update()
        {
            if (isDrag)
            {
                var p = pictureBox1.PointToClient(Cursor.Position);

                sx = origsx + ((p.X - startx) / zoom);
                sy = origsy + (-(p.Y - starty) / zoom);
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

            var pos = pictureBox1.PointToClient(Cursor.Position);
            var p = Transform(pos);

            if (e.Button == MouseButtons.Middle)
            {
                if (selected != hovered && selected != null && hovered != null)
                {
                    if (selected.Parents.Contains(hovered))
                    {
                        selected.Parents.Remove(hovered);
                        hovered.Childs.Remove(selected);
                    }
                    else
                    if (selected.Childs.Contains(hovered))
                    {
                        selected.Childs.Remove(hovered);
                        hovered.Parents.Remove(selected);
                    }
                    else
                    {
                        hovered.AddChild(selected);
                    }
                }


            }
            if (e.Button == MouseButtons.Left)
            {
                captured = hovered;
                selected = hovered;
                propertyGrid1.SelectedObject = captured;
                isDrag2 = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                isDrag = true;
                startx = pos.X;
                starty = pos.Y;
                origsx = sx;
                origsy = sy;
            }
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            float zold = zoom;
            if (e.Delta > 0) { zoom *= 1.5f; ; }
            else { zoom *= 0.5f; }
            if (zoom < 0.08) { zoom = 0.08f; }
            if (zoom > 10) { zoom = 10f; }

            var pos = pictureBox1.PointToClient(Cursor.Position);

            sx = -(pos.X / zold - sx - pos.X / zoom);
            sy = (pos.Y / zold + sy - pos.Y / zoom);
        }



        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gr = Graphics.FromImage(bmp);
        }

        Bitmap bmp;
        Graphics gr;

        public PointF GetPos()
        {
            var pos = pictureBox1.PointToClient(Cursor.Position);
            var posx = (pos.X / zoom - sx);
            var posy = (-pos.Y / zoom - sy);

            return new PointF(posx, posy);
        }
        public List<AItem> AllItems = new List<AItem>();
        AItem captured = null;
        AItem selected = null;
        private void Timer1_Tick(object sender, EventArgs e)
        {

            var pos = GetPos();
            if (captured != null && isDrag2)
            {
                captured.Position = pos;
            }

            #region hovered check
            hovered = null;
            foreach (var item in AllItems)
            {
                if (item.Position.DistTo(pos) < item.Radius)
                {
                    hovered = item;
                    break;
                }
            }

            #endregion
            Update();
            gr.Clear(Color.White);

            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (var item in AllItems.Where(z => z.Parents.Count == 0))
            {
                DrawRec(item);
            }

            pictureBox1.Image = bmp;
        }

        public virtual PointF Transform(PointF p1)
        {
            return new PointF((p1.X + sx) * zoom, -1 * (p1.Y + sy) * zoom);
        }

        AItem hovered = null;
        public void DrawRec(AItem item)
        {

            var pos = Transform(item.Position);
            foreach (var aitem in item.Childs)
            {
                var pos2 = Transform(aitem.Position);
                gr.DrawLine(Pens.Black, pos.X, pos.Y, pos2.X, pos2.Y);
                var dx = aitem.Position.X - item.Position.X;
                var dy = aitem.Position.Y - item.Position.Y;
                var len = item.Position.DistTo(aitem.Position);
                dx /= len;
                dy /= len;

                float arad = 5;
                var px = item.Position.X + dx * (item.Radius + arad);
                var py = item.Position.Y + dy * (item.Radius + arad);
                var tr = Transform(new PointF(px, py));
                px = tr.X;
                py = tr.Y;
                arad *= zoom;
                gr.FillEllipse(Brushes.Yellow, px - arad, py - arad, arad * 2, arad * 2);
                gr.DrawEllipse(Pens.Black, px - arad, py - arad, arad * 2, arad * 2);
            }

            var rad = item.Radius * zoom;
            var br = item.Brush;
            if (item == hovered)
            {
                br = Brushes.LightBlue;

            }
            if (item == selected)
            {
                gr.DrawRectangle(Pens.Red, pos.X - rad, pos.Y - rad, rad * 2, rad * 2);
            }

            gr.FillEllipse(br, pos.X - rad, pos.Y - rad, rad * 2, rad * 2);

            if (item.IsProgress)
            {
                gr.FillPie(Brushes.LightGreen, pos.X - rad, pos.Y - rad, rad * 2, rad * 2, 0, (item.Progress / 100f) * 360f);
                gr.DrawPie(Pens.Gray, pos.X - rad, pos.Y - rad, rad * 2, rad * 2, 0, (item.Progress / 100f) * 360f);
            }
            gr.DrawEllipse(Pens.Black, pos.X - rad, pos.Y - rad, rad * 2, rad * 2);
            var font = new Font("Arial", 8);
            var ms = gr.MeasureString(item.Name, font);
            gr.DrawString(item.Name, font, Brushes.Black, pos.X - ms.Width / 2, pos.Y - ms.Height);
            var pstr = (int)item.Progress + "%";
            var ms2 = gr.MeasureString(pstr, font);
            gr.DrawString(pstr, font, Brushes.Black, pos.X - ms2.Width / 2, pos.Y - ms2.Height / 2 + 10);

            foreach (var aitem in item.Childs)
            {
                DrawRec(aitem);
            }
        }



        public float zoom = 1;

        private void DeleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selected == null) return;
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

        private void AddChildToSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selected == null) return;
            var a = new AItem() { Name = "new node1" };
            selected.AddChild(a);
            AllItems.Add(a);
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.xml|*.xml";
            if (sfd.ShowDialog() != DialogResult.OK) return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<root>");
            foreach (var item in AllItems)
            {
                sb.AppendLine($"<item id=\"{item.Id}\" name=\"{item.Name}\" progress=\"{item.Progress}\" pos=\"{item.Position.X};{item.Position.Y}\" radius=\"{item.Radius}\">");
                sb.AppendLine("<childs>");
                foreach (var citem in item.Childs)
                {
                    sb.Append(citem.Id + ";");
                }
                sb.AppendLine("</childs>");
                sb.AppendLine("</item>");
            }
            sb.AppendLine("</root>");


            File.WriteAllText(sfd.FileName, sb.ToString());

        }


        public float ParseFloat(string str)
        {
            return float.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);
        }
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "*.xml|*.xml";
            if (sfd.ShowDialog() != DialogResult.OK) return;
            var doc = XDocument.Load(sfd.FileName);
            AllItems.Clear();

            foreach (var item in doc.Descendants("item"))
            {
                int id = int.Parse(item.Attribute("id").Value);
                var nm = item.Attribute("name").Value;
                var progress = int.Parse(item.Attribute("progress").Value);
                var pos = (item.Attribute("pos").Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(z => ParseFloat(z))).ToArray();
                var rad = ParseFloat(item.Attribute("radius").Value);
                AllItems.Add(new AItem() { Id = id, Name = nm, Progress = progress, IsProgress = true, Position = new PointF(pos[0], pos[1]), Radius = rad });
            }
            AItem.NewId = AllItems.Max(z => z.Id) + 1;
            foreach (var item in doc.Descendants("item"))
            {
                int id = int.Parse(item.Attribute("id").Value);
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

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllItems.Clear();
        }

        private void AddRootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var a = new AItem() { Name = "new root1" };
            AllItems.Add(a);
        }

        public float sx;
        public float sy;
    }
}

