﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace NPush.Views.Tools
{
    class SelectorForm : Form
    {
        public bool CleanDraw;
        public Point Start { get; set; }
        public Point End { get; set; }
        private readonly Pen pen = new Pen(Color.FromArgb(100, 100, 100), 1);
        private readonly SolidBrush brush = new SolidBrush(Color.FromArgb(150, 255, 255, 255));

        public SelectorForm()
        {
            this.Opacity = 0.2f;
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(255, 255, 254);
            this.TransparencyKey = this.BackColor;
            this.WindowState = FormWindowState.Maximized;

            this.Paint += OnPaint;
        }

        internal void Initialize()
        {
            this.Start = Point.Empty;
            this.End = Point.Empty;

            this.CreateGraphics().Clear(this.BackColor);

            this.CleanDraw = true;
            this.Refresh();
            this.CleanDraw = false;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);
            e.Graphics.FillRectangle(this.brush, this.getRectangle());
            e.Graphics.DrawRectangle(this.pen, this.getRectangle());

            if (this.CleanDraw)
                e.Graphics.Clear(this.BackColor);
        }

        public Rectangle getRectangle()
        {
            return new Rectangle(Math.Min(this.Start.X, this.End.X), Math.Min(this.Start.Y, this.End.Y), Math.Abs(this.End.X - this.Start.X), Math.Abs(this.End.Y - this.Start.Y));
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
    }
}
