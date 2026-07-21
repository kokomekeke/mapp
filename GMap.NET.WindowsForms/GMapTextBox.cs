using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GMap.NET.WindowsForms.ToolTips
{
    public class GMapTextBox : GMapLabel, ISerializable
    {

        public static readonly Brush DefaultBorder = new SolidBrush(Color.Black); //FromArgb(155, Color.Black));

        /// <summary>
        ///     border color
        /// </summary>
        [NonSerialized] public Brush BorderColor = DefaultBorder;


        public static readonly Brush DefaultFill = new SolidBrush(Color.White); //FromArgb(155, Color.White));

        /// <summary>
        ///     background color
        /// </summary>
        [NonSerialized] public Brush FillColor = DefaultFill;


        public static readonly Pen DefaultStroke = new Pen(Color.FromArgb(155, Color.Black));

        /// <summary>
        ///     specifies how the outline is painted 
        /// </summary>
        [NonSerialized] public Pen Stroke = DefaultStroke;



        public Size TextPadding = new Size(3, 3);


        public GMapTextBox()
        {
        }

        public GMapTextBox(PointLatLng pointLatLng, string text)
            : base(pointLatLng, text) 
        {

        }

        public GMapTextBox(PointLatLng pointLatLng, string text, Font font)
            : base (pointLatLng, text, font) 
        {
        }


        public override void OnRender(Graphics g)
        {
            if (string.IsNullOrEmpty(Text))
                return;

            var st = g.MeasureString(Text, Font).ToSize();

            var rect = new Rectangle(LocalPosition.X,
                LocalPosition.Y - st.Height,
                st.Width + 2*TextPadding.Width,
                st.Height + TextPadding.Height);

            var rectText = rect;
            rectText.Offset(TextPadding.Width, TextPadding.Height); //Inflate(-TextPadding.Width, -TextPadding.Height);

            g.FillRectangle(FillColor, rect);
            g.DrawRectangle(Stroke, rect);
            g.DrawString(Text, Font, TextColor, rectText);
            g.Flush();
        }

    } /* class GMapTextBox */

} /* namespace GMap.NET.WindowsForms.ToolTips */
