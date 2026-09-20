/*=============================================================================
*  MapRectangle: rectangle on map
*
* (c) Kótai 2026
*
* ver 1.00                                                         04-Sep-2026.
*          Last modification                                       20-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;

namespace SgxMap.Shapes
{
    public class MapRectangle: MapLine, ISerializable
    {
        protected float m_width;             // in km
        protected float m_height;

        protected float m_localWidth;
        protected float m_localHeight;
        protected Rectangle m_area;

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


        public float LocalWidth
        { 
            get { return m_localWidth; } 
        }

        public float LocalHeight
        {
            get { return m_localHeight; }
        }



        //---------------------------------------------------------------------------------------------

        public MapRectangle()
            : base()
        {

        }

        public MapRectangle(PointLatLng center, float width, float height, System.Drawing.Color color )
        {
            Position = center;
            m_width = width;
            m_height = height;
            Color = color;
            Position2 = PointLatLng.Add(Position, new SizeLatLng((int)width, (int)height));
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
            Position2 = PointLatLng.Add(Position, new SizeLatLng((int)width, (int)height));
        }


        public static MapRectangle CreateRectangle(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "circle", 1: name,  2: lat1, 3: lng1, 4: radius, 5: Width, 6: color
                errorstring = string.Empty;
                var p1 = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                float Width = float.Parse(ls[4]);
                float Height = float.Parse(ls[5]);
                var color = ColorTranslator.FromHtml(ls[6]);

                return new MapRectangle(ls[1], p1, Width, Height, color);
            }
            catch (Exception ex)
            {
                errorstring = ex.ToString();
                return null;
            }
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

            m_area = new Rectangle(posx, posy, w, h);
            m_graphicsPath?.AddRectangle(m_area);
        }


        public override void OnRender(Graphics g)
        {
            if (m_graphicsPath == null)
            {
                UpdateGraphicsPath();
            }
            if (m_graphicsPath != null)
            {
                if (IsHighlighted)
                    g.DrawRectangle(HighlightStroke, Rectangle.Inflate(m_area, 4,4));
                g.DrawPath(Stroke, m_graphicsPath);
            }
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


        public void SortPoints()
        {
            double x1 = Position.Lng;
            double x2 = Position2.Lng;
            double y1 = Position.Lat;
            double y2 = Position2.Lat;

            m_position.Lng = Math.Min(x1, x2);
            m_position2.Lng = Math.Max(x1, x2);

            m_position.Lat = Math.Max(y1, y2);
            m_position2.Lat = Math.Min(y1, y2);

            UpdateLocalPosition();
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
            Points[0] = Position;                                                           // top left
            Points[1] = Position2;                                                          // bottom right
            Points[2] = new PointLatLng(Position2.Lat, Position.Lng);                       // bottom left
            Points[3] = new PointLatLng(Position.Lat, Position2.Lng);                       // top right

            var p = Layer.MapPanel.GetPositionFromPoint(Position, Width / 2, 90);
            Points[4] = new PointLatLng(Position.Lat, p.Lng);                         // top center
            p = Layer.MapPanel.GetPositionFromPoint(Position2, Width / 2, -90);
            Points[5] = new PointLatLng(Position2.Lat, p.Lng);                               // bottom center

            p = Layer.MapPanel.GetPositionFromPoint(Position, Height / 2, 180);
            Points[6] = new PointLatLng(p.Lat, Position.Lng);                                // left center
            p= Layer.MapPanel.GetPositionFromPoint(Position2, Height / 2, 360);
            Points[7] = new PointLatLng(p.Lat, Position2.Lng);                               // right cnter
        }


        public override void Select(bool select)
        {
            if (select)
            {
                if (m_isSelected)
                    return;

                Layer.MapPanel.ClearLinePoints();

                BuildPoints();

                for (int i = 0; i < Points.Length; i++)
                {
                    if (!Points[i].IsEmpty)
                        //Layer.AddLinePoint(this, Points[i], (i > 5) ? MovementType.Horizontal : (i > 3)? MovementType.Vertical : MovementType.Both, Color.Black);
                        Layer.AddLinePoint(this, Points[i], MovementType.Both, Color.Black);
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
                            Position = new PointLatLng(m_position.Lat, newpoint.Lng); 
                            Position2 = new PointLatLng(newpoint.Lat, m_position2.Lng);
                            break;
                        case 3:
                            Position = new PointLatLng(newpoint.Lat, m_position.Lng);
                            m_position2 = new PointLatLng(m_position2.Lat, newpoint.Lng);
                            break;
                        case 4:
                            Position = new PointLatLng(newpoint.Lat, m_position.Lng);
                            break;
                        case 5:
                            Position2 = new PointLatLng(newpoint.Lat, m_position2.Lng);
                            break;
                        case 6:
                            Position = new PointLatLng(m_position.Lat, newpoint.Lng);
                            break;
                        case 7:
                            Position2 = new PointLatLng(m_position2.Lat, newpoint.Lng);
                            break;

                        default: return;
                    }
                    BuildPoints();
                    if (i >= 4)
                        Points[i] = newpoint; 
                    return;
                }
            }
        }


    } /* MapRectangle */
    } /*  namespace SgxMap.Objects */
