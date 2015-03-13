using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Primitives
{
    [Serializable]
    public class SPolygone
    {
        private SRect rect;

        public List<Coord> Vertexes { get; private set; }
        public Coord Center { get; private set; }

        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }

        public SPolygone()
        {
            Vertexes = new List<Coord>();

            InitGabarites();
        }

        public SPolygone(SRect rect)
        {
            Vertexes = new List<Coord>();
            Add(rect.TopLeft);
            Add(rect.TopRight);
            Add(rect.RightBottom);
            Add(rect.BottomLeft);

            InitGabarites();

            Center = rect.Center;
        }

        private void InitGabarites()
        {
            MaxX = double.MinValue;
            MinX = double.MaxValue;
            MaxY = double.MinValue;
            MinY = double.MaxValue;

            foreach (var v in Vertexes)
                CorrectGabarites(v);
        }

        public void Add(Coord vertex)
        {
            Vertexes.Add(vertex);
            CorrectGabarites(vertex);
        }

        private void CorrectGabarites(Coord vertex)
        {
            if (vertex.X >= MaxX) MaxX = vertex.X;
            if (vertex.X <= MinX) MinX = vertex.X;
            if (vertex.Y >= MaxY) MaxY = vertex.Y;
            if (vertex.Y <= MinY) MinY = vertex.Y;
        }

        private void CalculateCenter()
        {
            Coord res = new Coord(0, 0);
            foreach (var coord in Vertexes)
                res += coord;
            Center = new Coord(res.X / Vertexes.Count(), res.Y / Vertexes.Count);
        }

        /// <summary>
        /// Works only with rectangle polygones!!!!!!!
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public bool ContainsInsideRect(Coord coord)
        {
            if (coord.X <= MinX) return false;
            if (coord.X > MaxX) return false;
            if (coord.Y <= MinY) return false;
            if (coord.Y > MaxY) return false;

            return true;    // works only with rectangles
        }

        public IEnumerable<ILine> GetEdges()
        {
            var res = new List<ILine>();
            var startPoint = Vertexes.First();

            for (int i = 0; i < Vertexes.Count() - 1; i++)
                res.Add(new Line() { A = Vertexes[i], B = Vertexes[i + 1] });

            res.Add(new Line() { A = Vertexes.Last(), B = Vertexes.First() } );
            return res;
        }
    }
}
