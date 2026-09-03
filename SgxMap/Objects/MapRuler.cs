using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;

namespace SgxMap.Objects
{
    public class MapRuler : MapLine, ISerializable, IDeserializationCallback, IDisposable
    {

        private MapLabel m_label;


        public MapRuler(Layer layer, int x, int y, Color color)
            : base(layer.Control.FromLocalToLatLng(x, y), layer.Control.FromLocalToLatLng(x, y), color)
        {
            Layer = layer;
        }

        public override void OnRender(Graphics g)
        {
            try
            {
                base.OnRender(g);
                m_label?.OnRender(g);
            }
            catch { }
            ;
        }

        public void UpdateLabel()
        {
            if ((m_label != null) && !string.IsNullOrEmpty(m_label.Text))
                m_label.UpdateLocalPosition();
        }


        public override void BuildGraphicsPath()
        {
            base.BuildGraphicsPath();
            m_label?.BuildGraphicsPath();
        }


        public void SetPosition2(PointLatLng point)
        {
            if ((point == null) || point.IsEmpty || (point == Position2))
                return;

            Position2 = point;
            if (m_label == null)
                m_label = Layer.AddLabel(Position2, Length.ToString("F1") + " km", new Font("Arial", 12), true);
            else
            {
                if (m_label.Layer == null)
                    m_label.Layer = this.Layer;

                m_label.Position = point;
                m_label.Text = Length.ToString("F1") + " km";
                Layer?.RefreshObject(m_label);
            }
            Layer?.RefreshObject(this);
        }

        public void UpdateEndPoint(int x, int y)
        {
            var point = Layer.Control.FromLocalToLatLng(x, y);
            SetPosition2(point);
        }

        public void ShowLength()
        {
            if (m_label == null)
            {
                m_label = new MapLabel(Position2, Length.ToString("F1") + " km ");
                m_label.Layer = this.Layer;
                m_label.Font = new Font("Arial", 8);
            }
            else m_label.Text = Length.ToString("F1") + " km";

            if (m_label.Layer == null)
                m_label.Layer = this.Layer;

            m_layer?.RefreshObject(m_label);
        }

    } /* class MapRuler */
} /* namespace SgxMap.Objects */
