using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Security.Cryptography;

namespace NewFMS
{
    public class CaptchaImage
    {
        public Bitmap Image
        {
            get { return this.image; }
        }

        private string text;
        private int width;
        private int height;
        private System.Drawing.FontFamily fontFamily;
        private Bitmap image;

        private Random random = new Random();

        public CaptchaImage(int width, int height, System.Drawing.FontFamily fontFamily)
        {
            this.width = width;
            this.height = height;
            this.fontFamily = fontFamily;
        }
        //public CaptchaImage(string s, int width, int height, System.Drawing.FontFamily fontFamily)
        //{
        //    this.text = s;
        //    this.width = width;
        //    this.height = height;
        //    this.fontFamily = fontFamily;
        //}
        public string CreateRandomText(int Length)
        {

            Random r = new Random();
            string s = "";
            for (int j = 0; j < 5; j++)
            {
                int i = r.Next(3);
                int ch;
                switch (i)
                {
                    case 1:
                        ch = r.Next(0, 9);
                        s = s + ch.ToString();
                        break;
                    case 2:
                        ch = r.Next(65, 90);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    case 3:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    default:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                }
                r.NextDouble();
                r.Next(100000, 999999);
            }
            return s;

        }

        private void DrawRandomLines(Graphics g)
        {
            SolidBrush color = new SolidBrush(Color.DarkGray);
            for (int i = 0; i < 3; i++)
            {
                g.DrawLines(new Pen(color, 2), GetRandomPoints());
            }
        }
        private Point[] GetRandomPoints()
        {
            Point[] points = { new Point(random.Next(0, 90), random.Next(0, 40)), new Point(random.Next(0, 40), random.Next(0, 90)) };
            return points;
        }


        public void GenerateImage()
        {
            //// Create a new 32-bit bitmap image.
            //Bitmap bitmap = new Bitmap(this.width, this.height, PixelFormat.Format32bppArgb);

            //// Create a graphics object for drawing.
            //Graphics g = Graphics.FromImage(bitmap);
            //g.PageUnit = GraphicsUnit.Pixel;
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            //Rectangle rect = new Rectangle(0, 0, this.width, this.height);

            //// Fill in the background.
            //HatchBrush hatchBrush = new HatchBrush(HatchStyle.DottedGrid, Color.LightGray, Color.White);
            //g.FillRectangle(hatchBrush, rect);

            //// Set up the text font.
            //SizeF size;
            //float fontSize = rect.Height + 1;
            //Font font;
            //// Adjust the font size until the text fits within the image.
            //do
            //{
            //    fontSize--;
            //    font = new Font(this.fontFamily.Name, fontSize, GraphicsUnit.Pixel);
            //    size = g.MeasureString(this.text, font);
            //} while (size.Width > rect.Width);

            //// Set up the text format.
            //StringFormat format = new StringFormat();
            //format.Alignment = StringAlignment.Center;
            //format.LineAlignment = StringAlignment.Center;

            //// Create a path using the text and warp it randomly.
            //GraphicsPath path = new GraphicsPath();

            //path.AddString(this.text, font.FontFamily, (int)font.Style, font.Size, rect, format);
            //float v = 4F;
            //PointF[] points =
            //{
            //    new PointF(this.random.Next(rect.Width) / v, this.random.Next(rect.Height) / v),
            //    new PointF(rect.Width - this.random.Next(rect.Width) / v, this.random.Next(rect.Height) / v),
            //    new PointF(this.random.Next(rect.Width) / v, rect.Height - this.random.Next(rect.Height) / v),
            //    new PointF(rect.Width - this.random.Next(rect.Width) / v, rect.Height - this.random.Next(rect.Height) / v)
            //};
            //Matrix matrix = new Matrix();
            //matrix.Translate(0F, 0F);
            //path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

            //// Draw the text.
            //hatchBrush = new HatchBrush(HatchStyle.Shingle, Color.LightGray, Color.DarkGray);
            //g.FillPath(hatchBrush, path);

            //// Add some random noise.
            //int m = Math.Max(rect.Width, rect.Height);
            //for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            //{
            //    int x = this.random.Next(rect.Width);
            //    int y = this.random.Next(rect.Height);
            //    int w = this.random.Next(m / 50);
            //    int h = this.random.Next(m / 50);
            //    g.FillEllipse(hatchBrush, x, y, w, h);
            //}

            //// Clean up.
            //font.Dispose();
            //hatchBrush.Dispose();
            ////DrawRandomLines(g);
            //g.Dispose();
            //// Set the image.
            //this.image = bitmap;



            Bitmap bitmap = new Bitmap
            (this.width, this.height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, this.width, this.height);
            HatchBrush hatchBrush = new HatchBrush(HatchStyle.SmallConfetti,
                Color.DarkGray, Color.White);
            g.FillRectangle(hatchBrush, rect);
            SizeF size;
            float fontSize = rect.Height + 1;
            Font font;

            do
            {
                fontSize--;
                font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold);
                size = g.MeasureString(this.text, font);
            } while (size.Width > rect.Width);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            GraphicsPath path = new GraphicsPath();
            //path.AddString(this.text, font.FontFamily, (int) font.Style, 
            //    font.Size, rect, format);
            path.AddString(this.text, font.FontFamily, (int)font.Style, 25, rect, format);
            float v = 10F;
            PointF[] points =
          {
                new PointF(this.random.Next(rect.Width) / v, this.random.Next(
                   rect.Height) / v),
                new PointF(rect.Width - this.random.Next(rect.Width) / v, 
                    this.random.Next(rect.Height) / v),
                new PointF(this.random.Next(rect.Width) / v, 
                    rect.Height - this.random.Next(rect.Height) / v),
                new PointF(rect.Width - this.random.Next(rect.Width) / v,
                    rect.Height - this.random.Next(rect.Height) / v)
          };
            Matrix matrix = new Matrix();
            matrix.Translate(0F, 0F);
            path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
            hatchBrush = new HatchBrush(HatchStyle.Percent10, Color.Black, Color.Gray);
            g.FillPath(hatchBrush, path);
            int m = Math.Max(rect.Width, rect.Height);
            for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            {
                int x = this.random.Next(rect.Width);
                int y = this.random.Next(rect.Height);
                int w = this.random.Next(m / 50);
                int h = this.random.Next(m / 50);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }
            font.Dispose();
            hatchBrush.Dispose();
            DrawRandomLines(g);
            g.Dispose();
            this.image = bitmap;



            //          // Create a new 32-bit bitmap image.
            //          Bitmap bitmap = new Bitmap(
            //            this.width,
            //            this.height,
            //            PixelFormat.Format32bppArgb);

            //          // Create a graphics object for drawing.
            //          Graphics g = Graphics.FromImage(bitmap);
            //          g.SmoothingMode = SmoothingMode.AntiAlias;
            //          Rectangle rect = new Rectangle(0, 0, this.width, this.height);

            //          // Fill in the background.
            //          HatchBrush hatchBrush = new HatchBrush(
            //            HatchStyle.SmallConfetti,
            //            Color.LightGray,
            //            Color.White);
            //          g.FillRectangle(hatchBrush, rect);

            //          // Set up the text font.
            //          SizeF size;
            //          float fontSize = rect.Height + 1;
            //          Font font;
            //          // Adjust the font size until the text fits within the image.
            //          do
            //          {
            //              fontSize--;
            //              font = new Font(
            //                this.fontFamily,
            //                fontSize,
            //                FontStyle.Bold);
            //              size = g.MeasureString(this.text, font);
            //          } while (size.Width > rect.Width);

            //          // Set up the text format.
            //          StringFormat format = new StringFormat();
            //          format.Alignment = StringAlignment.Center;
            //          format.LineAlignment = StringAlignment.Center;

            //          // Create a path using the text and warp it randomly.
            //          GraphicsPath path = new GraphicsPath();
            //          path.AddString(
            //            this.text,
            //            font.FontFamily,
            //            (int)font.Style,
            //            font.Size, rect,
            //            format);
            //          float v = 4F;
            //          PointF[] points =
            //{
            //  new PointF(
            //    this.random.Next(rect.Width) / v,
            //    this.random.Next(rect.Height) / v),
            //  new PointF(
            //    rect.Width - this.random.Next(rect.Width) / v,
            //    this.random.Next(rect.Height) / v),
            //  new PointF(
            //    this.random.Next(rect.Width) / v,
            //    rect.Height - this.random.Next(rect.Height) / v),
            //  new PointF(
            //    rect.Width - this.random.Next(rect.Width) / v,
            //    rect.Height - this.random.Next(rect.Height) / v)
            //};
            //          Matrix matrix = new Matrix();
            //          matrix.Translate(0F, 0F);
            //          path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

            //          // Draw the text.
            //          hatchBrush = new HatchBrush(
            //            HatchStyle.LargeConfetti,
            //            Color.LightGray,
            //            Color.DarkGray);
            //          g.FillPath(hatchBrush, path);

            //          // Add some random noise.
            //          int m = Math.Max(rect.Width, rect.Height);
            //          for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            //          {
            //              int x = this.random.Next(rect.Width);
            //              int y = this.random.Next(rect.Height);
            //              int w = this.random.Next(m / 50);
            //              int h = this.random.Next(m / 50);
            //              g.FillEllipse(hatchBrush, x, y, w, h);
            //          }

            //          // Clean up.
            //          font.Dispose();
            //          hatchBrush.Dispose();
            //          DrawRandomLines(g);
            //          g.Dispose();

            //          // Set the image.
            //          this.image = bitmap;

        }

        public void SetText(string text)
        {
            this.text = text;
        }
    }
}