using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace imgcrypt
{
    class Pixelator
    {

        public static Bitmap pixelate(string filePath)
        {
            Random rnd = new Random();
            string a = Convert.ToBase64String(System.IO.File.ReadAllBytes(filePath));
            char[] aR = a.ToCharArray();
            double sq = Math.Sqrt(aR.Length);
            int autosize = ((int)sq) + 2;
            Bitmap imageholder = new Bitmap(autosize, autosize);
            Graphics g = Graphics.FromImage(imageholder);
            int fff = 0;
            while (fff <= aR.Length - 1)
            {
                for (int y = 1; y <= imageholder.Height - 1; y++)
                {
                    for (int x = 1; x <= imageholder.Width - 1; x++)
                    {
                        if (fff <= aR.Length - 1)
                        {                            
                            int green = rnd.Next(0, 255);
                            int blue = rnd.Next(0, 255);

                            int charCode = aR[fff];
                            imageholder.SetPixel(x, y, Color.FromArgb(charCode, 0, 0));
                            fff++;
                        }
                    }
                }
            }
            return imageholder;
        }

        public static byte[] depixelate(Bitmap img)
        {
            string holder = "";
            int xmax = img.Width - 1;
            int ymax = img.Height - 1;
            for (int y = 1; y <= ymax; y++)
            {
                for (int x = 1; x <= xmax; x++)
                {
                    Color c = img.GetPixel(x, y);
                    holder += (char)c.R;
                }
            }

            return Convert.FromBase64String(holder.Replac​e(Convert.ToChar(0).ToString(), ""));
        }
  
    }
}
