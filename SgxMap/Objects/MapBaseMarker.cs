/*=============================================================================
*  MapMarker: base class of all markers on map
*
* (c) Kótai 2026.
*
* ver 1.00                                                         06-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using Newtonsoft.Json.Linq;
using SgxMap.Shapes.ToolTips;
using Extensions = GMap.NET.Extensions;

namespace SgxMap.Shapes
{
    public class MapBaseMarker: BasePoint, ISerializable
    {
        public MovementType m_movement = MovementType.Both;
        public BasePoint? m_parent = null;
        protected Point  m_offset;
        protected Rectangle m_area;    // System.Drawing rectangle
        public bool DisableRegionCheck;             // if true, marker will be rendered even if it's outside current view
        protected PointLatLng m_oldposition;

        protected Pen BorderPen = new Pen(Color.Black);

        protected MapToolTip m_toolTip;
        protected MarkerTooltipMode m_toolTipMode = MarkerTooltipMode.Always;
        protected MarketTooltipTextType m_toolTipTextType = MarketTooltipTextType.Location;
        string m_toolTipText = string.Empty;
        string m_toolTipHeader = string.Empty;


        public BasePoint? Parent
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


        public MapToolTip ToolTip
        {
            get { return m_toolTip; }
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

        public virtual string ToolTipText
        {
            get
            {
                return m_toolTipText;
            }

            set
            {
                if (m_toolTip == null && !string.IsNullOrEmpty(value))
                {
                    m_toolTip = new MapToolTip(this);
                }

                m_toolTipText = value;
                m_toolTipTextType = MarketTooltipTextType.Custom;
                Layer?.RefreshObject(this);
            }
        }

        public virtual string ToolTipHeader
        {
            get
            {
                return m_toolTipHeader;
            }

            set
            {
                if (m_toolTip == null && !string.IsNullOrEmpty(value))
                {
                    m_toolTip = new MapPointToolTip(this);
                }

                m_toolTipHeader = value;
                m_toolTipTextType = MarketTooltipTextType.Custom;
                Layer?.RefreshObject(this);
            }
        }


        public MarkerTooltipMode ToolTipMode
        {
            get { return m_toolTipMode; }
            set
            {
                if (value != m_toolTipMode)
                {
                    m_toolTipMode = value;
                    Layer?.RefreshObject(this);
                }
            }
        }


        public MarketTooltipTextType TooltipTextType
        {
            get { return m_toolTipTextType; }
            set
            {
                m_toolTipTextType = value;
                ChangeTooltipText(Position);
                Layer?.RefreshObject(this);
            }
        }

        public override bool IsVisible
        {
            get { return m_isVisible; }
            set
            {
                if (m_isVisible == value)
                {
                    m_isVisible = value;
                    m_toolTip.IsVisible = value && m_toolTipMode != MarkerTooltipMode.Never;
                }
            }
        }

        //--------------------------------------------------------------------------------------

        public MapBaseMarker()
        {
        }

        public MapBaseMarker(string name, PointLatLng pos, Size size, Color color)
            : base(name, pos, color) 
        {
            Size = size;
            Fill = new SolidBrush(color);
            SetTooltipText(pos);
            ToolTipHeader = name;
        }

        public MapBaseMarker(string name, PointLatLng pos, int size, Color color)
            : base(name, pos, color)
        {
            Size = new Size(size, size);
            Fill = new SolidBrush(color);
            SetTooltipText(pos);
            ToolTipHeader = name;
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


        public void Reset()
        {
            Parent = null;
        }

        public MapToolTip AddToolTip()
        { 
            var t = new MapPointToolTip(this);
            Layer.AddToolTip(t);
            SetTooltipText(Position);
            if (string.IsNullOrEmpty(ToolTipHeader))
            {
                ToolTipHeader = "Untitled " + ID;
            }
            return t;
        }

        public virtual void CopyToClipboard()
        {
            System.Windows.Forms.Clipboard.SetText(Position.ToGeoString());
        }


        public virtual void ChangeColor(Color color)
        {
            Stroke = new Pen(color, Stroke.Width);
        }

        public void ChangeTooltipText(PointLatLng point)
        {
            if (m_toolTip == null)
            {
                m_toolTip = AddToolTip();
            }
            else SetTooltipText(point);

            m_toolTip.IsVisible = IsVisible && m_toolTipMode != MarkerTooltipMode.Never &&
                                (!string.IsNullOrEmpty(m_toolTipText) || (m_toolTipTextType == MarketTooltipTextType.Custom));
        }

        public void SetTooltipText(PointLatLng point)
        {
            switch (m_toolTipTextType)
            {
                case MarketTooltipTextType.MarkerName: m_toolTipText = Name; break;
                case MarketTooltipTextType.Location: m_toolTipText = point.ToGeoString(); break;
                case MarketTooltipTextType.Address: m_toolTipText = Layer.MapPanel.GetPlaceAddress(point); break;
                case MarketTooltipTextType.LocationAdderss: m_toolTipText = point.ToGeoString() + "   " + Layer.MapPanel.GetPlaceAddress(point); break;
                case MarketTooltipTextType.None: m_toolTipText = string.Empty; break;
            }

        }



        protected override void MoveTo(PointLatLng pos)
        {
            if (pos == m_position)
                return;

            m_oldposition = m_position;
            base.MoveTo(pos);

            ChangeTooltipText(pos);

            if (IsVisible)
            {
                if (Parent != null)
                {
                    if (Parent is MapLine)
                    {
                        ((MapLine)Parent).MovePoint(m_oldposition, pos);
                    }
                    else if (Parent is GeoCircle)
                    {
                        ((GeoCircle)Parent).ChangeRadius(m_position);
                    }
                    else if ((Parent is MapPolyLine) && !m_oldposition.IsEmpty)
                    {
                        ((MapPolyLine)Parent).MovePoint(m_oldposition, m_position);
                    }

                }
            }

            Layer?.RefreshObject(this);
        } /* ChangePosition */


        public override void UpdateLocalPosition()
        {
            if (Layer != null && Layer.MapPanel != null)
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
            info.AddValue("MoveMentType", m_movement);
            if (Parent != null)
            {
                info.AddValue("Parent", Parent.ID);
            }
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
            m_movement = Extensions.GetStruct(info, "MovementType", MovementType.Both);
        }


        public override bool IsInside(int x, int y)
        {
            return m_area.Contains(new Point(x, y));
        }

        public override bool IsInside(PointLatLng p)
        {
            if (m_graphicsPath != null)
            {
                var d = m_layer?.FromLatLngToLocalPosition(p);
                if (d != null)
                    return m_graphicsPath.IsOutlineVisible(d.Value.X, d.Value.Y, NormalStroke);
            }

            return false;
        }


        #endregion

        #region IDisposable Members

        bool _disposed;

        public override void Dispose()
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


