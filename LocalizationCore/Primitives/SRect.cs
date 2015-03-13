using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;

namespace LocalizationCore.Primitives
{
    [Serializable]
    public class SRect
    {
        public Coord TopLeft { get; set; }
        public Coord RightBottom { get; set; }

        public SRect(Coord topLeft, Coord rightBottom)
        {
            TopLeft = topLeft;
            RightBottom = rightBottom;
        }

        public Coord TopRight
        {
            get { return new Coord(RightBottom.X, TopLeft.Y); }
        }

        public Coord BottomLeft
        {
            get { return new Coord(TopLeft.X, RightBottom.Y); }
        }

        public double Height
        {
            get { return RightBottom.Y - TopLeft.Y; }
        }

        public double Width
        {
            get { return RightBottom.X - TopLeft.X; }
        }

        public Coord Center
        {
            get { return new Coord((RightBottom.X + TopLeft.X) / 2, (RightBottom.Y + TopLeft.Y) / 2); }
        }

        public bool Contains(Coord coord)
        {
            return (coord.X >= TopLeft.X && coord.X <= RightBottom.X) &&
                   (coord.Y >= TopLeft.Y && coord.Y <= RightBottom.Y);
        }

        public static bool operator ==(SRect rect1, SRect rect2)
        {
            if ((object)rect1 == null || (object)rect2 == null)
                return PointerComparer.comparePointers(rect1, rect2);

            return rect1.TopLeft == rect2.TopLeft &&
                   rect1.TopRight == rect2.TopRight &&
                   rect1.RightBottom == rect2.RightBottom &&
                   rect1.BottomLeft == rect2.BottomLeft;
        }

        public static bool operator !=(SRect rect1, SRect rect2)
        {
            return !(rect1 == rect2);
        }
    }
}
