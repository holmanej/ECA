﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECA
{
    public partial class Form1 : Form
    {

        private ECA eca;
        private Bitmap Pattern = new Bitmap(1, 1);

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
                Text = "Rule Number"
            };
            controlPanel.Controls.Add(rule_Label);
            TextBox rule_TextBox = new TextBox()
            {
                Location = new Point(20, 50),
                Size = new Size(100, 30),
                Text = "30"
            };
            controlPanel.Controls.Add(rule_TextBox);

            Label random_Label = new Label()
            {
                Location = new Point(20, 100),
                Size = new Size(150, 20),
                Text = "Density Percentage"
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
            gfx.DrawImage(Pattern, 205, 5);
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

            Size area = new Size(Width - 200, Height);
            Pattern = new Bitmap(area.Width, area.Height);
            Graphics gfx = CreateGraphics();
            SolidBrush grayBrush = new SolidBrush(Color.Gray);
            gfx.FillRectangle(grayBrush, 200, 0, area.Width, area.Height);

            area.Width -= 5;
            area.Height -= 5;
            eca = new ECA(rule, density, cb.Checked, area);

            for (int i = 0; i < area.Height - 1; i++)
            {
                for (int j = 0; j < area.Width - 1; j++)
                {
                    if (eca.Field[j, i] == 1)
                    {
                        Pattern.SetPixel(j, i, Color.Black);
                    }
                    else
                    {
                        Pattern.SetPixel(j, i, Color.White);
                    }
                }
                log.Clear();
                log.AppendText(((i * 1000) / area.Height) / 10 + "%");
            }
            gfx.DrawImage(Pattern, 205, 5);
            log.Clear();
            log.AppendText("100%\r\nDone!");
        }
    }
}
