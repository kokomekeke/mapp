/*=============================================================================
*  MapTextBox: text box on map
*
* (c) Kótai 2026.
*
* ver 1.00                                                         04-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;

namespace SgxMap.Objects
{
    public class MapTextBox: MapLabel, ISerializable, IDisposable
    {
        [NonSerialized] public Brush BorderColor = new SolidBrush(Color.Black);  // border color 
        //[NonSerialized] public Brush FillColor = new SolidBrush(Color.White);    // background color

        public Size TextPadding = new Size(3, 3);

        public MapTextBox()
        {
        }

        public MapTextBox(PointLatLng pointLatLng, string text)
            : base(pointLatLng, text)
        {
            FillColor = Color.White;
        }

        public MapTextBox(string name, PointLatLng pointLatLng, string text, Font font)
            : base(name, pointLatLng, text, font)
        {
            FillColor = Color.White;
        }


        //public override void BuildGraphicsPath()
        //{
        //    if (string.IsNullOrEmpty(Text))
        //        return;

        //    var st = g.MeasureString(Text, Font).ToSize();

        //    var rect = new Rectangle(LocalPosition.X,
        //        LocalPosition.Y - st.Height,
        //        st.Width + 2 * TextPadding.Width,
        //        st.Height + TextPadding.Height);

        //    var rectText = rect;
        //    rectText.Offset(TextPadding.Width, TextPadding.Height); //Inflate(-TextPadding.Width, -TextPadding.Height);

        //    //m_graphicsPath.FillRectangle(FillColor, rect);
        //    m_graphicsPath?.AddRectangle(rect);
        //    if (!string.IsNullOrEmpty(_text))
        //        m_graphicsPath?.AddString(_text, Font.FontFamily, (int)Font.Style, Font.Size, rectText, StringFormat.GenericDefault);

        //} /* UpdateGraphicsPath */


        public override void OnRender(Graphics g)
        {
            if (string.IsNullOrEmpty(Text))
                return;

            var st = g.MeasureString(Text, Font).ToSize();

            var rect = new Rectangle(LocalPosition.X,
                LocalPosition.Y - st.Height,
                st.Width + 2 * TextPadding.Width,
                st.Height + TextPadding.Height);

            var rectText = rect;
            rectText.Offset(TextPadding.Width, TextPadding.Height); //Inflate(-TextPadding.Width, -TextPadding.Height);

            g.FillRectangle(Fill, rect);
            g.DrawRectangle(Stroke, rect);
            g.DrawString(Text, Font, TextColor, rectText);
            g.Flush();
        }


        #region serialization

        protected MapTextBox(SerializationInfo info, StreamingContext context)
        {
            ID        = info.GetInt64("ID");
            Name      = info.GetString("Name") ?? string.Empty;
            Position  = (PointLatLng)info.GetValue("Position", typeof(PointLatLng));
            Color     = (Color)info.GetValue("Color", typeof(Color));
            _text     = info.GetString("text");
            Font      = (Font)info.GetValue("font", typeof(Font));
            TextColor = new SolidBrush(Color);
            BorderColor = (SolidBrush)info.GetValue("bordercolor", typeof(Brush)); 
            //FillColor = (SolidBrush)info.GetValue("fillcolor", typeof(Brush)); 
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("text", Text);
            info.AddValue("font", Font);
            info.AddValue("bordercolor", BorderColor);
            //info.AddValue("fillcolor", FillColor);
        }

        public override void OnDeserialization(object? sender)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            TextColor.Dispose();
            Font.Dispose();
        }

        #endregion serialization


    } /* class GMapTextBox */

} /* namespace SgxMap.Objects */
