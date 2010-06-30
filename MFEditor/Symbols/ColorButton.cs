﻿namespace MFEditor
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class ColorButton : Button
    {
        private bool m_bButtonPushed = false;
        private bool m_bIsAtBelow = true;
        public bool m_bPanelVisible = false;
        private Color m_ButtonColor = Color.Transparent;

        public event OnColorChangedEventHandler OnColorChanged;

        public ColorButton()
        {
            this.InitializeComponent();
            this.m_ButtonColor = Color.Transparent;
            this.m_bButtonPushed = false;
            this.m_bPanelVisible = false;
            this.m_bIsAtBelow = true;
            base.Text = "";
        }

        private void ColorButton_MouseDown(object sender, MouseEventArgs e)
        {
            this.m_bButtonPushed = true;
        }

        private void ColorButton_MouseUp(object sender, MouseEventArgs e)
        {
            this.m_bButtonPushed = false;
        }

        private void ColorButton_Paint(object sender, PaintEventArgs e)
        {
            Pen pen;
            base.Text = "";
            int num = 0;
            if (this.m_bPanelVisible || (this.m_bButtonPushed && base.RectangleToScreen(base.ClientRectangle).Contains(Cursor.Position)))
            {
                ControlPaint.DrawButton(e.Graphics, e.ClipRectangle, ButtonState.Pushed);
                num = 1;
            }
            Rectangle rect = new Rectangle((e.ClipRectangle.Left + 5) + num, (e.ClipRectangle.Top + 5) + num, e.ClipRectangle.Width - 0x18, e.ClipRectangle.Height - 11);
            Pen pen2 = new Pen(SystemColors.ControlDark);
            if (base.Enabled)
            {
                e.Graphics.FillRectangle(new SolidBrush(this.m_ButtonColor), rect);
                e.Graphics.DrawRectangle(pen2, rect);
            }
            e.Graphics.DrawLine(pen2, rect.Right + 4, rect.Top, rect.Right + 4, rect.Bottom);
            e.Graphics.DrawLine(new Pen(SystemColors.ControlLightLight), rect.Right + 5, rect.Top, rect.Right + 5, rect.Bottom);
            if (base.Enabled)
            {
                pen = new Pen(SystemColors.ControlText);
            }
            else
            {
                pen = new Pen(SystemColors.GrayText);
            }
            Point point = new Point(rect.Right, (e.ClipRectangle.Height + num) / 2);
            e.Graphics.DrawLine(pen, (int)(point.X + 9), (int)(point.Y - 1), (int)(point.X + 13), (int)(point.Y - 1));
            e.Graphics.DrawLine(pen, point.X + 10, point.Y, point.X + 12, point.Y);
            e.Graphics.DrawLine(pen, point.X + 11, point.Y, point.X + 11, point.Y + 1);
        }

        private void InitializeComponent()
        {
            base.Paint += new PaintEventHandler(this.ColorButton_Paint);
            base.MouseDown += new MouseEventHandler(this.ColorButton_MouseDown);
            base.MouseUp += new MouseEventHandler(this.ColorButton_MouseUp);
        }

        public bool LocationAtBelow
        {
            get
            {
                return this.m_bIsAtBelow;
            }
            set
            {
                this.m_bIsAtBelow = value;
            }
        }

        public Color ButtonColor
        {
            get
            {
                return this.m_ButtonColor;
            }
            set
            {
                if (this.m_ButtonColor != value)
                {
                    this.m_ButtonColor = value;
                    base.Invalidate();
                    if (this.OnColorChanged != null)
                    {
                        this.OnColorChanged(this.m_ButtonColor);
                    }
                }
            }
        }

        public delegate void OnColorChangedEventHandler(Color newColor);
    }
}

