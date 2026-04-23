using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace GMap.NET.WindowsForms.Markers
{
    [Serializable]
    public class GMarkerLinePoint: GMapMarker, ISerializable
    {
        [NonSerialized]
        public Pen Pen;


        public GMarkerLinePoint(Object parent, PointLatLng p)
           : base(p)
        {
            Parent = parent;
            Pen = new Pen(Brushes.Black, 3);

            // do not forget set Size of the marker
            // if so, you shall have no event on it ;}
            Size = new Size(8, 8);
            Offset = new Point(-Size.Width / 2, -Size.Height / 2);
        }


        public override void OnRender(Graphics g)
        {
            g.DrawRectangle(Pen, new Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width/2, Size.Height/2));
            if (IsMouseOver)
                g.DrawRectangle(Pens.Red, new Rectangle(LocalPosition.X - Size.Width/4, LocalPosition.Y - Size.Height/4, Size.Width, Size.Height));
        }

        public override void Dispose()
        {
            if (Pen != null)
            {
                Pen.Dispose();
                Pen = null;
            }

            base.Dispose();
        }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected GMarkerLinePoint(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
        }

        #endregion
    }
}
