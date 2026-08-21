using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace Demo.WindowsForms.Source
{
    [Serializable]

    public class LayerGroup: ISerializable, IDeserializationCallback, IDisposable
    {
        public GMapOverlay Objects = new GMapOverlay("objects");
        public GMapOverlay Routes = new GMapOverlay("routes");
        public GMapOverlay Polygons = new GMapOverlay("polygons");

        public string MapProvider = GMapProviders.BingMap.ToString();
        public PointLatLng Center = new PointLatLng(0, 0);
        public double Zoom = 1;


        public LayerGroup()
        {
        }

        protected LayerGroup(SerializationInfo info, StreamingContext context)
        {
            Objects = (GMapOverlay)info.GetValue("Objects", typeof(GMapOverlay));
            Routes = (GMapOverlay)info.GetValue("Routes", typeof(GMapOverlay));
            Polygons = (GMapOverlay)info.GetValue("Polygons", typeof(GMapOverlay));
            MapProvider = info.GetString("MapProvider");
            Center = (PointLatLng)info.GetValue("Center", typeof(PointLatLng));
            Zoom = info.GetDouble("Zoom");
        }


        public void Clear()
        {
            Objects.Clear();
            Routes.Clear();
            Polygons.Clear();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Objects", Objects);
            info.AddValue("Routes", Routes);
            info.AddValue("Polygons", Polygons);
            info.AddValue("MapProvider", MapProvider);
            info.AddValue("Center", Center);
            info.AddValue("Zoom", Zoom);
        }

        public void OnDeserialization(object sender)
        {
        }

        public void Dispose()
        {
        }
    }
}
