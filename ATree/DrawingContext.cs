using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ATree
{
    public class DrawingContext
    {
        public void Init(Control c)
        {
            Box = c;
            bmp = new Bitmap(c.Width, c.Height);
            gr = Graphics.FromImage(bmp);

            c.SizeChanged += C_SizeChanged;
        }

        private void C_SizeChanged(object sender, System.EventArgs e)
        {
            bmp = new Bitmap(Box.Width, Box.Height);
            gr = Graphics.FromImage(bmp);
        }

        public AItem hovered = null;
        public AItem selected = null;
        public Bitmap bmp;
        public Graphics gr;
        public float sx;
        public float sy;
        public float zoom = 1;
        public virtual PointF Transform(PointF p1)
        {
            return new PointF((p1.X + sx) * zoom, -1 * (p1.Y + sy) * zoom);
        }
        public Control Box;
        public PointF GetPos()
        {
            var pos = Box.PointToClient(Cursor.Position);
            var posx = (pos.X / zoom - sx);
            var posy = (-pos.Y / zoom - sy);

            return new PointF(posx, posy);
        }
        public virtual PointF BackTransform(PointF p1)
        {
            return new PointF((p1.X / zoom - sx), -(p1.Y / zoom + sy));
        }

        internal void FitToPoints(PointF[] points, int gap = 0)
        {
            var maxx = points.Max(z => z.X) + gap;
            var minx = points.Min(z => z.X) - gap;
            var maxy = points.Max(z => z.Y) + gap;
            var miny = points.Min(z => z.Y) - gap;

            var w = bmp.Width;
            var h = bmp.Height;

            var dx = maxx - minx;
            var kx = w / dx;
            var dy = maxy - miny;
            var ky = h / dy;

            var oz = zoom;
            var sz1 = new Size((int)(dx * kx), (int)(dy * kx));
            var sz2 = new Size((int)(dx * ky), (int)(dy * ky));
            zoom = kx;
            if (sz1.Width > w || sz1.Height > h) zoom = ky;
            var tr0 = Transform(new PointF(0, 0));
            var x = dx / 2 + minx;
            var y = dy / 2 + miny;

            var shift = new PointF(((w / 2f) / zoom - x), ((h / 2f) / zoom + y));
            sx = shift.X;
            sy = -shift.Y;
            var tr1 = Transform(new PointF(0, 0));
        }
    }
}

