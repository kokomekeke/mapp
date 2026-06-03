using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMap.NET.WindowsForms
{

    public enum AnchorType
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public class GMapScaleLabel :GMapLabel
    {

        private AnchorType _anchor = AnchorType.Left;
        private int _offset = 5;                    // offset in pixels from side of map

        public AnchorType Anchor
        {
            get
            {
                return _anchor;
            }
            set
            {
                _anchor = value;
            }
        }


        public GMapScaleLabel()
            : base ()
        {

        }

        public GMapScaleLabel(PointLatLng pos, string text)
        : base(pos, text)   
        {
        }

        public GMapScaleLabel(PointLatLng pointLatLng, string text, Font font, AnchorType anchor)
            : base(pointLatLng, text, font)
        {
            _anchor = anchor;   
        }

        public override void UpdateLocalPosition()
        {
            if (Overlay != null && Overlay.Control != null)
            {
                Overlay.Control.UpdateScaleLabelLocalPosition(this);
                //UpdateGraphicsPath();
            }
        } /* UpdateLocalPosition */


        public override void OnRender(Graphics g)
        {
            try
            {
                var position = LocalPosition;
                switch (Anchor)
                {
                    case AnchorType.Left:
                        position = new Point(_offset, LocalPosition.Y);
                        break;
                    case AnchorType.Right:
                        position = new Point((int)g.VisibleClipBounds.Width - _offset, LocalPosition.Y);
                        break;
                    case AnchorType.Top:
                        position = new Point(LocalPosition.X, _offset);
                        break;
                    case AnchorType.Bottom:
                        position = new Point(LocalPosition.X, (int)g.VisibleClipBounds.Height - _offset);
                        break;
                }
                g.DrawString(Text, Font, Fill, position);
            }
            catch { }
            ;
        }



    } /* class GMapScaleLabel */
} /* namespace GMap.NET.WindowsForms */
