/*=============================================================================
*  MapEllipse: Ellipse on map
*
* (c) Kótai 2026.
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
    public class MapEllipse: MapRectangle, ISerializable
    {

        public MapEllipse(PointLatLng center, float width, float height, System.Drawing.Color color)
            : base(center, width, height, color)
        {
        }

        public MapEllipse(PointLatLng topleft, PointLatLng bottomright, System.Drawing.Color color)
            : base(topleft, bottomright, color)
        {
        }

        public MapEllipse(string name, PointLatLng center, float width, float height, System.Drawing.Color color)
            : base(name, center, width, height, color)
        {
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

            m_graphicsPath?.AddEllipse(new Rectangle(posx, posy, w, h));
        }

        public override void Shift(PointLatLng oldpoint, PointLatLng newpoint)
        {
            Position = newpoint;
        }


        /// <summary>
        ///     checks if point is inside the ellipse,
        ///     info.: http://greatmaps.codeplex.com/discussions/279437#post700449
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool IsInside(PointLatLng p)
        {
            var d = Layer.MapPanel.FromLatLngToLocalPosition(p);
            float dx = d.X - LocalPosition.X;
            float dy = d.Y - LocalPosition.Y;
            return Math.Abs(((dx * dx) / (m_localWidth * m_localWidth /4) + (dy * dy) / (m_localHeight * m_localHeight/4)) - 1.0) <= 0.05;
        }

        public override bool IsInside(int x, int y)
        {
            float dx = x - LocalPosition.X - m_localWidth / 2f;
            float dy = y - LocalPosition.Y - m_localHeight / 2f;
            return Math.Abs(((dx * dx) / (m_localWidth * m_localWidth / 4) + (dy * dy) / (m_localHeight * m_localHeight / 4)) - 1.0) <= 10;
        }


    } /* class MapEllipse */
} /* namespace SgxMap.Objects */
