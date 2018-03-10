using System.Drawing;
using System.Drawing.Drawing2D;

namespace MediaLib
{
    public static class ShapeHelper
    {
        public static Bitmap AddCircle(Bitmap bitmap, Brush brush, System.Windows.Point center, double radius)
        {
            using (Graphics grf = Graphics.FromImage(bitmap))
            {
                PointF rectOrigin = new PointF((float)(center.X - radius), (float)(center.Y - radius));
                RectangleF rect = new RectangleF(rectOrigin, new SizeF((float)radius * 2F, (float)radius * 2F));
                grf.DrawEllipse(new Pen(brush, 2), rect);
            }

            return bitmap;

        }

        public static Bitmap CropToCircle(Bitmap bitmap, Color excludeColor, System.Windows.Point center, double radius)
        {
            PointF rectOrigin = new PointF((float)(center.X - radius), (float)(center.Y - radius));
            RectangleF rect = new RectangleF(rectOrigin, new SizeF((float)radius * 2f, (float)radius * 2f));

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                GraphicsPath clipPath = new GraphicsPath();
                clipPath.AddEllipse(rect);

                graphics.SetClip(clipPath, CombineMode.Exclude);

                var color = Color.FromArgb(80, excludeColor);
                using (SolidBrush brush = new SolidBrush(color))
                {
                    graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
                }

                graphics.DrawPath(Pens.Transparent, clipPath);
            }

            return bitmap;
        }
    }
}
