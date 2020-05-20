﻿using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;

// Sonic '06 Randomiser Suite is licensed under the MIT License:
/*
 * MIT License

 * Copyright (c) 2020 Knuxfan24
 * Copyright (c) 2020 HyperPolygon64

 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace Unify.TabControl
{
    public class UnifyTabControl : System.Windows.Forms.TabControl
    {
        /// <summary>
        ///     Format of the title of the TabPage
        /// </summary>
        private readonly StringFormat CenterSringFormat = new StringFormat {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center
        };

        /// <summary>
        ///     The color of the active tab header
        /// </summary>
        private Color activeColor = Color.FromArgb(111, 24, 123);

        /// <summary>
        ///     The color of the background of the Tab
        /// </summary>
        private Color backTabColor = Color.FromArgb(28, 28, 28);

        /// <summary>
        ///     The color of the border of the control
        /// </summary>
        private Color borderColor = Color.FromArgb(30, 30, 30);

        /// <summary>
        ///     Message for the user before losing
        /// </summary>
        private string closingMessage;

        /// <summary>
        ///     The color of the tab header
        /// </summary>
        private Color headerColor = Color.FromArgb(45, 45, 48);

        /// <summary>
        ///     The color of the horizontal line which is under the headers of the tab pages
        /// </summary>
        private Color horizLineColor = Color.FromArgb(111, 24, 123);

        /// <summary>
        ///     A random page will be used to store a tab that will be deplaced in the run-time
        /// </summary>
        private TabPage predraggedTab;

        /// <summary>
        ///     The color of the text
        /// </summary>
        private Color textColor = Color.FromArgb(255, 255, 255);
        
        ///<summary>
        /// Shows closing buttons
        /// </summary> 
        public bool ShowClosingButton { get; set; }

        /// <summary>
        /// Selected tab text color
        /// </summary>
        public Color selectedTextColor = Color.FromArgb(255, 255, 255);
        /// <summary>
        ///     Init
        /// </summary>
        public UnifyTabControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw
                | ControlStyles.OptimizedDoubleBuffer,
                true);
            DoubleBuffered = true;
            SizeMode = TabSizeMode.Normal;
            ItemSize = new Size(240, 16);
            AllowDrop = true;
        }

        [Category("Colors"), Browsable(true), Description("The color of the selected page")]
        public Color ActiveColor
        {
            get
            {
                return this.activeColor;
            }

            set
            {
                this.activeColor = value;
            }
        }

        [Category("Colors"), Browsable(true), Description("The color of the background of the tab")]
        public Color BackTabColor
        {
            get
            {
                return this.backTabColor;
            }

            set
            {
                this.backTabColor = value;
            }
        }

        [Category("Colors"), Browsable(true), Description("The color of the border of the control")]
        public Color BorderColor
        {
            get
            {
                return this.borderColor;
            }

            set
            {
                this.borderColor = value;
            }
        }

        /// <summary>
        ///     The color of the closing button
        /// </summary>
        [Category("Colors"), Browsable(true), Description("The color of the closing button")]
        public Color ClosingButtonColor { get; set; } = Color.WhiteSmoke;

        /// <summary>
        ///     The message that will be shown before closing.
        /// </summary>
        [Category("Options"), Browsable(true), Description("The message that will be shown before closing.")]
        public string ClosingMessage
        {
            get
            {
                return this.closingMessage;
            }

            set
            {
                this.closingMessage = value;
            }
        }

        [Category("Colors"), Browsable(true), Description("The color of the header.")]
        public Color HeaderColor
        {
            get
            {
                return this.headerColor;
            }

            set
            {
                this.headerColor = value;
            }
        }

        [Category("Colors"), Browsable(true),
         Description("The color of the horizontal line which is located under the headers of the pages.")]
        public Color HorizontalLineColor
        {
            get
            {
                return this.horizLineColor;
            }

            set
            {
                this.horizLineColor = value;
            }
        }

        /// <summary>
        ///     Show a Yes/No message before closing?
        /// </summary>
        [Category("Options"), Browsable(true), Description("Show a Yes/No message before closing?")]
        public bool ShowClosingMessage { get; set; }

        [Category("Colors"), Browsable(true), Description("The color of the title of the page")]
        public Color SelectedTextColor
        {
            get
            {
                return this.selectedTextColor;
            }

            set
            {
                this.selectedTextColor = value;
            }
        }

        [Category("Colors"), Browsable(true), Description("The color of the title of the page")]
        public Color TextColor
        {
            get
            {
                return this.textColor;
            }

            set
            {
                this.textColor = value;
            }
        }

        private bool noTabDisplay = false;

        public bool NoTabDisplay {
            get { return this.noTabDisplay; }
            set { if (this.noTabDisplay = value) if (!DesignMode) this.Top -= 20; }
        }

        /// <summary>
        ///     Sets the Tabs on the top
        /// </summary>
        protected override void CreateHandle()
        {
            base.CreateHandle();
            Alignment = TabAlignment.Top;
        }

        private bool allowDragging = false;

        public bool AllowDragging {
            get { return this.allowDragging; }
            set { this.allowDragging = value; }
        }

        /// <summary>
        ///     Drags the selected tab
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if (AllowDragging)
            {
                var draggedTab = (TabPage)drgevent.Data.GetData(typeof(TabPage));
                var pointedTab = getPointedTab();

                if (ReferenceEquals(draggedTab, predraggedTab) && pointedTab != null)
                {
                    drgevent.Effect = DragDropEffects.Move;

                    if (!ReferenceEquals(pointedTab, draggedTab))
                    {
                        this.ReplaceTabPages(draggedTab, pointedTab);
                    }
                }

                base.OnDragOver(drgevent);
            }
        }

        /// <summary>
        ///     Handles the selected tab|closes the selected page if wanted.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (AllowDragging)
            {
                predraggedTab = getPointedTab();
                var p = e.Location;

                if (this.ShowClosingButton)
                {
                    for (var i = 0; i < this.TabCount; i++)
                    {
                        var r = this.GetTabRect(i);
                        r.Offset(r.Width - 15, 2);
                        r.Width = 10;
                        r.Height = 10;
                        if (!r.Contains(p))
                        {
                            continue;
                        }

                        if (this.ShowClosingMessage)
                        {
                            if (DialogResult.Yes == MessageBox.Show(this.ClosingMessage, "Close", MessageBoxButtons.YesNo))
                            {
                                this.TabPages.RemoveAt(i);
                            }
                        }
                        else
                        {
                            this.TabPages.RemoveAt(i);
                        }
                    }
                }

                base.OnMouseDown(e);
            }
        }

        /// <summary>
        ///     Holds the selected page until it sets down
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (AllowDragging)
            {
                if (e.Button == MouseButtons.Left && predraggedTab != null)
                {
                    this.DoDragDrop(predraggedTab, DragDropEffects.Move);
                }

                base.OnMouseMove(e);
            }
        }

        /// <summary>
        ///     Abandons the selected tab
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (AllowDragging) {
                predraggedTab = null;
                base.OnMouseUp(e);
            }
        }

        /// <summary>
        ///     Draws the control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!NoTabDisplay || DesignMode)
            {
                var g = e.Graphics;
                var Drawer = g;

                Drawer.SmoothingMode = SmoothingMode.HighQuality;
                Drawer.PixelOffsetMode = PixelOffsetMode.HighQuality;
                Drawer.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                Drawer.Clear(this.headerColor);

                try
                {
                    SelectedTab.BackColor = this.backTabColor;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    SelectedTab.BorderStyle = BorderStyle.None;
                }
                catch
                {
                    // ignored
                }

                for (var i = 0; i <= TabCount - 1; i++)
                {
                    var Header = new Rectangle(
                        new Point(GetTabRect(i).Location.X + 2, GetTabRect(i).Location.Y),
                        new Size(GetTabRect(i).Width, GetTabRect(i).Height));
                    var HeaderSize = new Rectangle(Header.Location, new Size(Header.Width, Header.Height));
                    Brush ClosingColorBrush = new SolidBrush(this.ClosingButtonColor);

                    if (i == SelectedIndex)
                    {
                        // Draws the back of the header 
                        Drawer.FillRectangle(new SolidBrush(this.headerColor), HeaderSize);

                        // Draws the back of the color when it is selected
                        Drawer.FillRectangle(
                            new SolidBrush(this.activeColor),
                            new Rectangle(Header.X - 5, Header.Y - 3, Header.Width, Header.Height + 5));

                        // Draws the title of the page
                        Drawer.DrawString(
                            TabPages[i].Text,
                            Font,
                            new SolidBrush(this.selectedTextColor),
                            HeaderSize,
                            this.CenterSringFormat);

                        // Draws the closing button
                        if (this.ShowClosingButton)
                        {
                            e.Graphics.DrawString("x", Font, ClosingColorBrush, HeaderSize.Right - 17, 0);
                        }
                    }
                    else
                    {
                        // Simply draws the header when it is not selected
                        Drawer.DrawString(
                            TabPages[i].Text,
                            Font,
                            new SolidBrush(this.textColor),
                            HeaderSize,
                            this.CenterSringFormat);
                    }
                }

                // Draws the horizontal line
                Drawer.DrawLine(new Pen(this.horizLineColor, 5), new Point(0, 21), new Point(Width, 21));

                // Draws the background of the tab control
                Drawer.FillRectangle(new SolidBrush(this.backTabColor), new Rectangle(0, 20, Width, Height - 20));

                // Draws the border of the TabControl
                Drawer.DrawRectangle(new Pen(this.borderColor, 2), new Rectangle(0, 0, Width, Height));
                Drawer.InterpolationMode = InterpolationMode.HighQualityBicubic;
            } else {
                var g = e.Graphics;
                var Drawer = g;

                Drawer.SmoothingMode = SmoothingMode.HighQuality;
                Drawer.PixelOffsetMode = PixelOffsetMode.HighQuality;
                Drawer.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                Drawer.Clear(this.backTabColor);

                for (var i = 0; i <= TabCount - 1; i++)
                {
                    var Header = new Rectangle(
                                    new Point(GetTabRect(i).Location.X, GetTabRect(i).Location.Y),
                                    new Size(0, 0));
                var HeaderSize = new Rectangle(Header.Location, new Size(Header.Width, Header.Height));

                    if (i == SelectedIndex)
                    {
                        // Draws the back of the header 
                        Drawer.FillRectangle(new SolidBrush(this.headerColor), HeaderSize);

                        // Draws the back of the color when it is selected
                        Drawer.FillRectangle(
                            new SolidBrush(this.activeColor),
                            new Rectangle(Header.X, Header.Y, Header.Width, Header.Height));
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the pointed tab
        /// </summary>
        /// <returns></returns>
        private TabPage getPointedTab()
        {
            for (var i = 0; i <= this.TabPages.Count - 1; i++)
            {
                if (this.GetTabRect(i).Contains(this.PointToClient(Cursor.Position)))
                {
                    return this.TabPages[i];
                }
            }

            return null;
        }

        /// <summary>
        ///     Swaps the two tabs
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Destination"></param>
        private void ReplaceTabPages(TabPage Source, TabPage Destination)
        {
            var SourceIndex = this.TabPages.IndexOf(Source);
            var DestinationIndex = this.TabPages.IndexOf(Destination);

            this.TabPages[DestinationIndex] = Source;
            this.TabPages[SourceIndex] = Destination;

            if (this.SelectedIndex == SourceIndex)
            {
                this.SelectedIndex = DestinationIndex;
            }
            else if (this.SelectedIndex == DestinationIndex)
            {
                this.SelectedIndex = SourceIndex;
            }

            this.Refresh();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UnifyTabControl
            // 
            this.ResumeLayout(false);
        }
    }
}
