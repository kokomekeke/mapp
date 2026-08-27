using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace GMap.NET.WindowsForms
{
    /// <summary>
    ///     GMap.NET marker
    /// </summary>
    [Serializable]
    public class GMapDFToolTip : ISerializable, IDisposable
    {
        GMapDFLine _dfline;

        public GMapDFLine DFLine
        {
            get
            {
                return _dfline;
            }
            internal set
            {
                _dfline = value;
            }
        }

        public Point Offset;

        public static readonly StringFormat DefaultFormat = new StringFormat();

        /// <summary>
        ///     string format
        /// </summary>
        [NonSerialized] public readonly StringFormat Format = DefaultFormat;

        public static readonly Font DefaultFont =
            new Font(FontFamily.GenericSansSerif, 11, FontStyle.Regular, GraphicsUnit.Pixel);

        public static readonly Font TitleFont =
            new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold, GraphicsUnit.Pixel);

        /// <summary>
        ///     font
        /// </summary>
        [NonSerialized] public Font Font = DefaultFont;

        public static readonly Pen DefaultStroke = new Pen(Color.FromArgb(140, Color.Black));

        /// <summary>
        ///     specifies how the outline is painted
        /// </summary>
        [NonSerialized] public Pen Stroke = DefaultStroke;

        public static readonly Brush DefaultFill = new SolidBrush(Color.FromArgb(222, Color.White));

        /// <summary>
        ///     background color
        /// </summary>
        [NonSerialized] public Brush Fill = DefaultFill;

        public static readonly Brush DefaultForeground = new SolidBrush(Color.DimGray);

        /// <summary>
        ///     text foreground
        /// </summary>
        [NonSerialized] public Brush Foreground = DefaultForeground;

        /// <summary>
        ///     text padding
        /// </summary>
        public Size TextPadding = new Size(20, 21);


        Object _parent = null;
        public Object Parent
        {
            get { return _parent; }
            protected set { _parent = value; }
        }


        GMapOverlay _overlay;

        public GMapOverlay Overlay
        {
            get
            {
                return _overlay;
            }
            internal set
            {
                _overlay = value;
            }
        }


        static GMapDFToolTip()
        {
            DefaultStroke.Width = 1;

            DefaultStroke.LineJoin = LineJoin.Round;
            DefaultStroke.StartCap = LineCap.RoundAnchor;

            DefaultFormat.LineAlignment = StringAlignment.Near;
            DefaultFormat.Alignment = StringAlignment.Near;
        }

        public GMapDFToolTip(GMapDFLine dfline)
        {
            DFLine = dfline;
            Offset = new Point(14, -44);
        }

        public virtual void OnRender(Graphics g)
        {
            var st = g.MeasureString(DFLine.Name, Font).ToSize();
            RectangleF rect = new Rectangle(DFLine.ToolTipPosition.X,
                DFLine.ToolTipPosition.Y - st.Height,
                st.Width + TextPadding.Width,
                st.Height + TextPadding.Height);
            RectangleF rectText = new Rectangle(DFLine.ToolTipPosition.X,
                DFLine.ToolTipPosition.Y - st.Height,
                st.Width + TextPadding.Width,
                st.Height + TextPadding.Height);
            rect.Offset(Offset.X, Offset.Y);
            rectText.Offset(Offset.X + 7, Offset.Y + 7);
            g.DrawLine(Stroke, DFLine.ToolTipPosition.X, DFLine.ToolTipPosition.Y, rect.X, rect.Y + rect.Height / 2);
            g.FillRectangle(Fill, rect);
            //g.DrawRectangle(Stroke, rect);

            DrawRoundRectangle(g, Stroke, rect.X, rect.Y, rect.Width, rect.Height, 8);
            //g.DrawString(DFLine.ToolTipText, Font, Foreground, rectText, Format);
            WriteString(g, DFLine.Name, rectText);
            g.Flush();
        }

        #region ISerializable Members

        /// <summary>
        ///     Initializes a new instance of the <see cref="GMapDFToolTip" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected GMapDFToolTip(SerializationInfo info, StreamingContext context)
        {
            Offset = Extensions.GetStruct(info, "Offset", Point.Empty);
            TextPadding = Extensions.GetStruct(info, "TextPadding", new Size(10, 10));
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
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Offset", Offset);
            info.AddValue("TextPadding", TextPadding);
        }

        #endregion

        #region IDisposable Members

        bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        #endregion

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
                        g.DrawString(String.Format("{0}", vec2[0]), TitleFont, Foreground, rectText, Format);
                        rectText.X += st.Width + 2;
                        if (vec2.Length > 1)
                        {
                            g.DrawString(vec2[1], Font, Foreground, rectText, Format);
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
    }
}
