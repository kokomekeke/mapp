/*=============================================================================
*  Layer in SgxMap
*
* (c) Kótai 2026.
*
* ver 1.00                                                         01-Sep-2026.
=============================================================================*/
using System.Runtime.Serialization;
using GMap.NET;
using SgxMap.ObjectModel;
using SgxMap.Objects;

namespace SgxMap
{
    public class Layer
    {
        private long m_id;
        private string m_name = string.Empty;
        bool _isVisibile = true;
        SgxMapPanel? _control;


        /// <summary>
        ///     list of objects, should be thread safe
        /// </summary>
        public readonly ObservableCollectionThreadSafe<BasePoint> Objects =
            new ObservableCollectionThreadSafe<BasePoint>();

        public long ID
        {
            get { return m_id; }
            private set { m_id = value; }
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public bool IsVisible
        {
            get { return _isVisibile; }
            set { _isVisibile = value; }
        }


        public SgxMapPanel? Control
        {
            get
            {
                return _control;
            }
            internal set
            {
                _control = value;
            }
        }


        public Layer(string? name, SgxMapPanel? control = null)
        {
            m_id = BasePoint.LastID++;
            m_name = name ?? "layer-" + m_id;
            _control = control;
        }

        public void AddObject(BasePoint obj)
        {
            if (obj == null) return;
            Objects.Add(obj);
            obj.Layer = this;
        }

        public void RemoveObject(BasePoint obj)
        {
            if (obj == null) return;
            Objects.Remove(obj);
            obj.Layer = null;
        }

        public void ClearObjects()
        {
            foreach (var obj in Objects)
            {
                obj.Layer = null;
            }
            Objects.Clear();
        }


        public Point? FromLatLngToLocalPosition(PointLatLng position)
        {
            if (Control != null)
            {
                return Control.FromLatLngToLocalPosition(position);
            }
            return null;
        }


        public BasePoint? GetBasePointbyName(string name)
        {
            return Objects.FirstOrDefault(obj => obj.Name == name);
        }

        public BasePoint? GetBasePointbyID(long id)
        {
            return Objects.FirstOrDefault(obj => obj.ID == id);
        }

        public BasePoint? GetBasePointbyPosition(PointLatLng position)
        {
            return Objects.FirstOrDefault(obj => obj.Position == position);
        }

        public BasePoint? GetObjectbyLocalPosition(int x, int y)
        {
            return Objects.FirstOrDefault(obj => obj.IsInside(x, y));
        }

        public void ForceUpdate()
        {
            if (Control != null)
            {
                foreach (var obj in Objects)
                {
                    if (obj.IsVisible)
                    {
                        obj.UpdateLocalPosition();
                    }
                }
            }
        }


        /// <summary>
        ///     renders objects/routes/polygons
        /// </summary>
        /// <param name="g"></param>
        public virtual void OnRender(Graphics g)
        {
            if (Control != null)
            {
                foreach (var o in Objects)
                {
                    if (o.IsVisible)
                    {
                        o.OnRender(g);
                    }
                }
            }
        }

        public void AddFrom(Layer overlay)
        {
            if (overlay != null)
            {
                foreach (var m in overlay.Objects)
                {
                    Objects.Add(m);
                }

            }
        }


        public void RefreshObjects()
        {
            foreach (var obj in Objects)
            {
                obj.UpdateLocalPosition();
            }

            if (Control != null && !Control.HoldInvalidation)
            {
                Control.Invalidate();
            }
        }


        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", ID);
            info.AddValue("Name", Name);
            info.AddValue("IsVisible", IsVisible);

            var objectArray = new BasePoint[Objects.Count];
            Objects.CopyTo(objectArray, 0);
            info.AddValue("Objects", objectArray);
        }

        protected Layer(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt64("Id");
            Name = info.GetString("Name") ?? string.Empty;
            IsVisible = info.GetBoolean("IsVisible");
            var objectArray = (BasePoint[])info.GetValue("Objects", typeof(BasePoint[]))!;
            foreach (var obj in objectArray)
            {
                AddObject(obj);
            }
        }

        #endregion ISerializable Members

        #region IDisposable Members

        bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                //ClearEvents();

                foreach (var m in Objects)
                {
                    m.Dispose();
                }

                ClearObjects();
            }
        }

        #endregion IDisposable Members





    } /* class Layer */
} /* namespace SgxMap */

