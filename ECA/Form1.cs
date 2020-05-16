using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECA
{
    public partial class Form1 : Form
    {

        private ECA eca;
        private Bitmap Pattern = new Bitmap(1, 1);
        private Rectangle renderArea = new Rectangle(0, 0, 3000, 1100);

        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            Paint += Form1_Paint;

            Panel controlPanel = new Panel()
            {
                Location = Location,
                Size = new Size(200, Height)
            };
            Controls.Add(controlPanel);
            Label rule_Label = new Label()
            {
                Location = new Point(20, 20),
                Size = new Size(150, 20),
                Text = "Rule Number [0-255]"
            };
            controlPanel.Controls.Add(rule_Label);
            TextBox rule_TextBox = new TextBox()
            {
                Location = new Point(20, 50),
                Size = new Size(100, 20),
                Text = "30"
            };
            controlPanel.Controls.Add(rule_TextBox);

            Label random_Label = new Label()
            {
                Location = new Point(20, 100),
                Size = new Size(150, 20),
                Text = "Density Percentage [0-100]"
            };
            controlPanel.Controls.Add(random_Label);
            CheckBox random_CheckBox = new CheckBox()
            {
                Location = new Point(20, 130),
                Size = new Size(20, 20)
            };
            controlPanel.Controls.Add(random_CheckBox);
            TextBox random_TextBox = new TextBox()
            {
                Location = new Point(50, 130),
                Text = "50"
            };
            controlPanel.Controls.Add(random_TextBox);

            Button generate_Button = new Button()
            {
                Location = new Point(20, 160),
                Size = new Size(100, 30),
                Text = "Generate!"
            };
            generate_Button.Click += Generate_Button_Click;
            controlPanel.Controls.Add(generate_Button);

            TextBox log_TextBox = new TextBox()
            {
                Location = new Point(20, 200),
                Size = new Size(100, 100),
                Multiline = true,
                ReadOnly = true
            };
            controlPanel.Controls.Add(log_TextBox);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gfx = CreateGraphics();
            Size area = new Size(Width - 200, Height);
            SolidBrush grayBrush = new SolidBrush(Color.Gray);
            gfx.FillRectangle(grayBrush, 200, 0, area.Width, area.Height);
            gfx.DrawImage(CropPattern(), 205, 5);
        }

        private void Generate_Button_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            var CC = b.Parent.Controls;
            foreach (Control c in b.Parent.Controls)
            {
                Debug.WriteLine(c.Text);
            }
            CheckBox cb = (CheckBox)CC[3];
            int rule = int.Parse(CC[1].Text);
            if (rule < 0)
            {
                rule = 0;
                CC[1].Text = "0";
            }
            if (rule > 255)
            {
                rule = 255;
                CC[1].Text = "255";
            }
            int density = int.Parse(CC[4].Text);
            if (density < 0)
            {
                density = 0;
                CC[4].Text = "0";
            }
            if (density > 100)
            {
                density = 100;
                CC[4].Text = "100";
            }

            TextBox log = (TextBox)CC[6];
            log.Clear();

            Pattern = new Bitmap(renderArea.Width, renderArea.Height, PixelFormat.Format32bppRgb);
            Graphics gfx = CreateGraphics();
            SolidBrush grayBrush = new SolidBrush(Color.Gray);
            gfx.FillRectangle(grayBrush, b.Parent.Width, 0, Width - b.Parent.Width, Height);

            eca = new ECA(rule, density, cb.Checked, renderArea.Size);

            BitmapData bmpData = Pattern.LockBits(renderArea, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * Pattern.Height;
            Debug.WriteLine(bytes);
            byte[] patternValues = new byte[bytes];
            Marshal.Copy(ptr, patternValues, 0, bytes);

            for (int i = 0; i < renderArea.Height - 1; i++)
            {
                Parallel.For(0, renderArea.Width - 1, (j) =>
               {
                   int coordinate = (i * 4 * renderArea.Width) + (j * 4);
                   if (eca.Field[j, i] == 1)
                   {
                       patternValues[coordinate] = 0;
                       patternValues[coordinate + 1] = 0;
                       patternValues[coordinate + 2] = 0;
                       patternValues[coordinate + 3] = 255;

                   }
                   else
                   {
                       patternValues[coordinate] = 255;
                       patternValues[coordinate + 1] = 255;
                       patternValues[coordinate + 2] = 255;
                       patternValues[coordinate + 3] = 255;
                   }
               });
                log.Clear();
                log.AppendText(i * 1000 / renderArea.Height / 10 + "%");
            }
            Marshal.Copy(patternValues, 0, ptr, bytes);
            Pattern.UnlockBits(bmpData);
            gfx.DrawImage(CropPattern(), 205, 5);
            log.Clear();
            log.AppendText("100%\r\nDone!");
        }

        private Bitmap CropPattern()
        {
            int w = Width - 200;
            int h = Height;
            Bitmap b = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(b);
            g.DrawImage(Pattern, new Rectangle(0, 0, w, h), new Rectangle((Pattern.Width - w) / 2, 0, w, h), GraphicsUnit.Pixel);
            return b;
        }
    }
}
