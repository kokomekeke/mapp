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


    } /* class MapEllipse */
} /* namespace SgxMap.Objects */
