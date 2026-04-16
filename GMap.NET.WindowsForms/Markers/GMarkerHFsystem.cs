using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.ToolTips;

namespace GMap.NET.WindowsForms.Markers
{
    // ReSharper disable InconsistentNaming
    public enum GMarkerHFsystemType
    {
        none = 0,
        Világoskékkör,
        Lilakör,
        Sárgakör,
        Bíborkör,
        Feketekör,
        Piroskör,
        Kékkör,
        Sötétvöröskör,
        Zöldkör
    }

    public enum  GeoBallonState 
    {
        geoDisplayNone = 0,
        geoDisplayName,
        geoDisplayBalloon
    }

    // ReSharper restore InconsistentNaming

    [Serializable]
    public class GMarkerHFsystem : GMapMarker, ISerializable, IDeserializationCallback
    {
        private Bitmap _bitmap;

        public Bitmap Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
            }
        }

        public static int padding = 4;
        public static int padding2 = padding/2;

        public readonly GMarkerHFsystemType Symbol;       // symbol

        GeoBallonState _ballonstate;
        public GeoBallonState BallonState
        {
            get { return _ballonstate; }
            set
            {
                _ballonstate = value;
                if (ToolTip == null && !string.IsNullOrEmpty(Name))
                {
                    ToolTip = new GMapHFBaloonToolTip(this);
                }

                if (_ballonstate == GeoBallonState.geoDisplayNone)
                {
                    //ToolTip = null;
                    ToolTipMode = MarkerTooltipMode.Never;
                }
                else                 
                {
                    ToolTipMode = MarkerTooltipMode.Always;
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set 
            { 
                //if (_name == value) return;
                _name = value;
                if (ToolTip == null && !string.IsNullOrEmpty(value))
                {
                    ToolTip = new GMapHFBaloonToolTip(this);
                }
            }
        }

        private string _note;
        public string Note
        {
            get { return _note; }
            set 
            { 
                _note = value;
                if (ToolTip == null && !string.IsNullOrEmpty(value))
                {
                    ToolTip = new GMapHFBaloonToolTip(this);
                }
            }
        }

        public override string ToolTipText
        {
            get
            {
                if (BallonState == GeoBallonState.geoDisplayName)
                {
                    return Name;
                }
                else if (BallonState == GeoBallonState.geoDisplayBalloon)
                {
                    return Name + "\n" + Note;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        bool _highlight = false;

        public bool Highlight
        {
            get { return _highlight; }
            set
            {
                _highlight = value;
                if (Overlay != null && Overlay.Control != null)
                {
                    if (!Overlay.Control.HoldInvalidation)
                    {
                        Overlay.Control.Invalidate();
                    }
                }
            }
        }


        public GMarkerHFsystem(PointLatLng p, GMarkerHFsystemType type, int magnification = 1)
            : base(p)
        {
            Symbol = type;
            Offset = Point.Empty;

            if (type != GMarkerHFsystemType.none)
            {
                LoadBitmap(magnification);
            }
        }

        void LoadBitmap(int magnification)
        {
            Bitmap = GetIcon(Symbol.ToString());
            Size = new Size(Bitmap.Width*magnification + padding, Bitmap.Height*magnification + padding);
            Offset = Point.Empty;
        }

        /// <summary>
        ///     marker using manual bitmap, NonSerialized
        /// </summary>
        /// <param name="p"></param>
        /// <param name="bitmap"></param>
        public GMarkerHFsystem(PointLatLng p, Bitmap bitmap, int magnification = 1)
            : base(p)
        {
            Bitmap = bitmap;
            Size = new Size(bitmap.Width*magnification + padding, bitmap.Height* magnification + padding);
            Offset = Point.Empty;
        }

        static readonly Dictionary<string, Bitmap> IconCache = new Dictionary<string, Bitmap>();

        internal static Bitmap GetIcon(string name)
        {
            Bitmap ret;
            if (!IconCache.TryGetValue(name, out ret))
            {
                ret = Properties.Resources.ResourceManager.GetObject(name, Properties.Resources.Culture) as Bitmap;
                IconCache.Add(name, ret);
            }

            return ret;
        }

        public static int ID = 0;
        public void SetDefaultInfo()
        {
            Name = $"[ { ++ID}] DF";
            Note = $",, 21 000 000 Hz\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\n{Position.ToGeoString()}," ;
            BallonState = GeoBallonState.geoDisplayBalloon;
        }

        public void UpdateDefaultInfo()
        {
            Name = $"[ {ID}] DF";
            Note = $",, 21 000 000 Hz\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\n{Position.ToGeoString()},";
            BallonState = GeoBallonState.geoDisplayBalloon;
        }


        public override void OnRender(Graphics g)
        {
            if (Highlight)
            {   
                g.FillEllipse(Brushes.Yellow, LocalPosition.X - Size.Width, LocalPosition.Y - Size.Height, Size.Width*2, Size.Height*2);
            }   

            lock (Bitmap)
            {
                g.DrawImage(Bitmap, LocalPosition.X - Size.Width/2 + padding2, LocalPosition.Y- Size.Height/2 + padding2, Size.Width-padding, Size.Height-padding);
            }

            if (IsSelected)
                g.DrawRectangle(Pens.Black, LocalArea);

            //g.DrawString(LocalPosition.ToString(), SystemFonts.DefaultFont, Brushes.Red, LocalPosition);
        }

        public override void Dispose()
        {
            if (Bitmap != null)
            {
                if (!IconCache.ContainsValue(Bitmap))
                {
                    Bitmap.Dispose();
                    Bitmap = null;
                }
            }

            base.Dispose();
        }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", Symbol);
            info.AddValue("ballonstate", BallonState);  
            info.AddValue("Name", Name);
            info.AddValue("Note", Note);
            //info.AddValue("Bearing", this.Bearing);

            base.GetObjectData(info, context);
        }

        protected GMarkerHFsystem(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Symbol = Extensions.GetStruct(info, "type", GMarkerHFsystemType.none);
            BallonState = Extensions.GetStruct(info, "ballonstate", GeoBallonState.geoDisplayNone);
            Name = info.GetString("Name");
            Note = info.GetString("Note");
        }

        #endregion

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            if (Symbol != GMarkerHFsystemType.none)
            {
                LoadBitmap(1);
            }
        }

        #endregion
    }
}
