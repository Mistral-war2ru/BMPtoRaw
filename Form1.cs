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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
                    FileInfo f = new FileInfo(nn + ".pal");
                    if (f.Exists) f.Delete();
                    FileStream fs = f.Create();
                    foreach (Color c in bmp.Palette.Entries)
                    {
                        byte[] ar = { c.R, c.G, c.B, 0 };
                        fs.Write(ar, 0, 4);
                    }
                    fs.Close();
                    FileInfo f2 = new FileInfo(nn + ".raw");
                    if (f2.Exists) f2.Delete();
                    FileStream fs2 = f2.Create();

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
        }
    }
}
