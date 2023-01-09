using System;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace BMPtoRaw
{
    public partial class Form1 : Form
    {
        private static int mode = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mode = 1;
            openFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mode = 2;
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (File.Exists(openFileDialog1.FileName))
            {
                FileInfo bf = new FileInfo(openFileDialog1.FileName);
                string nn = bf.Directory.FullName + @"\" + bf.Name.Remove(bf.Name.Length - 4, 4);
                Bitmap bmp = new Bitmap(openFileDialog1.FileName);
                if (bmp.Palette.Entries.Length == 256)
                {
                    if (mode == 1)
                    {
                        FileInfo f = new FileInfo(nn + ".pal");
                        if (f.Exists) f.Delete();
                        FileStream fs = f.Create();
                        foreach (Color c in bmp.Palette.Entries)
                        {
                            byte[] ar = { c.R, c.G, c.B, 0 };
                            fs.Write(ar, 0, 4);
                        }
                        fs.Close();
                    }
                    FileInfo f2 = new FileInfo(nn + (mode == 1 ? ".raw" : "-ui.bmp"));
                    if (f2.Exists) f2.Delete();
                    FileStream fs2 = f2.Create();
                    if (mode == 2)
                    {
                        byte[] pic_size = { 0, 0, 0, 0 };
                        pic_size[0] = (byte)(bmp.Width % 256);
                        pic_size[1] = (byte)(bmp.Width / 256);
                        pic_size[2] = (byte)(bmp.Height % 256);
                        pic_size[3] = (byte)(bmp.Height / 256);
                        fs2.Write(pic_size, 0, 4);
                    }

                    BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format8bppIndexed);
                    unsafe
                    {
                        byte* pointer = (byte*)(data.Scan0.ToPointer());
                        int stride = data.Stride;
                        int height = data.Height;
                        byte* position;
                        for (int i = 0; i < height; i++)
                        {
                            position = pointer + stride * i;
                            for (int j = 0; j < stride; j++)
                            {
                                byte[] aar = { *position };
                                fs2.Write(aar, 0, 1);
                                position++;
                            }
                        }
                    }
                    fs2.Close();
                }
                else
                    MessageBox.Show("Only 256 color bmp");
                bmp.Dispose();
            }
            mode = 0;
        }
    }
}
