using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Wyvern
{
    [DefaultProperty("ShowSplit")]
    public class SplitButton : Button
    {
        private PushButtonState _state;
        private const int PushButtonWidth = 14;
        private static int BorderSize = SystemInformation.Border3DSize.Width * 2;
        private bool skipNextOpen;
        private Rectangle dropDownRectangle;
        private Rectangle UpRectangle, DownRectangle;
        private bool showSplit = true;
        public event EventHandler UpClickMouseDown, UpClickMouseUp, DownClickMouseDown, DownClickMouseUp;

        [DefaultValue(true)]
        [Category("Appearance")]
        [DisplayName("Show Split")]
        public bool ShowSplit
        {
            set
            {
                if (value != showSplit)
                {
                    showSplit = value;
                    Invalidate();
                    if (this.Parent != null)
                    {
                        this.Parent.PerformLayout();
                    }
                }
            }
        }

        private PushButtonState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (!_state.Equals(value))
                {
                    _state = value;
                    Invalidate();
                }
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize = base.GetPreferredSize(proposedSize);
            if (showSplit && !string.IsNullOrEmpty(Text) && TextRenderer.MeasureText(Text, Font).Width + PushButtonWidth > preferredSize.Width)
            {
                return preferredSize + new Size(PushButtonWidth + BorderSize * 2, 0);
            }
            return preferredSize;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData.Equals(Keys.Down) && showSplit)
            {
                return true;
            }
            else
            {
                return base.IsInputKey(keyData);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (!showSplit)
            {
                base.OnGotFocus(e);
                return;
            }

            if (!State.Equals(PushButtonState.Pressed) && !State.Equals(PushButtonState.Disabled))
            {
                State = PushButtonState.Default;
            }
        }

        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (showSplit)
            {
                if (kevent.KeyCode.Equals(Keys.Down))
                {
                    ShowContextMenuStrip();
                }
                else if (kevent.KeyCode.Equals(Keys.Space) && kevent.Modifiers == Keys.None)
                {
                    State = PushButtonState.Pressed;
                }
            }
            base.OnKeyDown(kevent);
        }

        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            if (kevent.KeyCode.Equals(Keys.Space))
            {
                if (Control.MouseButtons == MouseButtons.None)
                {
                    State = PushButtonState.Normal;
                }
            }
            base.OnKeyUp(kevent);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (!showSplit)
            {
                base.OnLostFocus(e);
                return;
            }
            if (!State.Equals(PushButtonState.Pressed) && !State.Equals(PushButtonState.Disabled))
            {
                State = PushButtonState.Normal;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!showSplit)
            {
                base.OnMouseDown(e);
                return;
            }

            if (dropDownRectangle.Contains(e.Location))
            {
                ShowContextMenuStrip();
            }
            else if (UpRectangle.Contains(e.Location))
            {
                if (UpClickMouseDown != null)
                    UpClickMouseDown(this, e);
            }
            else if (DownRectangle.Contains(e.Location))
            {
                if (DownClickMouseDown != null)
                    DownClickMouseDown(this, e);
            }
            else
            {
                State = PushButtonState.Pressed;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (!State.Equals(PushButtonState.Pressed) && !State.Equals(PushButtonState.Disabled))
            {
                State = PushButtonState.Hot;
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!showSplit)
            {
                base.OnMouseLeave(e);
                return;
            }

            if (!State.Equals(PushButtonState.Pressed) && !State.Equals(PushButtonState.Disabled))
            {
                if (Focused)
                {
                    State = PushButtonState.Default;
                }
                else
                {
                    State = PushButtonState.Normal;
                }
            }
            if (UpClickMouseUp != null) UpClickMouseUp(this, e);
            if (DownClickMouseUp != null) DownClickMouseUp(this, e);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (!showSplit)
            {
                base.OnMouseUp(mevent);
                return;
            }
            
            if (ContextMenuStrip == null || !ContextMenuStrip.Visible)
            {
                SetButtonDrawState();
                if (Bounds.Contains(Parent.PointToClient(Cursor.Position)) && !dropDownRectangle.Contains(mevent.Location) && !DownRectangle.Contains(mevent.Location) && !UpRectangle.Contains(mevent.Location))
                {
                    OnClick(new EventArgs());
                }
                if (UpRectangle.Contains(mevent.Location) && UpClickMouseUp != null) UpClickMouseUp(this, mevent);
                if (DownRectangle.Contains(mevent.Location) && DownClickMouseUp != null) DownClickMouseUp(this, mevent);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (!showSplit)
            {
                return;
            }

            Graphics g = pevent.Graphics;
            Rectangle bounds = this.ClientRectangle;

            // draw the button background as according to the current state.
            if (State != PushButtonState.Pressed && IsDefault && !Application.RenderWithVisualStyles)
            {
                Rectangle backgroundBounds = bounds;
                backgroundBounds.Inflate(-1, -1);
                ButtonRenderer.DrawButton(g, backgroundBounds, State);

                // button renderer doesnt draw the black frame when themes are off =(
                g.DrawRectangle(SystemPens.WindowFrame, 0, 0, bounds.Width - 1, bounds.Height - 1);
            }
            else
            {
                ButtonRenderer.DrawButton(g, bounds, State);
            }
            // calculate the current dropdown rectangle.
            dropDownRectangle = new Rectangle(bounds.Right - PushButtonWidth - 1, BorderSize, PushButtonWidth, bounds.Height - BorderSize * 2);
            UpRectangle = new Rectangle(BorderSize, BorderSize, bounds.Width - BorderSize * 2, PushButtonWidth);
            DownRectangle = new Rectangle(BorderSize, bounds.Height - PushButtonWidth - BorderSize, bounds.Width - BorderSize * 2, PushButtonWidth);

            Rectangle focusRect =
                new Rectangle(BorderSize,
                              PushButtonWidth,
                              bounds.Width - dropDownRectangle.Width - BorderSize,
                              bounds.Height - (PushButtonWidth * 2));

            bool drawSplitLine = (State == PushButtonState.Hot || State == PushButtonState.Pressed || !Application.RenderWithVisualStyles);

            if (RightToLeft == RightToLeft.Yes)
            {
                dropDownRectangle.X = bounds.Left + 1;
                focusRect.X = dropDownRectangle.Right;
                if (drawSplitLine)
                {
                    // draw two lines at the edge of the dropdown button
                    g.DrawLine(SystemPens.ButtonShadow, bounds.Left + PushButtonWidth, BorderSize, bounds.Left + PushButtonWidth, bounds.Bottom - BorderSize);
                    g.DrawLine(SystemPens.ButtonFace, bounds.Left + PushButtonWidth + 1, BorderSize, bounds.Left + PushButtonWidth + 1, bounds.Bottom - BorderSize);
                }
            }
            else
            {
                if (drawSplitLine)
                {
                    // draw two lines at the right edge of the dropdown button
                    g.DrawLine(SystemPens.ButtonShadow, bounds.Right - PushButtonWidth, BorderSize, bounds.Right - PushButtonWidth, bounds.Bottom - BorderSize);
                    g.DrawLine(SystemPens.ButtonFace, bounds.Right - PushButtonWidth - 1, BorderSize, bounds.Right - PushButtonWidth - 1, bounds.Bottom - BorderSize);
                    // draw two lines at the up edge of the dropdown button
                    g.DrawLine(SystemPens.ButtonShadow, BorderSize-1, PushButtonWidth, bounds.Right - PushButtonWidth, PushButtonWidth);
                    g.DrawLine(SystemPens.ButtonFace, BorderSize-1, PushButtonWidth - 1, bounds.Right - PushButtonWidth, PushButtonWidth - 1);
                    // draw two lines at the bottom edge of the dropdown button
                    g.DrawLine(SystemPens.ButtonShadow, BorderSize - 1, bounds.Bottom - PushButtonWidth, bounds.Right - PushButtonWidth, bounds.Bottom - PushButtonWidth);
                    g.DrawLine(SystemPens.ButtonFace, BorderSize - 1, bounds.Bottom - PushButtonWidth - 1, bounds.Right - PushButtonWidth, bounds.Bottom - PushButtonWidth - 1);
                }
            }

            // Draw an arrow in the correct location
            PaintArrow(g, dropDownRectangle,false,false);
            PaintArrow(g, UpRectangle,true,true);
            PaintArrow(g, DownRectangle,true,false);
            
            // Figure out how to draw the text
            TextFormatFlags formatFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
            
            
            // If we dont' use mnemonic, set formatFlag to NoPrefix as this will show ampersand.
            if (!UseMnemonic)
            {
                formatFlags = formatFlags | TextFormatFlags.NoPrefix;
            }
            else if (!ShowKeyboardCues)
            {
                formatFlags = formatFlags | TextFormatFlags.HidePrefix;
            }

            if (!string.IsNullOrEmpty(this.Text))
            {
                if (Enabled)
                    TextRenderer.DrawText(g, Text, Font, focusRect, SystemColors.ControlText, formatFlags);
                else
                    TextRenderer.DrawText(g, Text, Font, focusRect, SystemColors.GrayText, formatFlags);
            }

            if (Image != null)
            {
                if (Enabled)
                    g.DrawImage(Image, focusRect);
                else
                    g.DrawImage(MakeGrayscale(Image), focusRect);

            }
            
            // draw the focus rectangle.

            if (State != PushButtonState.Pressed && Focused)
            {
                ControlPaint.DrawFocusRectangle(g, focusRect);
            }
        }

        Image MakeGrayscale(Image original)
        {
            //create a blank bitmap the same size as original
            Image newBitmap =
               new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(new float[][] { 
                new float[] { .3f, .3f, .3f, 0, 0 }, 
                new float[] { .59f, .59f, .59f, 0, 0 }, 
                new float[] { .11f, .11f, .11f, 0, 0 }, 
                new float[] { 0, 0, 0, 1, 0 }, 
                new float[] { 0, 0, 0, 0, 1 } });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original,
               new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height,
               GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        private static void PaintArrow(Graphics g, Rectangle Rect,bool Horiz,bool Up)
        {
            Point middle = new Point((Rect.Left + Rect.Width / 2),(Rect.Top + Rect.Height / 2));

            //if the width is odd - favor pushing it over one pixel right.
             middle.X += (Rect.Width % 2);
             if (Horiz)
                 middle.X -= PushButtonWidth / 2;
            Point[] arrow;
            if (Up)//Up Arrow
                arrow = new Point[] { new Point(middle.X - 3, middle.Y + 1), new Point(middle.X + 3, middle.Y + 1), new Point(middle.X, middle.Y - 4) };
            else//Down Arrow
                arrow = new Point[] { new Point(middle.X - 2, middle.Y - 1), new Point(middle.X + 3, middle.Y - 1), new Point(middle.X, middle.Y + 4) };
            
            g.FillPolygon(Brushes.Green, arrow);
        }

        private void ShowContextMenuStrip()
        {
            if (skipNextOpen)
            {
                // we were called because we're closing the context menu strip
                // when clicking the dropdown button.
                skipNextOpen = false;
                return;
            }
            State = PushButtonState.Pressed;

            if (ContextMenuStrip != null)
            {
                ContextMenuStrip.Closing += new ToolStripDropDownClosingEventHandler(ContextMenuStrip_Closing);
                ContextMenuStrip.Show(this, new Point(0, Height), ToolStripDropDownDirection.Default);
            }
        }

        void ContextMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            ContextMenuStrip cms = sender as ContextMenuStrip;
            if (cms != null)
            {
                cms.Closing -= new ToolStripDropDownClosingEventHandler(ContextMenuStrip_Closing);
            }

            SetButtonDrawState();

            if (e.CloseReason == ToolStripDropDownCloseReason.AppClicked)
            {
                Point pt = this.PointToClient(Cursor.Position);
                skipNextOpen = (dropDownRectangle.Contains(pt)||UpRectangle.Contains(pt)||DownRectangle.Contains(pt));
            }
        }


        private void SetButtonDrawState()
        {
            if (Bounds.Contains(Parent.PointToClient(Cursor.Position)))
                State = PushButtonState.Hot;
            else if (Focused)
                State = PushButtonState.Default;
            else
                State = PushButtonState.Normal;
        }
    }
}

