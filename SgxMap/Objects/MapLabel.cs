/*=============================================================================
*  Nap lable: label on map
*
* (c) Kótai 2026.
*
* ver 1.00                                                         03-Sep-2026.
*          Last modification                                       04-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;

namespace SgxMap.Objects
{
    [Serializable]
    public class MapLabel : BasePoint, ISerializable, IDeserializationCallback, IDisposable
    {

        /// <summary>
        ///     foreground color
        /// </summary>
        public Brush TextColor = new SolidBrush(Color.Black);

        public Size Size { get; set; }

        public static readonly Font DefaultFont = new Font("Arial", 12);
        [NonSerialized] public Font Font = DefaultFont;


        protected string? _text;
        public string? Text
        {   
            get { return _text; }
            set
            {
                _text = value;
                Size = TextRenderer.MeasureText(_text, Font);

                if (Layer != null && Layer.MapPanel != null)
                {
                    m_layer?.RefreshObjects();
                }

            }
        }

        public MapLabel() :
            base()
        {
        }

        public MapLabel(PointLatLng pointLatLng, string text)
            : base (pointLatLng, Color.Black)
        {
            Text = text;
        }

        public MapLabel(string name, PointLatLng pointLatLng, string text)
            : base(name, pointLatLng, Color.Black)
        {
            Text = text;
        }

        public MapLabel(string name, PointLatLng pointLatLng, string text, Color color)
            : base(name, pointLatLng, color)
        {
            Text = text;
            TextColor = new SolidBrush(color);
        }



        public MapLabel(string name, PointLatLng pointLatLng, string text, Font font)
            : base(name, pointLatLng, Color.Black)
        {
            Font = font;
            Text = text;
            TextColor = new SolidBrush(Color.Black);
        }

        public static MapLabel? CreateLabel(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "Label", 1: name,  2: lat1, 3: lng1, 4: color, 5: text

                errorstring = string.Empty;

                if (ls.Length < 6)
                {
                    errorstring = "Not enough parameters to create a point.";
                    return null;
                }

                string name = ls[1];
                var position = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                var color = ColorTranslator.FromHtml(ls[4]);
                var text = ls[5];
                return new MapLabel(name, position, text, color);
            }
            catch (Exception ex)
            {
                errorstring = $"Error creating label: {ex.Message}";
                return null;
            }
        } /* CreateLabel */

        public static MapLabel? CreateLabelExt(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "label", 1:name,  2: "lat", 3: "lng", 4: "text", 5: "fontname", 6: "fontsize", 7: "fontstyle"
                errorstring = string.Empty;
                string name = ls[1];
                var pointLatLng = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                string text = ls[4];
                string fontName = ls[5];
                float fontSize = float.Parse(ls[6]);
                var fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), ls[7]);

                return new MapLabel(name, pointLatLng, text, new Font(fontName, fontSize, fontStyle));
            }
            catch (Exception ex)
            {
                errorstring = $"Error creating label: {ex.Message}";
                return null;
            }
        }

        public override void BuildGraphicsPath()
        {
            base.BuildGraphicsPath();

            if (!string.IsNullOrEmpty(_text))
                m_graphicsPath?.AddString(_text, Font.FontFamily, (int)Font.Style, Font.Size, LocalPosition, StringFormat.GenericDefault);
        } /* UpdateGraphicsPath */


        #region serialization

        protected MapLabel(SerializationInfo info, StreamingContext context)
        {
            ID        = info.GetInt64("ID");
            Name      = info.GetString("Name") ?? string.Empty;
            Position  = (PointLatLng)info.GetValue("Position", typeof(PointLatLng));
            Color     = (Color)info.GetValue("Color", typeof(Color));
            _text     = info.GetString("text");
            Font      = (Font)info.GetValue("font", typeof(Font));
            TextColor = new SolidBrush(Color);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("text", Text);
            info.AddValue("font", Font);
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


    } /* class MapLabel */
} /* namespace SgxMap.Objects */
