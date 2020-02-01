using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        public float Radius
        {
            get
            {
                return Width;
            }
            set
            {
                Width = value;
                Height = value;
            }
        }

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
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

            var zoom = ctx.zoom;
            var rad = Radius * zoom;
            var w1 = Width * zoom;
            var h1 = Height * zoom;
            var br = Brush;
            if (this == ctx.hovered)
            {
                br = Brushes.LightBlue;

            }
            if (this == ctx.selected)
            {
                int gap = 5;
                ctx.gr.DrawRectangle(Pens.Red, pos.X - w1 - gap, pos.Y - h1 - gap, w1 * 2 + gap * 2, h1 * 2 + gap * 2);
            }

            switch (Shape)
            {
                case ShapeType.Ellipse:
                    ctx.gr.FillEllipse(br, pos.X - w1, pos.Y - h1, w1 * 2, h1 * 2);
                    break;
                case ShapeType.Rectangle:
                    ctx.gr.FillPath(br, RoundedRect(new Rectangle((int)(pos.X - w1), (int)(pos.Y - h1), (int)(w1 * 2), (int)(h1 * 2)), 10));
                    break;
            }


            if (this.IsProgress)
            {
                ctx.gr.FillPie(Brushes.LightGreen, pos.X - rad, pos.Y - rad, rad * 2, rad * 2, 0, (Progress / 100f) * 360f);
                ctx.gr.DrawPie(Pens.Gray, pos.X - rad, pos.Y - rad, rad * 2, rad * 2, 0, (Progress / 100f) * 360f);
            }

            switch (this.Shape)
            {
                case ShapeType.Ellipse:
                    ctx.gr.DrawEllipse(Pens.Black, pos.X - w1, pos.Y - h1, w1 * 2, h1 * 2);
                    break;
                case ShapeType.Rectangle:
                    ctx.gr.DrawPath(Pens.Black, RoundedRect(new Rectangle((int)(pos.X - w1), (int)(pos.Y - h1), (int)(w1 * 2), (int)(h1 * 2)), 10));
                    break;
            }

            var font = Font;
            var ms = ctx.gr.MeasureString(Name, font);
            ctx.gr.DrawString(Name, font, TextBrush, pos.X - ms.Width / 2, pos.Y - ms.Height);
            var pstr = (int)Progress + "%";
            var ms2 = ctx.gr.MeasureString(pstr, font);
            ctx.gr.DrawString(pstr, font, TextBrush, pos.X - ms2.Width / 2, pos.Y - ms2.Height / 2 + 10);

            foreach (var aitem in Childs)
            {
                aitem.Draw(ctx);
            }
        }
        public Brush Brush = Brushes.LightYellow;

        public PointF Position;
        public bool Done { get; set; }
        public bool IsProgress { get; set; }
        public int Progress { get; set; }
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

        public virtual bool IsHovered(PointF pos)
        {
            return (Position.DistTo(pos) < Radius);            
        }

        internal void AddChild(AItem aItem)
        {
            Childs.Add(aItem);
            aItem.Parents.Add(this);
        }

        public virtual void ToXml(StringBuilder sb)
        {
            sb.AppendLine($"<item id=\"{Id}\" name=\"{Name}\" progress=\"{Progress}\" pos=\"{Position.X};{Position.Y}\" radius=\"{Radius}\">");
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
}

