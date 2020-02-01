using System.Drawing;
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
    }
}

