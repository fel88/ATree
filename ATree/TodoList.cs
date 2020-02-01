using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ATree
{
    public class TodoList : AItem
    {
        public List<TodoItem> Items = new List<TodoItem>();

        public override bool IsHovered(PointF pos)
        {
            var rect = new RectangleF(Position.X, Position.Y - Height, Width, Height);
            var r = rect.Contains(pos);
            if (r)
            {

            }
            return r;
        }

        public override void Event(UiEvent ev)
        {
            var rect = new RectangleF(Position.X, Position.Y - Height - 25, Width, Height);

            if (ev is UiMouseEvent mev)
            {
                var r = rect.Contains(mev.Position);
                if (!r) return;
                if (Items.Any())
                {
                    if (mev.Position.X > (rect.Left + rect.Width - 60))
                    {
                        Items.RemoveAt(hoveredItemIndex);
                        ev.Handled = true;
                    }


                    else if (mev.Position.X < (rect.Left + 30))
                    {
                        Items[hoveredItemIndex].Done = !Items[hoveredItemIndex].Done;
                        ev.Handled = true;
                    }
                    

                }
            }
        }

        public override void ToXml(StringBuilder sb)
        {
            sb.AppendLine($"<item type=\"todo\" id=\"{Id}\" name=\"{Name}\" pos=\"{Position.X};{Position.Y}\" >");
            sb.AppendLine("<items>");
            foreach (var citem in Items)
            {
                sb.AppendLine($"<subitem text=\"{citem.Text}\" done=\"{citem.Done}\"/>");
            }
            sb.AppendLine("</items>");
            sb.AppendLine("</item>");
        }

        int hoveredItemIndex = 0;


        public override void Draw(DrawingContext ctx)
        {
            Progress = 0;
            if (Items.Any())
            {
                Progress = (int)Math.Round((Items.Count(z => z.Done) / (float)Items.Count) * 100f);
            }
            var pos = ctx.Transform(Position);
            foreach (var aitem in Childs)
            {
                var pos2 = ctx.Transform(aitem.Position);
                ctx.gr.DrawLine(Pens.Black, pos.X, pos.Y, pos2.X, pos2.Y);
                var dx = aitem.Position.X - Position.X;
                var dy = aitem.Position.Y - Position.Y;
                var len = Position.DistTo(aitem.Position);
                dx /= len;
                dy /= len;

                float arad = 5;
                var px = Position.X + dx * (Radius + arad);
                var py = Position.Y + dy * (Radius + arad);
                var tr = ctx.Transform(new PointF(px, py));
                px = tr.X;
                py = tr.Y;
                arad *= ctx.zoom;
                ctx.gr.FillEllipse(Brushes.Yellow, px - arad, py - arad, arad * 2, arad * 2);
                ctx.gr.DrawEllipse(Pens.Black, px - arad, py - arad, arad * 2, arad * 2);
            }

            if (Items.Any())
            {
                Height = Items.Count * 25 + 25;
                Width = Items.Max(z => ctx.gr.MeasureString(z.Text, Font).Width) + 60;
            }
            Width = Math.Max(130, Width);
            var zoom = ctx.zoom;
            var rad = Radius * zoom;
            var w1 = Width * zoom;
            var h1 = Height * zoom;
            var br = Brush;
            //if (this == ctx.hovered)
            {
                br = Brushes.LightBlue;

            }
            if (this == ctx.selected)
            {
                int gap = 5;
                ctx.gr.DrawRectangle(Pens.Red, pos.X - gap, pos.Y - gap, w1 + gap * 2, h1 + gap * 2);
            }

            ctx.gr.FillRectangle(br, new Rectangle((int)(pos.X), (int)(pos.Y), (int)(w1), (int)(h1)));
            ctx.gr.FillRectangle(Brushes.LightGreen, new Rectangle((int)(pos.X), (int)(pos.Y), (int)(w1), (int)(25 * zoom)));
            ctx.gr.DrawRectangle(Pens.Black, new Rectangle((int)(pos.X), (int)(pos.Y), (int)(w1), (int)(h1)));


            var font = Font;
            var ms = ctx.gr.MeasureString(Name, font);
            ctx.gr.DrawString(Name, font, TextBrush, pos.X, pos.Y);
            var pstr = (int)Progress + "%";
            var ms2 = ctx.gr.MeasureString(pstr, font);
            ctx.gr.DrawString(pstr, font, TextBrush, pos.X + 100 * zoom, pos.Y);
            int yy = 0;
            var gpos = ctx.Box.PointToClient(Cursor.Position);

            //add button
            ctx.gr.FillRectangle(Brushes.White, (int)(pos.X + w1 - 60), (int)(pos.Y + 5), (int)(50), (int)(15));
            ctx.gr.DrawRectangle(new Pen(Color.Black, 1), (int)(pos.X + w1 - 60), (int)(pos.Y + 5), (int)(50), (int)(15));
            ctx.gr.DrawString("add", font, Brushes.Black, (int)(pos.X + w1 - 60), (int)(pos.Y + 5));

            int index = 0;
            foreach (var item in Items)
            {
                int gap2 = 5;
                var yy2 = pos.Y + 25 * zoom + yy;
                var r1 = new Rectangle((int)pos.X, (int)(pos.Y + 25 * zoom + yy), (int)w1, (int)(25 * zoom));

                var tbb = TextBrush;
                if (r1.Contains(new Point((int)gpos.X, (int)gpos.Y)))
                {
                    ctx.gr.FillRectangle(Brushes.Blue, r1);
                    hoveredItemIndex = index;
                    tbb = new SolidBrush(Color.White);
                }
                index++;
                ctx.gr.FillRectangle(Brushes.White, new Rectangle((int)(pos.X + gap2), (int)(yy2 + gap2), 15, 15));
                ctx.gr.DrawRectangle(Pens.Black, new Rectangle((int)(pos.X + gap2), (int)(yy2 + gap2), 15, 15));

                if (item.Done)
                {
                    ctx.gr.DrawLine(new Pen(Color.Red, 3), (int)(pos.X + gap2), (int)(yy2 + gap2), (int)(pos.X + gap2 + 15), (int)(yy2 + gap2 + 15));
                    ctx.gr.DrawLine(new Pen(Color.Red, 3), (int)(pos.X + gap2), (int)(yy2 + gap2 + 15), (int)(pos.X + gap2 + 15), (int)(yy2 + gap2));

                }

                ctx.gr.FillRectangle(Brushes.White, (int)(pos.X + w1 - 60), (int)(yy2 + gap2), (int)(50), (int)(15));
                ctx.gr.DrawRectangle(new Pen(Color.Black, 1), (int)(pos.X + w1 - 60), (int)(yy2 + gap2), (int)(50), (int)(15));
                ctx.gr.DrawString("delete", font, Brushes.Black, (int)(pos.X + w1 - 60), (int)(yy2 + gap2));

                ctx.gr.DrawRectangle(Pens.Black, r1);
                ctx.gr.DrawString(item.Text, font, tbb, pos.X + 25, (int)(pos.Y + 25 * zoom + yy));
                yy += (int)(25 * zoom);
            }
        }

        public static AItem ParseXml(XElement item)
        {
            TodoList ret = new TodoList();
            int id = int.Parse(item.Attribute("id").Value);
            var nm = item.Attribute("name").Value;
            ret.Name = nm;

            var pos = (item.Attribute("pos").Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(z => Form1.ParseFloat(z))).ToArray();
            ret.Position = new PointF(pos[0], pos[1]);
            foreach (var sitem in item.Descendants("subitem"))
            {
                var t = sitem.Attribute("text").Value;
                var dn = bool.Parse(sitem.Attribute("done").Value);
                ret.Items.Add(new TodoItem() { Done = dn, Text = t });
            }
            return ret;
        }
    }
}

