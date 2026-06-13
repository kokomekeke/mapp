using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using GMap.NET.WindowsForms.Markers;

namespace GMap.NET.WindowsForms.ToolTips
{
    /// <summary>
    ///     GMap.NET marker
    /// </summary>
    [Serializable]
    public class GMapHFBaloonToolTip : GMapToolTip, ISerializable
    {
        //public static readonly Pen DefaultStroke = new Pen(Color.FromArgb(140, Color.Navy));
        [NonSerialized] public Brush HeaderFill = new SolidBrush(Color.LightGray);

        static GMapHFBaloonToolTip()
        {
            DefaultStroke.Width = 3;

            DefaultStroke.LineJoin = LineJoin.Round;
            DefaultStroke.StartCap = LineCap.RoundAnchor;
        }

        public GMapHFBaloonToolTip(GMapMarker marker)
            : base(marker)
        {
            Stroke = new Pen(Color.Black);
            Fill = Brushes.White;
            TextPadding = new Size(3, 3);
            Foreground = new SolidBrush(Color.Black);
        }

        public override void OnRender(Graphics g)
        {
            var st = g.MeasureString(Marker.ToolTipText, Font).ToSize();
            var rect = new Rectangle(Marker.ToolTipPosition.X,
                Marker.ToolTipPosition.Y - st.Height,
                st.Width + TextPadding.Width,
                st.Height + TextPadding.Height);
            rect.Offset(Offset.X, Offset.Y);

            int lineOffset = 0;
            if (!g.VisibleClipBounds.Contains(rect))
            {
                var clippingOffset = new Point();
                if (rect.Right > g.VisibleClipBounds.Right)
                {
                    clippingOffset.X = -((rect.Left - Marker.LocalPosition.X) / 2 + rect.Width);
                    lineOffset = -rect.Width;
                }

                if (rect.Top < g.VisibleClipBounds.Top)
                {
                    clippingOffset.Y = ((rect.Bottom - Marker.LocalPosition.Y) + (rect.Height * 2));
                }

                rect.Offset(clippingOffset);
            }

            g.DrawLine(Stroke,
                Marker.ToolTipPosition.X,
                Marker.ToolTipPosition.Y,
                (rect.X - lineOffset) ,
                rect.Y + rect.Height) ;

            g.FillRectangle(Fill, rect.X, rect.Y, rect.Width, rect.Height);

            st = g.MeasureString("Wg", Font).ToSize();
            g.FillRectangle(HeaderFill, rect.X, rect.Y, rect.Width, st.Height);
            g.DrawRectangle(Stroke, rect.X, rect.Y, rect.Width, rect.Height);

            if (Format.Alignment == StringAlignment.Near)
                rect.Offset(TextPadding.Width/2, 0);
            rect.Offset(0, TextPadding.Height / 2);

            //g.DrawString(Marker.ToolTipText, Font, Foreground, rect, Format);
            WriteString(g, Marker.ToolTipText, rect);
        }


        protected override void WriteString(Graphics g, string text, RectangleF rectText)
        {
            SizeF st;
            string[] vec1 = text.Split('\n');
            for (int i = 0; i < vec1.Length; i++)
            {
                if (vec1[i] != "")
                {
                    if (i == 0)
                    {
                        st = g.MeasureString(vec1[0], TitleFont).ToSize();
                        g.DrawString(vec1[0], TitleFont, Foreground, rectText, Format);
                    }
                    else
                    {
                        st = g.MeasureString(vec1[i], Font).ToSize();
                        g.DrawString(vec1[i], Font, Foreground, rectText, Format);
                    }

                    rectText.Y += st.Height;
                }
                else
                {
                    st = g.MeasureString("\n", TitleFont).ToSize();
                    rectText.Y += st.Height;
                }
            }
        }


        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected GMapHFBaloonToolTip(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
