using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace MediaLib
{
    public class BitmapHelper
    {
        public static Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = File.Open(fileName, FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);
                bitmap = new Bitmap(image);
            }
            return bitmap;
        }

        public static BitmapSource GetBitmapFromMatrix(bool[,] matrix)
        {
            return BitmapToBitmapSource(BitMatrixToBitmap(matrix));
        }

        public static BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            };
        }

        public static Bitmap GetInitialFilledBitmap(int width, int height, Brush brush)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, width, height);
                graph.FillRectangle(brush, ImageSize);
            }
            return bmp;
        }

        public static unsafe Bitmap MatrixToBitmap(double[,] rawImage)
        {
            int width = rawImage.GetLength(1);
            int height = rawImage.GetLength(0);

            Bitmap Image = new Bitmap(width, height);
            BitmapData bitmapData = Image.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb
            );
            ColorARGB* startingPosition = (ColorARGB*)bitmapData.Scan0;


            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    double color = rawImage[i, j];
                    byte rgb = (byte)(color * 255);

                    ColorARGB* position = startingPosition + j + i * width;
                    position->A = 255;
                    position->R = rgb;
                    position->G = rgb;
                    position->B = rgb;
                }

            Image.UnlockBits(bitmapData);
            return Image;
        }

        public static unsafe Bitmap BitMatrixToBitmap(bool[,] rawImage)
        {
            int width = rawImage.GetLength(1);
            int height = rawImage.GetLength(0);

            Bitmap bmp = new Bitmap(width, height);
            BitmapData bData = bmp.LockBits(new Rectangle(new Point(), bmp.Size),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            byte* startingPosition = (byte*)bData.Scan0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    byte* position = startingPosition + j + i * width;
                    *position = (byte)(rawImage[i, j] ? 0 : 255);
                }
            }

            bmp.UnlockBits(bData);
            return bmp;
        }

        public static unsafe Bitmap MatrixToBitmap(bool[,] rawImage)
        {
            int width = rawImage.GetLength(1);
            int height = rawImage.GetLength(0);

            Bitmap Image = new Bitmap(width, height);
            BitmapData bitmapData = Image.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppRgb
            );

            ColorARGB* startingPosition = (ColorARGB*)bitmapData.Scan0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    byte rgb = (byte)(rawImage[i, j] ? 0 : 255);

                    ColorARGB* position = startingPosition + j + i * width;
                    position->A = 255;
                    position->R = rgb;
                    position->G = rgb;
                    position->B = rgb;
                }
            }

            Image.UnlockBits(bitmapData);
            return Image;
        }

        public struct ColorARGB
        {
            public byte B;
            public byte G;
            public byte R;
            public byte A;

            public ColorARGB(Color color)
            {
                A = color.A;
                R = color.R;
                G = color.G;
                B = color.B;
            }

            public ColorARGB(byte a, byte r, byte g, byte b)
            {
                A = a;
                R = r;
                G = g;
                B = b;
            }

            public Color ToColor()
            {
                return Color.FromArgb(A, R, G, B);
            }
        }
    }
}
