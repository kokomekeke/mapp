/*=============================================================================
*  Base point class for SgxMap. (Base class of all drawing objects on map)
*
* (c) Kótai 2026.
*
* ver 1.00                                                         31-Aug-2026.
*          Last modification                                       04-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using Newtonsoft.Json.Linq;

namespace SgxMap.Shapes
{
    public class BasePoint: ISerializable, IDeserializationCallback, IDisposable
    {
        #region static members
        public static long LastID = 0;

        public long GetNextID()
        {
            return ++LastID;
        }

        public void ResetIDCounter()
        {
            LastID = 0;
        }
        #endregion

        protected long m_id;
        protected PointLatLng m_position = PointLatLng.Empty;
        protected string m_name = string.Empty;
        protected bool m_isSelected = false;
        protected bool m_isVisible = true;
        protected bool m_isMouseOver = false;
        protected bool m_isHighlighted = false;
        protected bool m_isEditable = true;           // if true, then the ellipse can be edited by mouse, otherwise it is read-only
        protected bool IsHitTestVisible = true;        // can receive input
        protected bool m_isfilled = false;

        protected GraphicsPath? m_graphicsPath;
        protected Layer m_layer;

        public Point LocalPosition;

        protected int m_thickness = 2;

        /// <summary>
        /// specifies how the outline is painted
        /// </summary>
        [NonSerialized] public Pen Stroke = new Pen(Brushes.Black, 2);
        [NonSerialized] public Pen HighlightStroke = new Pen(Brushes.Yellow, 6);
        [NonSerialized] public SolidBrush HighlightFill = new SolidBrush(Color.Yellow);
        [NonSerialized] public SolidBrush Fill = new SolidBrush(Color.White);

        public long ID
        {
            get { return m_id; }
            protected set { 
                m_id = value;
                if (m_id > LastID) LastID = m_id;
            }
        }         

        public PointLatLng Position
        {
            get { return m_position; }
            set
            {
                if (m_position != value)
                {
                    MoveTo(value);
                    m_layer?.RefreshObject(this);
                }
            }   
        }

        public Color Color
        {
            get { return Stroke.Color; }
            set
            {
                if (Stroke.Color != value)
                {
                    Stroke.Color = value;
                    m_layer?.RefreshObject(this);
                }
            }
        }

        public Color HighlightColor
        {
            get { return HighlightStroke.Color; }
            set
            {
                if (HighlightStroke.Color != value)
                {
                    HighlightStroke.Color = value;
                    HighlightFill = new SolidBrush(value);
                    m_layer?.RefreshObject(this);
                }
            }
        }

        public Color FillColor
        {
            get { return Fill.Color; }
            set
            {
                if (value != Fill.Color)
                {
                    Fill.Color = value;
                    m_layer?.RefreshObject(this);
                }
            }
        }


        public int Thickness
        {
            get { return m_thickness; }
            set
            {
                if (m_thickness != value)
                {
                    m_thickness = value;
                    Stroke.Width = m_thickness;
                    HighlightStroke.Width = m_thickness + 4;
                    m_layer?.RefreshObject(this);
                }
            }
        }


        public string Name
        {
            get { return m_name; }
            set
            {
                if (m_name != value)
                {
                    m_name = value;
                }
            }
        }


        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                if (m_isSelected != value)
                {
                    Select(value);
                }
            }
        }


        public virtual bool IsVisible
        {
            get { return m_isVisible; }
            set
            {
                if (m_isVisible != value)
                {
                    m_isVisible = value;
                    m_layer?.RefreshObject(this);
                }
            }
        }


        public bool IsHighlighted
        {
            get { return m_isHighlighted; }
            set
            {
                if (m_isHighlighted != value)
                {
                    m_isHighlighted = value;
                    m_layer?.RefreshObject(this);
                }
            }
        }

        public bool IsEditable
        {
            get { return m_isEditable; }
            set { m_isEditable = value; }
        }


        public Layer Layer
        {
            get { return m_layer; }
            internal set
            {
                if (m_layer != value)
                {
                    m_layer = value;
                }
            }
        }

        /// <summary>
        ///     is mouse over marker
        /// </summary>
        public bool IsMouseOver
        {
            get
            {
                return m_isMouseOver;
            }
            internal set
            {
                m_isMouseOver = value;
            }
        }

        public bool IsFilled
        {
            get { return m_isfilled; }
            set
            {
                if (value != m_isfilled)
                {
                    m_isfilled = value;
                    UpdateLocalPosition();
                }
            }
        }

        //***************************************************************************/

        public BasePoint()
        {
            m_id = ++LastID;
            m_position = PointLatLng.Empty;
            m_name = "name-" + m_id;
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// Constructor for BasePoint class
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        //---------------------------------------------------------------------
        public BasePoint(string name, PointLatLng position, Color color)
        {
            m_id = ++LastID;
            m_position = position;
            m_name = name;
            Stroke.Color = color;
            Stroke.SetLineCap(LineCap.NoAnchor, LineCap.ArrowAnchor, DashCap.Flat);
        }

        public BasePoint(PointLatLng position, Color color)
        {
            m_id = ++LastID;
            m_position = position;
            m_name = "name-" + m_id;
            Stroke.Color = color;
            Stroke.SetLineCap(LineCap.NoAnchor, LineCap.ArrowAnchor, DashCap.Flat);
        }

        public BasePoint(string name, List<PointLatLng> points, Color color)
        {
            m_id = ++LastID;
            if (string.IsNullOrEmpty(name))
                 name = "Object-" + m_id.ToString();
            else m_name = name;
            Stroke.Color = color;
            Stroke.SetLineCap(LineCap.NoAnchor, LineCap.ArrowAnchor, DashCap.Flat);
            if ((points != null) && (points.Count > 0))
            {
                m_position = points[0];
            }
        }


        public static BasePoint? CreatePoint(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "Point", 1: name,  2: lat1, 3: lng1, 4: color

                errorstring = string.Empty;

                if (ls.Length < 5)
                {
                    errorstring = "Not enough parameters to create a point.";
                    return null;
                }

                string name = ls[1];
                var position = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                var color = ColorTranslator.FromHtml(ls[4]);
                return new BasePoint(name, position, color);
            }
            catch (Exception ex)
            {
                errorstring = $"Error creating point: {ex.Message}";
                return null;
            }
        }


        public bool IsEmpty()
        {
            return (m_position == PointLatLng.Empty);
        }


        public string ToGeoString()
        {
            return m_position.ToGeoString();
        }

        protected virtual void MoveTo(PointLatLng pos)
        {
            m_position = pos;
        }

        public virtual void Select(bool select)
        {
            m_isSelected = select;
            m_layer?.RefreshObject(this);
        }


        public void UpdateGraphicsPath()
        {
            if (m_graphicsPath == null)
            {
                m_graphicsPath = new GraphicsPath();
            }
            else
            {
                m_graphicsPath.Reset();
            }

            BuildGraphicsPath();
        }


        public virtual void BuildGraphicsPath()
        {
            
        }


        public virtual void OnRender(Graphics g)
        {
            if (m_graphicsPath == null)
            {
                UpdateGraphicsPath();
            }
            if (m_graphicsPath != null)
            { 
                if (m_isfilled)
                     g.FillPath(Fill, m_graphicsPath);
                g.DrawPath(Stroke, m_graphicsPath);
            }
        }

        public virtual void UpdateLocalPosition()
        {
            if (Layer != null && Layer.MapPanel != null)
            {
                LocalPosition = Layer.MapPanel.PositiontoLocal(Position);
            }
        }


        public virtual void UpdateObject()
        {
            UpdateLocalPosition();
            UpdateGraphicsPath();
        }

        /// <summary>
        ///     Indicates whether the specified point is contained within this System.Drawing.Drawing2D.GraphicsPath
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual bool IsInside(int x, int y)
        {
            if (m_graphicsPath != null)
            {
                return m_graphicsPath.IsOutlineVisible(x, y, Stroke);
            }

            return false;
        }

        public virtual bool IsInside(PointLatLng p)
        {
            if (m_graphicsPath != null)
            {
                var d = m_layer?.FromLatLngToLocalPosition(p);
                if (d != null)
                    return m_graphicsPath.IsOutlineVisible(d.Value.X, d.Value.Y, Stroke);
            }

            return false;
        }


        public virtual void Remove()
        {
            m_layer?.RemoveObject(this);
        }

        public virtual void MovePoint(PointLatLng oldpoint, PointLatLng newpoint)
        {
            if (Position == oldpoint)
                Position = newpoint;
        }


        public virtual void Shift(PointLatLng oldpoint, PointLatLng newpoint)
        {
            Position = newpoint;
        }

        public virtual void UpdateLinePoints()
        {
            //UpdateLocalPosition(); UpdateGraphicsPath();
        }


        #region serialization
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("Name", m_name);
            info.AddValue("Position", Position);
            info.AddValue("Color", Stroke.Color);
        }

        protected BasePoint(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt64("ID");
            Name = info.GetString("Name") ?? string.Empty;
            Position = (PointLatLng)info.GetValue("Position", typeof(PointLatLng));
            Color = (Color)info.GetValue("Color", typeof(Color));
        }

        public virtual void OnDeserialization(object? sender)
        {

        }

        #endregion serialization

        #region Dispose
        public virtual void Dispose()
        {
            Remove();
            if (Stroke != null)
            {
                Stroke.Dispose();
                Stroke = null;
            }
        }

        #endregion Dispose


    } /* class BasePoint */
}  /* namespace SgxMap.Objects */
