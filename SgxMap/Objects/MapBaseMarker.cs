/*=============================================================================
*  MapMarker: base class of all markers on map
*
* (c) Kótai 2026.
*
* ver 1.00                                                         06-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;

namespace SgxMap.Objects
{
    public class MapBaseMarker: BasePoint, ISerializable
    {
        public MovementType Movement = MovementType.Both;
        public object? m_parent = null;
        protected Point  m_offset;
        protected Rectangle m_area;    // System.Drawing rectangle
        public bool DisableRegionCheck;             // if true, marker will be rendered even if it's outside current view
        protected PointLatLng m_oldposition;

        /// <summary>
        /// background color
        /// </summary>
        [NonSerialized]
        protected SolidBrush Fill = new SolidBrush(Color.FromArgb(155, Color.AliceBlue));
        protected Pen BorderPen = new Pen(Color.Black);

        public object? Parent
        {
            get { return m_parent; }
            protected set { m_parent = value; }
        }


        public Point Offset
        {
            get
            {
                return m_offset;
            }
            set
            {
                if (m_offset != value)
                {
                    m_offset = value;
                    Layer?.RefreshObject(this);
                }
            }
        }

        public Rectangle LocalArea
        {
            get
            {
                return m_area;
            }
        }



        /// <summary>
        ///     ToolTip position in local coordinates
        /// </summary>
        public Point ToolTipPosition
        {
            get
            {
                var ret = m_area.Location + new Size(m_area.Size.Width / 2, m_area.Size.Height / 2);
                ret.Offset(-Offset.X, -Offset.Y);
                return ret;
            }
        }

        public Size Size
        {
            get
            {
                return m_area.Size;
            }
            set
            {
                m_area.Size = value;
            }
        }


        public Color BorderColor
        {
            get { return BorderPen.Color; }
            set
            {
                BorderPen.Color = value;
                Layer?.RefreshObject(this);
            }
        }

        public Color FillColor
        {
            get { return Fill.Color; }
            set { Fill = new SolidBrush(value); }
        }

        //public GMapToolTip ToolTip;

        //public MarkerTooltipMode ToolTipMode = MarkerTooltipMode.OnMouseOver;

        //string _toolTipText;

        //public virtual string ToolTipText
        //{
        //    get
        //    {
        //        return _toolTipText;
        //    }

        //    set
        //    {
        //        if (ToolTip == null && !string.IsNullOrEmpty(value))
        //        {
        //            ToolTip = new GMapRoundedToolTip(this);
        //        }

        //        _toolTipText = value;
        //    }
        //}





        public MapBaseMarker()
        {
        }

        public MapBaseMarker(string name, PointLatLng pos, Size size, Color color)
            : base(name, pos, color) 
        {
            Size = size;
            Fill = new SolidBrush(color);
        }

        public MapBaseMarker(string name, PointLatLng pos, int size, Color color)
            : base(name, pos, color)
        {
            Size = new Size(size, size);
            Fill = new SolidBrush(color);
        }



        public static MapBaseMarker? CreateMarker(string[] ls, out string errorstr)
        {
            // 0: marker, 1:name, 2:lat, 3:long, 4: size, 5:color
            string name = ls[1];
            if (name == ".") name = "";
            double lat = 0;
            double lng = 0;
            errorstr = string.Empty;

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

            if (!int.TryParse(ls[4], out int size))
            {
                errorstr = "Invalid size";
                return null;
            }

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

            var p = new PointLatLng(lat, lng);

            return new MapBaseMarker(name, p, new Size(size, size), color);
        } /* CreateMarker */


        public virtual void CopyToClipboard()
        {
            System.Windows.Forms.Clipboard.SetText(Position.ToGeoString());
        }


        public virtual void ChangeColor(Color color)
        {
            Stroke = new Pen(color, Stroke.Width);
        }

        protected override void ChangePosition(PointLatLng pos)
        {
            if (pos == m_position)
                return;

            m_oldposition = m_position;
            base.ChangePosition(pos);

            if (IsVisible)
            {
                if (Parent != null)
                {
                    if (Parent is GeoCircle)
                    {
                        ((GeoCircle)Parent).ChangeRadius(m_position);
                    }
                    else if ((Parent is MapPolyLine) && (m_oldposition != null))
                    {
                        ((MapPolyLine)Parent).MovePoint(m_oldposition, m_position);
                    }

                }
            }

            Layer?.RefreshObject(this);
        } /* ChangePosition */


        public override void UpdateLocalPosition()
        {
            if (Layer != null && Layer.Control != null)
            {
                base.UpdateLocalPosition();
                m_area.Location = new Point(LocalPosition.X - Size.Width / 2, LocalPosition.Y - Size.Height / 2);
            }
        }


        public override void BuildGraphicsPath()
        {
            m_graphicsPath?.AddRectangle(m_area);
            var rec = Rectangle.Inflate(m_area, -1, -1);
            m_graphicsPath?.AddEllipse(rec);
        }


        public override void OnRender(Graphics g)
        {
            if (IsSelected)
                g.DrawRectangle(BorderPen, m_area);
            if (m_area.Width > 2)
            {
                var rec = Rectangle.Inflate(m_area, -1, -1);
                g.FillEllipse(Fill, rec);
            }
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
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("Position", Position);
            info.AddValue("Offset", Offset);
            info.AddValue("Area", m_area);
            //info.AddValue("ToolTip", ToolTip);
            //info.AddValue("ToolTipMode", ToolTipMode);
            //info.AddValue("ToolTipText", ToolTipText);
            info.AddValue("Visible", IsVisible);
            info.AddValue("DisableregionCheck", DisableRegionCheck);
            info.AddValue("IsHitTestVisible", IsHitTestVisible);
            info.AddValue("Color", Stroke.Color.ToArgb());
            info.AddValue("FillColor", Fill.Color.ToArgb());
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MapBaseMarker" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected MapBaseMarker(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt64("ID");
            Position = Extensions.GetStruct(info, "Position", PointLatLng.Empty);
            Offset = Extensions.GetStruct(info, "Offset", Point.Empty);
            m_area = Extensions.GetStruct(info, "Area", Rectangle.Empty);

            //ToolTip = Extensions.GetValue<GMapToolTip>(info, "ToolTip", null);
            //if (ToolTip != null) ToolTip.Marker = this;
            //ToolTipMode =
            //    Extensions.GetStruct(info, "ToolTipMode", MarkerTooltipMode.OnMouseOver);
            //ToolTipText = info.GetString("ToolTipText");
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

                //if (ToolTip != null)
                //{
                //    _toolTipText = null;
                //    ToolTip.Dispose();
                //    ToolTip = null;
                //}
            }
        }

        #endregion
    } /* class MapMarker */

    public delegate void MarkerClick(MapBaseMarker item, MouseEventArgs e);

    public delegate void MarkerDoubleClick(MapBaseMarker item, MouseEventArgs e);

    public delegate void MarkerEnter(MapBaseMarker item);

    public delegate void MarkerLeave(MapBaseMarker item);


} /* namespace SgxMap.Objects */


