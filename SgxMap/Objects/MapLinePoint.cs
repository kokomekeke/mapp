using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;

namespace SgxMap.Objects
{
    public class MapLinePoint: MapBaseMarker, ISerializable
    {

        public MapLinePoint(BasePoint parent, PointLatLng p, Color color)
           : base("",p, new Size(8, 8), color)
        {
            Parent = parent;
            Name = Parent.Name;
        }

        public MapLinePoint(string name, BasePoint parent, PointLatLng p, Color color, MovementType movement)
           : base(name,p, new Size(8,8), color)
        {
            Parent = parent;
            m_movement = movement;
        }

        public override void OnRender(Graphics g)
        {
            g.DrawRectangle(Stroke, new Rectangle(LocalPosition.X - Size.Width / 4, LocalPosition.Y - Size.Height / 4, Size.Width / 2, Size.Height / 2));
            if (IsSelected)
                g.DrawRectangle(Pens.Red, LocalArea);
        }


        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected MapLinePoint(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
        }

        #endregion



    }
}
