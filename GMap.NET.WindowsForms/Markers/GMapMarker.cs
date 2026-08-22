using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Windows.Forms;
using Demo.WindowsForms.CustomMarkers;
using GMap.NET.WindowsForms.ToolTips;

namespace GMap.NET.WindowsForms.Markers
{
    /// <summary>
    ///     GMap.NET marker
    /// </summary>
    [Serializable]
    public abstract class GMapMarker : ISerializable, IDisposable
    {

        public static readonly Pen DefaultStroke = new Pen(Brushes.Red, 2);

        /// <summary>
        /// specifies how the outline is painted
        /// </summary>
        [NonSerialized] public Pen Stroke = DefaultStroke;

        /// <summary>
        /// background color
        /// </summary>
        [NonSerialized]
        public SolidBrush Fill = new SolidBrush(Color.FromArgb(155, Color.AliceBlue));


        object _parent = null;
        public object Parent
        {
            get { return _parent; }
            protected set { _parent = value; }
        }

        public string Name { get; protected set; }  

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

        private PointLatLng _position;

        public PointLatLng Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position != value)
                {
                    var oldPosition = _position;
                    _position = value;

                    if (IsVisible)
                    {
                        if (Overlay != null && Overlay.Control != null)
                        {
                            Overlay.Control.UpdateMarkerLocalPosition(this);
                        }

                        if (Parent != null)
                        {
                            if (Parent is GMapPolyLine)
                            {
                                ((GMapPolyLine)Parent).MovePoint(oldPosition, _position);
                            }

                            if (Overlay != null && Overlay.Control != null)
                            {
                                if (!Overlay.Control.HoldInvalidation)
                                {
                                    Overlay.Control.Invalidate();
                                }
                            }
                        }
                    }
                }
            }
        }

        public object Tag;

        Point _offset;

        public Point Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                if (_offset != value)
                {
                    _offset = value;

                    if (IsVisible)
                    {
                        if (Overlay != null && Overlay.Control != null)
                        {
                            Overlay.Control.UpdateMarkerLocalPosition(this);
                        }
                    }
                }
            }
        }

        Rectangle _area;

        /// <summary>
        ///     marker position in local coordinates, internal only, do not set it manualy
        /// </summary>
        public Point LocalPosition
        {
            get
            {
                return _area.Location + new Size(_area.Size.Width / 2, _area.Size.Height / 2);
            }
            set
            {
                var p = value - new Size(_area.Size.Width / 2, _area.Size.Height / 2);

                if (_area.Location != p)
                {
                    _area.Location = p;
                    {
                        if (Overlay != null && Overlay.Control != null)
                        {
                            if (!Overlay.Control.HoldInvalidation)
                            {
                                Overlay.Control.Invalidate();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     ToolTip position in local coordinates
        /// </summary>
        public Point ToolTipPosition
        {
            get
            {
                var ret = _area.Location + new Size(_area.Size.Width / 2, _area.Size.Height / 2);
                ret.Offset(-Offset.X, -Offset.Y);
                return ret;
            }
        }

        public Size Size
        {
            get
            {
                return _area.Size;
            }
            set
            {
                _area.Size = value;
            }
        }

        public Rectangle LocalArea
        {
            get
            {
                return _area;
            }
        }

        public GMapToolTip ToolTip;

        public MarkerTooltipMode ToolTipMode = MarkerTooltipMode.OnMouseOver;

        string _toolTipText;

        public virtual string ToolTipText
        {
            get
            {
                return _toolTipText;
            }

            set
            {
                if (ToolTip == null && !string.IsNullOrEmpty(value))
                {
                    ToolTip = new GMapRoundedToolTip(this);
                }

                _toolTipText = value;
            }
        }

        private bool _visible = true;

        /// <summary>
        ///     is marker visible
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (value != _visible)
                {
                    _visible = value;

                    if (Overlay != null && Overlay.Control != null)
                    {
                        if (_visible)
                        {
                            Overlay.Control.UpdateMarkerLocalPosition(this);
                        }
                        else
                        {
                            if (Overlay.Control.IsMouseOverMarker)
                            {
                                Overlay.Control.IsMouseOverMarker = false;
                                Overlay.Control.RestoreCursorOnLeave();
                            }
                        }

                        {
                            if (!Overlay.Control.HoldInvalidation)
                            {
                                Overlay.Control.Invalidate();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     if true, marker will be rendered even if it's outside current view
        /// </summary>
        public bool DisableRegionCheck;

        /// <summary>
        ///     can maker receive input
        /// </summary>
        public bool IsHitTestVisible = true;

        private bool _isMouseOver;

        /// <summary>
        ///     is mouse over marker
        /// </summary>
        public bool IsMouseOver
        {
            get
            {
                return _isMouseOver;
            }
            internal set
            {
                _isMouseOver = value;
            }
        }

        protected bool _isselected;
        public bool IsSelected
        {
            get { return _isselected; }
            set
            {
                if (_isselected != value)
                {
                    _isselected = value;
                    if (Overlay != null && Overlay.Control != null)
                    {
                        if (!Overlay.Control.HoldInvalidation)
                        {
                            Overlay.Control.Invalidate();
                        }
                    }
                }
            }
        }

        public GMapMarker()
        {
        }

        public GMapMarker(PointLatLng pos, string name = null)
        {
            Position = pos;
            Name = name;
        }

        public virtual void CopyToClipboard()
        {
            System.Windows.Forms.Clipboard.SetText(Position.ToGeoString());
        }


        public virtual void ChangeColor(Color color)
        {
            Stroke = new Pen(color, Stroke.Width);
        }



        public static GMapMarker CreateMarker(string[] ls, out string errorstr)
        {
            // 0: marker, 1:type, 2:name, 3:lat, 4:long, 5:color
            string name = ls[2];
            if (name == ".") name = "";
            double lat = 0;
            double lng = 0;
            errorstr = null;

            if (!double.TryParse(ls[3], out lat))
            {
                errorstr = "Invalid latitude";
                return null;
            }
            if (!double.TryParse(ls[4], out lng))
            {
                errorstr = "Invalid longitude";
                return null;
            }
            var p = new PointLatLng(lat, lng);


            if ((ls[1] == "hfsystem") && ls.Length > 5)
            {
                int magnification = 1;
                var t = GMarkerHFsystemType.Feketekör;

                if (!int.TryParse(ls[4], out magnification))
                {
                    errorstr = "Invalid magnification";
                    return null;
                }

                switch (ls[5])
                {
                    case "blue": t = GMarkerHFsystemType.Kékkör; break;
                    case "red": t = GMarkerHFsystemType.Piroskör; break;
                    case "yellow": t = GMarkerHFsystemType.Sárgakör; break;
                    case "black": t = GMarkerHFsystemType.Feketekör; break;
                    case "green": t = GMarkerHFsystemType.Zöldkör; break;
                    case "pink": t = GMarkerHFsystemType.Lilakör; break;
                    case "brown": t = GMarkerHFsystemType.Bíborkör; break;
                }

                return new GMarkerHFsystem(p, t, 1, name);

            }
            else if ((ls[1] == "google") && ls.Length > 5)
            {
                var t = GMarkerGoogleType.blue_pushpin;

                switch (ls[5])
                {
                    case "blue": t = GMarkerGoogleType.blue_pushpin; break;
                    case "red": t = GMarkerGoogleType.red_pushpin; break;
                    case "yellow": t = GMarkerGoogleType.yellow_pushpin; break;
                    case "black": t = GMarkerGoogleType.black_small; break;
                    case "green": t = GMarkerGoogleType.green_pushpin; break;
                    case "purple": t = GMarkerGoogleType.purple_pushpin; break;
                    case "brown": t = GMarkerGoogleType.brown_small; break;
                }

                return new GMarkerGoogle(p, t, name);
            }
            else
            {
                var color = Color.Black;
                switch (ls[5])
                {
                    case "blue": color = Color.Blue; break;
                    case "red": color = Color.Red; break;
                    case "yellow": color = Color.Yellow; break;
                    case "black": color = Color.Black; break;
                    case "green": color = Color.Green; break;
                    case "pink": color = Color.Pink; break;
                    case "brown": color = Color.Brown; break;
                }

                switch (ls[1])
                {
                    case "circle": return new GMapMarkerCircle(p, color, name);
                    case "cross": return new GMarkerCross(p, color, name);
                    case "rectangle": return new GMapMarkerRect(p, color, name);
                    default:
                        errorstr = "Invalid marker";
                        return null;
                }
            }

            //return null;
        }


        public static GMapMarker CreateHFMarker(string[] ls, out string errorstr)
        {
            // 0: hmarker, 1:name, 2:lat, 3:long, 4:color
            string name = ls[1];
            if (name == ".") name = "";
            double lat = 0;
            double lng = 0;
            errorstr = null;

            if (!double.TryParse(ls[2], out lat))
            {
                errorstr = "Invalid latitude";
                return null;
            }
            if (!double.TryParse(ls[3], out lng))
            {
                errorstr = "Invalid longitude";
                return null;
            }
            var p = new PointLatLng(lat, lng);

            var t = GMarkerHFsystemType.Feketekör;

            switch (ls[4])
            {
                case "blue": t = GMarkerHFsystemType.Kékkör; break;
                case "red": t = GMarkerHFsystemType.Piroskör; break;
                case "yellow": t = GMarkerHFsystemType.Sárgakör; break;
                case "black": t = GMarkerHFsystemType.Feketekör; break;
                case "green": t = GMarkerHFsystemType.Zöldkör; break;
                case "pink": t = GMarkerHFsystemType.Lilakör; break;
                case "brown": t = GMarkerHFsystemType.Bíborkör; break;
            }

            if (ls.Length > 5)
            {
                string infotext = ls[5];
                int ind = 6;
                while (ind < ls.Length)
                    infotext += " " + ls[ind++];

                if (!string.IsNullOrWhiteSpace(infotext))
                {
                    infotext = infotext.Replace("\\n", "\n").Replace("{date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));


                    var o = new GMarkerHFsystem(p, t, 1, name);
                    if (infotext == "auto")
                         o.SetDefaultNote();
                    else o.Note = infotext.Replace("{geo}", o.ToGeoString()); ;
                    o.Name = name;
                    o.BallonState = GeoBallonState.geoDisplayBalloon;
                    return o;
                }
            }

            return new GMarkerHFsystem(p, t, 1, name);
        } /* CreateHFMarker */


        public static GMapMarker CreateGMarker(string[] ls, out string errorstr)
        {
            // 0: hmarker, 1:name, 2:lat, 3:long, 4:color
            string name = ls[1];
            if (name == ".") name = "";
            double lat = 0;
            double lng = 0;
            errorstr = null;

            if (!double.TryParse(ls[2], out lat))
            {
                errorstr = "Invalid latitude";
                return null;
            }
            if (!double.TryParse(ls[3], out lng))
            {
                errorstr = "Invalid longitude";
                return null;
            }
            var p = new PointLatLng(lat, lng);

            var t = GMarkerGoogleType.blue_pushpin;

            switch (ls[4])
            {
                case "blue": t = GMarkerGoogleType.blue_pushpin; break;
                case "red": t = GMarkerGoogleType.red_pushpin; break;
                case "yellow": t = GMarkerGoogleType.yellow_pushpin; break;
                case "black": t = GMarkerGoogleType.black_small; break;
                case "green": t = GMarkerGoogleType.green_pushpin; break;
                case "purple": t = GMarkerGoogleType.purple_pushpin; break;
                case "brown": t = GMarkerGoogleType.brown_small; break;
            }

            return new GMarkerGoogle(p, t, name);

        } /* CreateGMarker */



        public virtual void OnRender(Graphics g)
        {
            //
        }

        public string ToGeoString()
        {
            return _position.ToGeoString();
        }


        #region ISerializable Members

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
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Position", Position);
            info.AddValue("Tag", Tag);
            info.AddValue("Offset", Offset);
            info.AddValue("Area", _area);
            info.AddValue("ToolTip", ToolTip);
            info.AddValue("ToolTipMode", ToolTipMode);
            info.AddValue("ToolTipText", ToolTipText);
            info.AddValue("Visible", IsVisible);
            info.AddValue("DisableregionCheck", DisableRegionCheck);
            info.AddValue("IsHitTestVisible", IsHitTestVisible);
            info.AddValue("Color", Stroke.Color.ToArgb());
            info.AddValue("FillColor", Fill.Color.ToArgb());
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GMapMarker" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected GMapMarker(SerializationInfo info, StreamingContext context)
        {
            Position = Extensions.GetStruct(info, "Position", PointLatLng.Empty);
            Tag = Extensions.GetValue<object>(info, "Tag", null);
            Offset = Extensions.GetStruct(info, "Offset", Point.Empty);
            _area = Extensions.GetStruct(info, "Area", Rectangle.Empty);

            ToolTip = Extensions.GetValue<GMapToolTip>(info, "ToolTip", null);
            if (ToolTip != null) ToolTip.Marker = this;

            ToolTipMode =
                Extensions.GetStruct(info, "ToolTipMode", MarkerTooltipMode.OnMouseOver);
            ToolTipText = info.GetString("ToolTipText");
            IsVisible = info.GetBoolean("Visible");
            DisableRegionCheck = info.GetBoolean("DisableregionCheck");
            IsHitTestVisible = info.GetBoolean("IsHitTestVisible");
            ChangeColor(Color.FromArgb(Extensions.GetStruct(info, "Color", Color.MidnightBlue.ToArgb())));
            Fill = new SolidBrush(Color.FromArgb(Extensions.GetStruct(info, "FillColor", Color.FromArgb(155, Color.AliceBlue).ToArgb())));
        }

        #endregion

        #region IDisposable Members

        bool _disposed;

        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                Tag = null;

                if (ToolTip != null)
                {
                    _toolTipText = null;
                    ToolTip.Dispose();
                    ToolTip = null;
                }
            }
        }

        #endregion
    }

    public delegate void MarkerClick(GMapMarker item, MouseEventArgs e);

    public delegate void MarkerDoubleClick(GMapMarker item, MouseEventArgs e);

    public delegate void MarkerEnter(GMapMarker item);

    public delegate void MarkerLeave(GMapMarker item);

    /// <summary>
    ///     modeof tooltip
    /// </summary>
    public enum MarkerTooltipMode
    {
        OnMouseOver,
        Never,
        Always,
    }

}
