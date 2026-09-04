/*=============================================================================
*  MapRectangle: rectangle on map
*
* (c) Kótai 2026
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
    public class MapRectangle: MapLine, ISerializable
    {
        protected float m_width;             // in km
        protected float m_height;

        protected float m_localWidth;
        protected float m_localHeight;


        public float Width
        {
            get { return m_width; }
            set
            {
                m_width = value;
                UpdateLocalPosition();
            }
        }

        public float Height
        {
            get { return m_height; }
            set
            {
                m_height = value;
                UpdateLocalPosition();
            }
        }


        public MapRectangle(PointLatLng center, float width, float height, System.Drawing.Color color )
        {
            Position = center;
            m_width = width;
            m_height = height;
            Color = color;
        }

        public MapRectangle(PointLatLng topleft, PointLatLng bottomright, System.Drawing.Color color)
        {
            Position = topleft;
            Position2 = bottomright;
            Color = color;
        }


        public MapRectangle(string name, PointLatLng center, float width, float height, System.Drawing.Color color)
        {
            Name = name;
            Position = center;
            m_width = width;
            m_height = height;
            Color = color;
        }



        public override void UpdateLocalPosition()
        {
            if (Layer != null && Layer.Control != null)
            {
                base.UpdateLocalPosition();
                m_localWidth = LocalPosition2.X - LocalPosition.X;
                m_localHeight = LocalPosition2.Y - LocalPosition.Y;
                UpdateGraphicsPath();
            }
        }

        public override void BuildGraphicsPath()
        {
            int posx = LocalPosition.X;
            int posy = LocalPosition.Y;
            int w = (int)m_localWidth;
            int h = (int)m_localHeight;
            if (w < 0)
            {
                posx = LocalPosition2.X;
                w = -w;
            }

            if (h < 0)
            {
                posy = LocalPosition2.Y;
                h = -h;
            }

            m_graphicsPath?.AddRectangle(new Rectangle(posx, posy, w, h));
        }


        public void SetPosition2(PointLatLng point)
        {
            if ((point == null) || point.IsEmpty || (point == Position2))
                return;

            Position2 = point;
            LocalPosition = Layer.Control.PositiontoLocal(Position);
            LocalPosition2 = Layer.Control.PositiontoLocal(Position2);
            m_localWidth = LocalPosition2.X - LocalPosition.X;
            m_localHeight = LocalPosition2.Y - LocalPosition.Y;
            UpdateGraphicsPath();
            Layer?.RefreshObject(this);
        }



        public virtual void UpdateEndPoint(int x, int y)
        {
            SetPosition2(Layer.Control.FromLocalToLatLng(x, y)); 
        }


    } /* MapRectangle */
} /*  namespace SgxMap.Objects */
