/*=============================================================================
*  MapPointToolTip : tooltip like tooltip in MapPoint
*  Notes:
*  pushpin Name id MapPoint is MapBaseMarker.TooltipHeader in this object
*  pushpit Note is MapBaseNumber.TooltipText in this object
*
* (c) Kótai 2026.
*
* ver 1.00                                                         14-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SgxMap.Shapes.ToolTips
{
    public class MapPointToolTip: MapToolTip, ISerializable
    {
        //public static readonly Pen DefaultStroke = new Pen(Color.FromArgb(140, Color.Navy));
        //[NonSerialized] public SolidBrush HeaderFill = new SolidBrush(Color.LightGray);


        static MapPointToolTip()
        {
            //DefaultStroke.Width = 3;

            //DefaultStroke.LineJoin = LineJoin.Round;
            //DefaultStroke.StartCap = LineCap.RoundAnchor;
        }

        public MapPointToolTip(MapBaseMarker marker)
            : base(marker)
        {
            Stroke = new Pen(Color.Black);
            Fill = new SolidBrush(Color.White);
            TextPadding = new Size(3, 3);
            //Foreground = new SolidBrush(Color.Black);
            TextPadding = new Size(2, 2);  // text padding
        }

        public override void OnRender(Graphics g)
        {
            if (string.IsNullOrEmpty(Marker.ToolTipText))
                return;

            var st = g.MeasureString(Marker.ToolTipHeader + "\n" + Marker.ToolTipText, TextFont).ToSize();
            var rect = new Rectangle(Marker.ToolTipPosition.X,
                Marker.ToolTipPosition.Y - st.Height,
                st.Width + 2*TextPadding.Width,
                st.Height + 4* TextPadding.Height);
            rect.Offset(Offset.X, Offset.Y);

            int lineOffset = 0;
            if (!g.VisibleClipBounds.Contains(rect))
            {
                var clippingOffset = new Point();
                if (rect.Right > g.VisibleClipBounds.Right)
                {
                    clippingOffset.X = -((rect.Left - Marker.LocalPosition.X) / 2 + rect.Width);
                    lineOffset = -rect.Width;
                }

                if (rect.Top < g.VisibleClipBounds.Top)
                {
                    clippingOffset.Y = ((rect.Bottom - Marker.LocalPosition.Y) + (rect.Height * 2));
                }

                rect.Offset(clippingOffset);
            }

            var p1 = new Point((int)(rect.X + +rect.Width / 8), (int)(rect.Y + rect.Height - 1));
            var p2 = new Point((int)(rect.X + rect.Width / 4), (int)(rect.Y + rect.Height - 1));
            var pnts = new Point[] { Marker.ToolTipPosition, p1, p2, Marker.ToolTipPosition };
            g.FillPolygon(Fill, pnts);

            g.DrawLine(Stroke, Marker.ToolTipPosition, p1);
            g.DrawLine(Stroke, Marker.ToolTipPosition, p2);

            g.FillRectangle(Fill, rect.X, rect.Y, rect.Width, rect.Height);
            g.DrawRectangle(Stroke, rect);

            int h = 0;
            if ((Marker.ToolTipShow == MarkerToolTipShow.Name) || (Marker.ToolTipShow != MarkerToolTipShow.Both))
            {
                st = g.MeasureString(Marker.ToolTipHeader, TextFont).ToSize();
                var hrect = new Rectangle(rect.X, rect.Y, rect.Width, st.Height + 2 * TextPadding.Height);
                g.FillRectangle(TitleFill, hrect);
                g.DrawRectangle(Stroke, hrect);
                var _hrect = new Rectangle(hrect.X + TextPadding.Width, hrect.Y + TextPadding.Height, hrect.Width - 2 * TextPadding.Width, hrect.Height - TextPadding.Height);
                WriteString(g, Marker.ToolTipHeader, _hrect);
                h = hrect.Height;
            }

            if (m_Format.Alignment == StringAlignment.Near)
                rect.Offset(TextPadding.Width / 2, 0);
            rect.Offset(0, TextPadding.Height / 2);


            if ((Marker.ToolTipShow == MarkerToolTipShow.Notes) || (Marker.ToolTipShow != MarkerToolTipShow.Both))
            {
                g.DrawRectangle(Stroke, rect.X, rect.Y, rect.Width, rect.Height);
                var _rect = new Rectangle(rect.X + TextPadding.Width, rect.Y + h + TextPadding.Height, rect.Width - 2 * TextPadding.Width, rect.Height - TextPadding.Height - h);
                WriteString(g, Marker.ToolTipText, _rect);
            }
        }



        #region ISerializable Members

        //void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    base.GetObjectData(info, context);
        //}

        //protected MapPointToolTip(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //}   

        #endregion

    } /* class MapPointToolTip */

} /* namespace SgxMap.Objects.ToolTips */

