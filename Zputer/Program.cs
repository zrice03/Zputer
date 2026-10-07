using System.Drawing;
using ZputerLib;

Bitmap bmp = new Bitmap(100, 100);
Graphics g = Graphics.FromImage(bmp);
g.FillRectangle(new SolidBrush(Color.White), 0, 0, bmp.Width, bmp.Height);
DMux.Draw(bmp, g, 0, 0);
bmp.Save(@"C:\Users\zrice\OneDrive - Epic\Desktop\chip.png");
