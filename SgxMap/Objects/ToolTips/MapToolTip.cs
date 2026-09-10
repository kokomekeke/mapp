using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;

namespace SgxMap.Objects.ToolTips
{
    public class MapToolTip: BasePoint, ISerializable
    {
        protected readonly MapBaseMarker m_marker;
        protected Point Offset;
        protected Size TextPadding = new Size(20, 21);  // text padding

        protected readonly StringFormat m_Format = new StringFormat();

        protected Font TextFont = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Regular, GraphicsUnit.Pixel);
        protected Font TitleFont = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold, GraphicsUnit.Pixel);
        protected SolidBrush TitleFill = new SolidBrush(Color.LightGray);  // title text foreground  (use Fill for text background)


        public MapBaseMarker Marker
        {
            get
            {
                return m_marker;
            }
        }

        
        public MapToolTip()            
            : base()
        {
            Stroke.Width = 1;
            Stroke.LineJoin = LineJoin.Round;
            Stroke.StartCap = LineCap.RoundAnchor;

            m_Format.LineAlignment = StringAlignment.Near;
            m_Format.Alignment = StringAlignment.Near;
        }

        public MapToolTip(MapBaseMarker marker)
            : this()
        {
            m_marker = marker;
            Offset = new Point(14, -34);
        }

        public override void OnRender(Graphics g)
        {
            if (string.IsNullOrEmpty(Marker.ToolTipText))
                return;

            var st = g.MeasureString(Marker.ToolTipText, TextFont).ToSize();
            RectangleF rect = new Rectangle(Marker.ToolTipPosition.X,
                Marker.ToolTipPosition.Y - st.Height,
                st.Width + TextPadding.Width,
                st.Height + TextPadding.Height);
            RectangleF rectText = new Rectangle(Marker.ToolTipPosition.X,
                Marker.ToolTipPosition.Y - st.Height,
                st.Width + TextPadding.Width,
                st.Height + TextPadding.Height);
            rect.Offset(Offset.X, Offset.Y);
            rectText.Offset(Offset.X + 7, Offset.Y + 7);
            g.DrawLine(Stroke, Marker.ToolTipPosition.X, Marker.ToolTipPosition.Y, rect.X, rect.Y + rect.Height / 2);
            g.DrawLine(Stroke, Marker.ToolTipPosition.X, Marker.ToolTipPosition.Y, rect.X, rect.Y + rect.Height);
            g.FillRectangle(Fill, rect);
            DrawRoundRectangle(g, Stroke, rect.X, rect.Y, rect.Width, rect.Height, 8);
            //g.DrawString(Marker.ToolTipText, Font, Foreground, rectText, Format);
            WriteString(g, Marker.ToolTipText, rectText);
            g.Flush();
        }

        public void DrawRoundRectangle(Graphics g, Pen pen, float h, float v, float width, float height, float radius)
        {
            using (var gp = new GraphicsPath())
            {
                gp.AddLine(h + radius, v, h + width - radius * 2, v);
                gp.AddArc(h + width - radius * 2, v, radius * 2, radius * 2, 270, 90);
                gp.AddLine(h + width, v + radius, h + width, v + height - radius * 2);
                gp.AddArc(h + width - radius * 2, v + height - radius * 2, radius * 2, radius * 2, 0, 90); // Corner
                gp.AddLine(h + width - radius * 2, v + height, h + radius, v + height);
                gp.AddArc(h, v + height - radius * 2, radius * 2, radius * 2, 90, 90);
                gp.AddLine(h, v + height - radius * 2, h, v + radius);
                gp.AddArc(h, v, radius * 2, radius * 2, 180, 90);

                gp.CloseFigure();

                g.FillPath(Fill, gp);
                g.DrawPath(pen, gp);
            }
        }

        public void DrawRectangle(Graphics g, Pen pen, float h, float v, float width, float height)
        {
            g.FillRectangle(Fill, h, v, width, height);
            g.DrawRectangle(pen, h, v, width, height);
        }


        protected virtual void WriteString(Graphics g, string text, RectangleF rectText)
        {
            string[] vec1 = text.Split('\n');
            for (int i = 0; i < vec1.Length; i++)
            {
                if (vec1[i] != "")
                {
                    string[] vec2 = vec1[i].Split('|');
                    if (vec2.Length > 0)
                    {
                        var st = g.MeasureString(vec2[0], TitleFont).ToSize();
                        g.DrawString(string.Format("{0}", vec2[0]), TitleFont, TitleFill, rectText, m_Format);
                        rectText.X += st.Width + 2;
                        if (vec2.Length > 1)
                        {
                            g.DrawString(vec2[1], TextFont, Fill, rectText, m_Format);
                        }

                        rectText.X -= st.Width + 2;
                        rectText.Y += st.Height;
                    }
                }
                else
                {
                    var st = g.MeasureString("\n", TitleFont).ToSize();
                    rectText.Y += st.Height;
                }
            }
        }


        #region ISerializable Members

        /// <summary>
        ///     Initializes a new instance of the <see cref="GMapToolTip" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected MapToolTip(SerializationInfo info, StreamingContext context)
        {
            Offset = Extensions.GetStruct(info, "Offset", Point.Empty);
            TextPadding = Extensions.GetStruct(info, "TextPadding", new Size(10, 10));
            Stroke = new Pen(Color.FromArgb(info.GetInt32("Stroke")));
            Fill = new SolidBrush(Color.FromArgb(info.GetInt32("Fill")));
            this.TextFont = (Font)Extensions.GetValue(info, "Font", TextFont);
        }

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">
        ///     The caller does not have the required permission.
        /// </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Offset", Offset);
            info.AddValue("TextPadding", TextPadding);
            info.AddValue("TitleFill", Fill.Color.ToArgb());
            info.AddValue("Font", TextFont);
            info.AddValue("TitleFont", TitleFont);
        }

        #endregion



    }
}
