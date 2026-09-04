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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;

namespace SgxMap.Objects
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
        protected PointLatLng m_position;
        protected string m_name = string.Empty;
        protected bool m_isSelected = false;
        protected bool m_isVisible = true;
        protected bool m_isMouseOver = false;
        protected bool m_isHighlighted = false;
        private bool m_isEditable = true;      // if true, then the ellipse can be edited by mouse, otherwise it is read-only
        protected GraphicsPath? m_graphicsPath;
        protected Layer? m_layer;

        public Point LocalPosition;

        protected int m_thickness = 2;

        /// <summary>
        /// specifies how the outline is painted
        /// </summary>
        [NonSerialized] public Pen Stroke = new Pen(Brushes.Black, 2);
        [NonSerialized] public Pen NormalStroke = new Pen(Brushes.Black, 2);
        [NonSerialized] public Pen HighlightStroke = new Pen(Brushes.Red, 3);

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
                    m_position = value;
                    m_layer?.RefreshObject(this);
                }
            }   
        }

        public Color Color
        {
            get { return NormalStroke.Color; }
            set
            {
                if (NormalStroke.Color != value)
                {
                    NormalStroke.Color = value;
                    Stroke = m_isHighlighted ? HighlightStroke : NormalStroke;
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
                    Stroke = m_isHighlighted ? HighlightStroke : NormalStroke;
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
                    NormalStroke.Width = m_thickness;
                    Stroke = m_isHighlighted ? HighlightStroke : NormalStroke;
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
                    m_isSelected = value;
                    m_layer?.RefreshObject(this);
                }
            }
        }


        public bool IsVisible
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
                    Stroke = m_isHighlighted ? HighlightStroke : NormalStroke;
                    m_layer?.RefreshObject(this);
                }
            }
        }

        public bool IsEditable
        {
            get { return m_isEditable; }
            set { m_isEditable = value; }
        }


        public Layer? Layer
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
            NormalStroke.Color = color;
        }

        public BasePoint(PointLatLng position, Color color)
        {
            m_id = ++LastID;
            m_position = position;
            m_name = "name-" + m_id;
            NormalStroke.Color = color;
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
            g.DrawPath(Stroke, m_graphicsPath);
        }

        public virtual void UpdateLocalPosition()
        {
            if (Layer != null && Layer.Control != null)
            {
                LocalPosition = Layer.Control.PositiontoLocal(Position);
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
                    return m_graphicsPath.IsOutlineVisible(d.Value.X, d.Value.Y, NormalStroke);
            }

            return false;
        }


        public virtual void Remove()
        {
            m_layer?.RemoveObject(this);
        }

        #region serialization
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("Name", m_name);
            info.AddValue("Position", Position);
            info.AddValue("Color", NormalStroke.Color);
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
            if (NormalStroke != null)
            {
                NormalStroke.Dispose();
                NormalStroke = null;
            }
        }

        #endregion Dispose


    } /* class BasePoint */
}  /* namespace SgxMap.Objects */
