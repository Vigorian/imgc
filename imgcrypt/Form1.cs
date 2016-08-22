using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace imgcrypt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void pickFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.DefaultExt = "exe";
            o.Filter = "Any Files|*.*";
            if (o.ShowDialog() == DialogResult.OK)
            {
                file.Text = o.FileName;
            }
        }

        private void doWork_Click(object sender, EventArgs e)
        {
            string filePath = file.Text;
            Bitmap img = Pixelator.pixelate(filePath);
            outputImage.Image = img;
        }

        private void saveImg_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.DefaultExt = "bmp";
            s.Filter = "PNG Files|*.png";
            if (s.ShowDialog() == DialogResult.OK)
            {
                Bitmap img = new Bitmap(outputImage.Image);
                img.Save(s.FileName);
            }
        }

        private void loadImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.DefaultExt = "bmp";
            o.Filter = "PNG Files|*.png";
            if (o.ShowDialog() == DialogResult.OK)
            {
                Bitmap img = new Bitmap(o.FileName);
                byte[] output = Pixelator.depixelate(img);

                SaveFileDialog s = new SaveFileDialog();
                s.DefaultExt = "exe";
                s.Filter = "Exe Files|*.exe";
                if (s.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllBytes(s.FileName, output);
                }
            }
        }
  
    }
}