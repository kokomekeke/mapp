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

        protected PointLatLng[] Points = new PointLatLng[8];

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
            if (Layer != null && Layer.MapPanel != null)
            {
                base.UpdateLocalPosition();
                m_localWidth = LocalPosition2.X - LocalPosition.X;
                m_localHeight = LocalPosition2.Y - LocalPosition.Y;
                UpdateGraphicsPath();
                var p = new PointLatLng(Position.Lat, Position2.Lng);
                m_width = (float)Math.Abs(Layer.MapPanel.GetDistance(Position, p));
                p = new PointLatLng(Position2.Lat, Position.Lng);
                m_height = (float)Math.Abs(Layer.MapPanel.GetDistance(Position, p));
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
            if (point.IsEmpty || (point == Position2))
                return;

            Position2 = point;
            LocalPosition = Layer.MapPanel.PositiontoLocal(Position);
            LocalPosition2 = Layer.MapPanel.PositiontoLocal(Position2);
            m_localWidth = LocalPosition2.X - LocalPosition.X;
            m_localHeight = LocalPosition2.Y - LocalPosition.Y;
            UpdateGraphicsPath();
            Layer?.RefreshObject(this);
        }



        public override void UpdateEndPoint(int x, int y)
        {
            SetPosition2(Layer.MapPanel.FromLocalToLatLng(x, y)); 
        }


        public override bool IsInside(int x, int y)
        {
            double x1 = LocalPosition.X;
            double y1 = LocalPosition.Y;
            double x2 = LocalPosition2.X;
            double y2 = LocalPosition2.Y;

            if (x >= x1 && x <= x2 && y >= y1 && y <= y2)
            {
                return (Math.Abs(x1 - x) < 10) || (Math.Abs(x2 - x) < 10) || (Math.Abs(y1 - y) < 10) || (Math.Abs(y2 - y) < 10);
            }

            return false;
        }

        protected void BuildPoints()
        {

            Points[0] = Position;
            Points[1] = Position2;
            Points[2] = Layer.MapPanel.GetPositionFromPoint(Position2, Height, 360);
            Points[3] = Layer.MapPanel.GetPositionFromPoint(Position, Height, 180);

            Points[4] = Layer.MapPanel.GetPositionFromPoint(Position, Width / 2, 90);
            Points[5] = Layer.MapPanel.GetPositionFromPoint(Position2, Width / 2, -90);

            Points[6] = Layer.MapPanel.GetPositionFromPoint(Position, Height / 2, 180);
            Points[7] = Layer.MapPanel.GetPositionFromPoint(Position2, Height / 2, 360);


        }

        public override void Select(bool select)
        {
            if (select)
            {
                if (m_isSelected)
                    return;

                Layer.MapPanel.ClearLinePoints();

                BuildPoints();

                foreach (var point in Points)
                {
                    if (!point.IsEmpty)
                        Layer.AddLinePoint(this, point, MovementType.Both, Color.Black);
                }
            }
            else
            {
                Layer.MapPanel.ClearLinePoints(this);
            }

            //base.Select(select);
            m_isSelected = select;
            m_layer?.RefreshObject(this);

        }

        public override void UpdateLinePoints()
        {
            if (m_isSelected)
            {
                Layer.MapPanel.ClearLinePoints();

                BuildPoints();

                foreach (var point in Points)
                {
                    if (!point.IsEmpty)
                        Layer.AddLinePoint(this, point, MovementType.Both, Color.Black);
                }

            }
        } /* UpdateLinePoints */

        public override void MovePoint(PointLatLng oldpoint, PointLatLng newpoint)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                if (oldpoint == Points[i])
                {
                    switch (i)
                    {
                        case 0: Position = newpoint; break;
                        case 1: Position2 = newpoint; break;
                        case 2:
                            //var p = new PointLatLng(newpoint.Lat, Position.Lng);
                            m_position.Lat = newpoint.Lat;
                            break;

                        default: return;
                    }
                    BuildPoints();
                    return;
                }
            }
        }


    } /* MapRectangle */
    } /*  namespace SgxMap.Objects */
