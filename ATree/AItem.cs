using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace ATree
{
    public class AItem
    {
        public static int NewId = 0;
        public int Id { get; set; }
        public AItem()
        {
            Id = NewId++;
        }
        public virtual void Event(UiEvent ev)
        {

        }
        public List<AItem> Parents = new List<AItem>();
        public string Name { get; set; }
        public float Width { get; set; } = 40;
        public float Height { get; set; } = 40;
        public float _radius = 40;

        public float Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
                Width = value * 2;
                Height = value * 2;
            }
        }

        public void Detach()
        {
            foreach (var item in Parents)
            {
                item.Childs.Remove(this);
            }
            Parents.Clear();
        }
        public static GraphicsPath RoundedRect(RectangleF bounds, float radius)
        {
            float diameter = radius * 2;
            SizeF size = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }


        public virtual void Draw(DrawingContext ctx)
        {

            var pos = ctx.Transform(Position);
            foreach (var aitem in Childs)
            {
                var pos2 = ctx.Transform(aitem.Position);
                var dx = aitem.Position.X - Position.X;
                var dy = aitem.Position.Y - Position.Y;
                var len = Position.DistTo(aitem.Position);
                dx /= len;
                dy /= len;

                float arad = 0;
                var px = aitem.Position.X - dx * (aitem.Radius + arad);
                var py = aitem.Position.Y - dy * (aitem.Radius + arad);
                var tr = ctx.Transform(new PointF(px, py));
                px = tr.X;
                py = tr.Y;
                arad *= ctx.zoom;
                //ctx.gr.DrawLine(Pens.Black, pos.X, pos.Y, pos2.X, pos2.Y);
                Pen pen = new Pen(Brushes.Black);
                // A triangle



                AdjustableArrowCap arrowCap = new AdjustableArrowCap(2, 1);

                arrowCap.WidthScale = 5;
                arrowCap.BaseCap = LineCap.Round;
                arrowCap.Height = 3;
                pen.CustomEndCap = arrowCap;


                Vector2d outp = new Vector2d();
                if (aitem.Shape == ShapeType.Rectangle)
                {
                    List<LineInfo> lines = new List<LineInfo>();
                    lines.Add(new LineInfo() { Start = new Vector2d(aitem.Rect.X, aitem.Rect.Y), End = new Vector2d(aitem.Rect.Right, aitem.Rect.Y) });
                    lines.Add(new LineInfo() { Start = new Vector2d(aitem.Rect.X, aitem.Rect.Y), End = new Vector2d(aitem.Rect.X, aitem.Rect.Bottom) });
                    lines.Add(new LineInfo() { Start = new Vector2d(aitem.Rect.Right, aitem.Rect.Bottom), End = new Vector2d(aitem.Rect.X, aitem.Rect.Bottom) });
                    lines.Add(new LineInfo() { Start = new Vector2d(aitem.Rect.Right, aitem.Rect.Bottom), End = new Vector2d(aitem.Rect.Right, aitem.Rect.Y) });

                    foreach (var item in lines)
                    {
                        if (Helpers.IntersectSegments(new Vector2d(Position.X, Position.Y), new Vector2d(aitem.Position.X, aitem.Position.Y), item.Start, item.End, ref outp))
                        {
                            break;
                        }
                    }

                    var outpf = ctx.Transform(new PointF((float)outp.X, (float)outp.Y));
                    ctx.gr.DrawLine(pen, pos.X, pos.Y, outpf.X, outpf.Y);

                }
                else
                {
                    ctx.gr.DrawLine(pen, pos.X, pos.Y, px, py);

                }
                /*ctx.gr.ExcludeClip(reg);
                ctx.gr.DrawLine(Pens.Black, pos.X, pos.Y, pos2.X, pos2.Y);
                ctx.gr.ResetClip();*/

                //ctx.gr.FillEllipse(Brushes.Yellow, px - arad, py - arad, arad * 2, arad * 2);
                //ctx.gr.DrawEllipse(Pens.Black, px - arad, py - arad, arad * 2, arad * 2);
            }

            var zoom = ctx.zoom;
            var rad = Radius * zoom;
            var w1 = Width * zoom;
            var h1 = Height * zoom;
            var br = Brush;
            if (Done)
            {
                br = Brushes.LightGreen;
            }
            if (this == ctx.hovered)
            {
                br = Brushes.LightBlue;

            }

            int gap = 5;
            switch (Shape)
            {
                case ShapeType.Ellipse:
                    if (this == ctx.selected)
                    {
                        ctx.gr.DrawRectangle(Pens.Red, pos.X - rad - gap, pos.Y - rad - gap, rad * 2 + gap * 2, rad * 2 + gap * 2);
                    }
                    ctx.gr.FillEllipse(br, pos.X - rad, pos.Y - rad, rad * 2, rad * 2);
                    break;
                case ShapeType.Rectangle:
                    if (this == ctx.selected)
                    {
                        ctx.gr.DrawRectangle(Pens.Red, pos.X - w1 / 2 - gap, pos.Y - h1 / 2 - gap, w1 + gap * 2, h1 + gap * 2);
                    }
                    ctx.gr.FillPath(br, RoundedRect(new Rectangle((int)(pos.X - w1 / 2), (int)(pos.Y - h1 / 2), (int)(w1), (int)(h1)), 10 * ctx.zoom));
                    break;
            }

            if (this.DrawProgress && Shape == ShapeType.Ellipse)
            {
                ctx.gr.FillPie(Brushes.LightGreen, pos.X - rad, pos.Y - rad, rad * 2, rad * 2, 0, (Progress / 100f) * 360f);
                ctx.gr.DrawPie(Pens.Gray, pos.X - rad, pos.Y - rad, rad * 2, rad * 2, 0, (Progress / 100f) * 360f);
            }
            if (this.DrawProgress && Shape == ShapeType.Rectangle)
            {
                var rr = RoundedRect(new RectangleF((pos.X - w1 / 2), (pos.Y - h1 / 2), ((Progress / 100f) * w1), (h1)), 1 * ctx.zoom);
                var rrClip = RoundedRect(new RectangleF((pos.X - w1 / 2), (pos.Y - h1 / 2), (w1), (h1)), 10 * ctx.zoom);
                ctx.gr.SetClip(rrClip);
                ctx.gr.FillPath(Brushes.LightGreen, rr);
                ctx.gr.ResetClip();
                //ctx.gr.DrawRectangle(Pens.Gray, pos.X - rad, pos.Y - rad, rad * 2, rad * 2, 0, (Progress / 100f) * 360f);
            }

            switch (this.Shape)
            {
                case ShapeType.Ellipse:
                    ctx.gr.DrawEllipse(Pens.Black, pos.X - rad, pos.Y - rad, rad * 2, rad * 2);
                    break;
                case ShapeType.Rectangle:
                    ctx.gr.DrawPath(Pens.Black, RoundedRect(new Rectangle((int)(pos.X - w1 / 2), (int)(pos.Y - h1 / 2), (int)(w1), (int)(h1)), 10 * ctx.zoom));
                    break;
            }

            var font = Font;
            var ms = ctx.gr.MeasureString(Name, font);
            ctx.gr.DrawString(Name, font, TextBrush, pos.X - ms.Width / 2, pos.Y - ms.Height);
            var pstr = (int)Progress + "%";
            var ms2 = ctx.gr.MeasureString(pstr, font);

            if (DrawProgress)
                ctx.gr.DrawString(pstr, font, TextBrush, pos.X - ms2.Width / 2, pos.Y - ms2.Height / 2 + 10);

            foreach (var aitem in Childs)
            {
                aitem.Draw(ctx);
            }
        }
        public Brush Brush = Brushes.LightYellow;

        PointF _pos;
        public float X { get => _pos.X; set => _pos.X = value; }
        public float Y { get => _pos.Y; set => _pos.Y = value; }
        public PointF Position { get => _pos; set => _pos = value; }
        public bool Done { get; set; }
        public bool DrawProgress { get; set; } = true;
        int _progress;
        public int Progress
        {
            get
            {
                if (AutoProgress && Childs.Count > 0)
                {
                    _progress = (int)Math.Round((Childs.Sum(z => z.Progress) / (float)Childs.Count));
                }
                return _progress;
            }
            set => _progress = Math.Max(Math.Min(100, value), 0);
        }
        public bool AutoProgress { get; set; }
        public List<AItem> Childs = new List<AItem>();
        public List<AInfoItem> Infos = new List<AInfoItem>();
        public ShapeType Shape { get; set; }

        public Brush TextBrush = Brushes.Black;

        Color _textColor;
        public Font Font { get; set; } = new Font("Arial", 8);
        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                _textColor = value;
                TextBrush = new SolidBrush(value);

            }
        }

        Color _backColor;

        public Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
                Brush = new SolidBrush(value);

            }
        }

        public RectangleF Rect
        {
            get
            {
                if (Shape == ShapeType.Ellipse)
                {
                    return new RectangleF(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);

                }
                return new RectangleF(Position.X - Width / 2, Position.Y - Height / 2, Width, Height);
            }
        }

        public virtual bool IsHovered(PointF pos)
        {
            if (Shape == ShapeType.Rectangle)
            {
                return Rect.IntersectsWith(new RectangleF(pos.X, pos.Y, 1, 1));
            }
            return (Position.DistTo(pos) < Radius);
        }

        internal void AddChild(AItem aItem)
        {
            Childs.Add(aItem);
            aItem.Parents.Add(this);
        }

        public virtual void ToXml(StringBuilder sb)
        {
            sb.AppendLine($"<item id=\"{Id}\" name=\"{Name}\" drawProgress=\"{DrawProgress}\" autoProgress=\"{AutoProgress}\" progress=\"{Progress}\" pos=\"{Position.X};{Position.Y}\" radius=\"{Radius}\" width=\"{Width}\" height=\"{Height}\" shape=\"{Shape}\">");
            sb.AppendLine("<childs>");
            foreach (var citem in Childs)
            {
                sb.Append(citem.Id + ";");
            }
            sb.AppendLine("</childs>");
            sb.AppendLine("</item>");
        }
    }

    public enum ShapeType
    {
        Ellipse, Rectangle
    }

    public class LineInfo
    {
        public Vector2d Start;
        public Vector2d End;
    }
}