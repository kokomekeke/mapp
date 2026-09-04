/*=============================================================================
*  MapCircleRuler: draw radius and circle
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
    public class MapCircleRuler: MapRuler, ISerializable, IDisposable
    {

        public MapCircleRuler(PointLatLng p1, PointLatLng p2, Color color)
            : base(p1, p2, color)
        {

        }


        public override void BuildGraphicsPath()
        {
            base.BuildGraphicsPath();
            double w1 = LocalPosition.X - LocalPosition2.X;
            double w2 = LocalPosition.Y - LocalPosition2.Y;
            int r = (int)Math.Sqrt(w1*w1 + w2*w2);
            if (r > 0)
            {
                var rec = new Rectangle(LocalPosition.X - r, LocalPosition.Y - r, 2 * r, 2 * r);
                m_graphicsPath?.AddEllipse(rec);
            }
        }


    }
}
