/*=============================================================================
*  MapRuler: draw a line and display distance
*
* (c) Kótai 2026.
*
* ver 1.00                                                         03-Sep-2026.
*           Last modification                                      04-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;

namespace SgxMap.Shapes
{
    public class MapRuler : MapLine, ISerializable, IDisposable
    {

        private MapTextBox? m_label;


        public MapRuler(PointLatLng p1, PointLatLng p2, Color color)
            : base(p1, p2, color)
        {

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
            if (point.IsEmpty || (point == Position2))
                return;

            Position2 = point;
            if (m_label == null)
                m_label = Layer.AddTextBox("", Position2, Length.ToString("F1") + " km", new Font("Arial", 12));
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

        public override void UpdateEndPoint(int x, int y)
        {
            var point = Layer.MapPanel.FromLocalToLatLng(x, y);
            SetPosition2(point);
        }


        public void ShowLength()
        {
            if (m_label == null)
            {
                m_label = new MapTextBox(Position2, Length.ToString("F1") + " km ");
                m_label.Layer = this.Layer;
                m_label.m_Font = new Font("Arial", 8);
            }
            else m_label.Text = Length.ToString("F1") + " km";

            if (m_label.Layer == null)
                m_label.Layer = this.Layer;

            m_layer?.RefreshObject(m_label);
        }


        public void RemoveLabel()
        {
            if ((Layer == null) || (m_label == null)) return;
            try
            {
                Layer?.RemoveObject(m_label);
                m_label = null;
            }
            catch { }
        }


        public override void Remove()
        {
            if (m_label != null)
            {
                m_label?.Remove();
                m_label=null;
            }
            base.Remove();
        }



    } /* class MapRuler */
} /* namespace SgxMap.Objects */
