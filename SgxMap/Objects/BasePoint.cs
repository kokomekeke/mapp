/*=============================================================================
*  Base point class for SgxMap. (Base class of all drawing objects on map
*
* (c) Kótai 2026.
*
* ver 1.00                                                         31-Aug-2026.
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
    internal class BasePoint: ISerializable, IDeserializationCallback, IDisposable
    {
        public static int LastID = 0;

        protected int m_id;
        protected PointLatLng m_position;
        protected string m_name = string.Empty;
        protected bool m_isSelected = false;
        protected bool m_isVisible = true;
        protected bool m_isMouseOver = false;
        protected GraphicsPath? m_graphicsPath;

        public Point LocalPosition;

        /// <summary>
        /// specifies how the outline is painted
        /// </summary>
        [NonSerialized] public Pen Stroke = new Pen(Brushes.Red, 2);

        public int ID
        {
            get { return m_id; }
            private set { m_id = value; }
        }         

        public PointLatLng Position
        {
            get { return m_position; }
            set
            {
                if (m_position != value)
                {
                    m_position = value;
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



        public BasePoint(string name, PointLatLng position, Color color)
        {
            m_id = ++LastID;
            m_position = position;
            m_name = name;
            Stroke.Color = color;
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
            //if (Overlay != null && Overlay.Control != null)
            //{
            //    Overlay.Control.UpdateLineLocalPosition(this);
            //    UpdateGraphicsPath();
            //}
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

        }

        public virtual void OnDeserialization(object? sender)
        {

        }

        #endregion serialization

        #region Dispose
        public virtual void Dispose()
        {
            if (Stroke != null)
            {
                Stroke.Dispose();
                Stroke = null;
            }
        }

        #endregion Dispose


    } /* class BasePoint */
}  /* namespace SgxMap.Objects */
