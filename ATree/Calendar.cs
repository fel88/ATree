using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ATree
{
    public partial class Calendar : Form
    {
        public Calendar()
        {
            InitializeComponent();
            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox1.Invalidate();
            SizeChanged += Calendar_SizeChanged;
        }

        private void Calendar_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        public void Init(Checklist ch)
        {
            checklist = ch;
        }
        Checklist checklist;
        DateTime current = DateTime.Now;
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var n = current;
            int days = DateTime.DaysInMonth(n.Year, n.Month);
            e.Graphics.Clear(Color.White);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var cw = pictureBox1.Width / 7;
            int ch = pictureBox1.Height / 5;
            int xx = 0;
            int yy = 0;
            var start = n;
            if (n.Day != 1)
                start = start.AddDays(-n.Day + 1);

            var fl = checklist.Flatten().ToArray();
            for (int i = 0; i < days; i++)
            {
                if (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
                {
                    e.Graphics.FillRectangle(Brushes.LightGreen, xx * cw, yy * ch, cw, ch);
                }
                if (start.Date == DateTime.Now.Date)
                {
                    HatchBrush brush =
             new HatchBrush(HatchStyle.BackwardDiagonal, Color.Red, Color.Violet);
                    e.Graphics.FillRectangle(brush, xx * cw, yy * ch, cw, ch);
                }
                e.Graphics.DrawRectangle(Pens.Black, xx * cw, yy * ch, cw, ch);

                e.Graphics.DrawString(start.Day.ToString(), SystemFonts.DefaultFont, Brushes.Black, xx * cw, yy * ch);

                var ww = fl.Where(z => z.PlannedFinishDate != null && start.Date == z.PlannedFinishDate.Value.Date).ToArray();
                start = start.AddDays(1);

                int yyshift = 15;
                foreach (var witem in ww)
                {
                    var rect = new RectangleF(xx * cw, yy * ch + yyshift, cw, ch - yyshift);

                    var ms = e.Graphics.MeasureString(witem.Name, SystemFonts.DefaultFont, new SizeF(cw, ch - yyshift));
                    e.Graphics.FillRectangle(Brushes.LightBlue, xx * cw, yy * ch + yyshift, rect.Width, ms.Height);
                    e.Graphics.DrawString(witem.Name,
                      SystemFonts.DefaultFont,
                      Brushes.Black, rect);
                    e.Graphics.DrawRectangle(Pens.Black, xx * cw, yy * ch + yyshift, rect.Width, ms.Height);
                 
                    if (witem.Status == CheckListStatusTypeEnum.Done)
                    {
                        e.Graphics.DrawLine(new Pen(Color.Red, 2), xx * cw, yy * ch + yyshift, xx*cw+rect.Width, yy * ch + yyshift+ms.Height);
                        e.Graphics.DrawLine(new Pen(Color.Red, 2), xx * cw, yy * ch + yyshift + ms.Height, xx * cw + rect.Width, yy * ch + yyshift);
                    }
                    yyshift += (int)ms.Height;
                }
                xx++;
                if (xx == 7)
                {
                    yy++;
                    xx = 0;
                }
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            current = dateTimePicker1.Value;
            pictureBox1.Invalidate();
        }
    }
}
