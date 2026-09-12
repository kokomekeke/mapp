/*=============================================================================
*  MapTextBox: text box on map
*
* (c) Kótai 2026.
*
* ver 1.00                                                         04-Sep-2026.
*           Last update                                            12-Sep-2026.
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
    public class MapTextBox: MapRectangle, ISerializable, IDisposable
    {
        [NonSerialized] public Brush BorderColor = new SolidBrush(Color.Black);  // border color 
        public Size TextPadding = new Size(3, 3);
        public Brush TextColor = new SolidBrush(Color.Black);
        protected string? m_text;
        public Font Font = new Font("Arial", 12);

        public int mouse_x = 0;
        public int mouse_y = 0;


        public string? Text
        {
            get { return m_text; }
            set
            {
                m_text = value;

                if (Layer != null && Layer.MapPanel != null)
                {
                    m_layer?.RefreshObject(this);
                }
            }
        }

        //------------------------------------------------------------------------------------------------

        public MapTextBox()
        {
            m_isfilled = true;
        }

        public MapTextBox(PointLatLng center, float width, float height, string text) 
            : base(center, width, height, Color.Black )
        {
            FillColor = Color.White;
            Text = text;
            m_isfilled = true;
        }

        public MapTextBox(string name, PointLatLng center, float width, float height, string text)
            : base(center, width, height, Color.Black)
        {
            Name = name;
            FillColor = Color.White;
            Text = text;
            m_isfilled = true;
        }

        public MapTextBox(string name, PointLatLng center, float width, float height, string text, Font font)
            : base(center, width, height, Color.Black)
        {
            Name = name;
            FillColor = Color.White;
            Font = font;
            Text = text;
            m_isfilled = true;
        }

        public MapTextBox(PointLatLng center, string text)
            : base(center, 10, 10, Color.Black)
        {
            FillColor = Color.White;
            Text = text;
            m_isfilled = true;
        }


        public MapTextBox(string name, PointLatLng topleft, PointLatLng bottomright, string text, Font font)
            : base(topleft, bottomright, Color.Black)
        {
            Name = name;
            FillColor = Color.White;
            Font = font;
            Text = text;
            m_isfilled = true;
        }


        public override bool IsInside(int x, int y)
        {
            double x1 = LocalPosition.X;
            double y1 = LocalPosition.Y;
            double x2 = LocalPosition2.X;
            double y2 = LocalPosition2.Y;

            return x >= x1 && x <= x2 && y >= y1 && y <= y2;
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
            if (!string.IsNullOrEmpty(Text))
            {

                var st = g.MeasureString(Text, Font).ToSize();

                var rect = new Rectangle(LocalPosition.X,
                    LocalPosition.Y,// - st.Height,
                    st.Width + 2 * TextPadding.Width,
                    st.Height + TextPadding.Height);

                var rectText = rect;
                rectText.Offset(TextPadding.Width, TextPadding.Height); //Inflate(-TextPadding.Width, -TextPadding.Height);

                //LocalPosition2 = LocalPosition + new Size(rectText.Width, rectText.Height);
                //Position2 = Layer.MapPanel.FromLocalToLatLng(LocalPosition2.X, LocalPosition2.Y);
                //BuildPoints();

                g.FillRectangle(Fill, rect);
                g.DrawRectangle(Stroke, rect);
                g.DrawString(Text, Font, TextColor, rectText);
                g.Flush();
            }
            else
            {
                base.OnRender(g);
            }
        }


        public override void Select(bool select)
        {
            if (select)
            {
                if (m_isSelected)
                    return;
                Layer.MapPanel.ClearLinePoints();
                Stroke.Width = 4;
            }
            else
            {
                Stroke.Width = 1;
                Layer.MapPanel.ClearLinePoints(this);
            }

            m_isSelected = select;
            m_layer?.RefreshObject(this);
        }



        #region serialization

        protected MapTextBox(SerializationInfo info, StreamingContext context)
        {
            ID        = info.GetInt64("ID");
            Name      = info.GetString("Name") ?? string.Empty;
            Position  = (PointLatLng)info.GetValue("Position", typeof(PointLatLng));
            Color     = (Color)info.GetValue("Color", typeof(Color));
            Text     = info.GetString("text");
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
