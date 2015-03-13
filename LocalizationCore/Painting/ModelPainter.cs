using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.BuildingModel;
using System.Drawing;
using LocalizationCore.Primitives;
using LocalizationCore.PersonModel;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Windows.Media;
using LocalizationCore.SensorModel.Devices;
using LocalizationCore;
using LocalizationCore.Localization.Locations;
using System.Windows;
using LocalizationCore.Localization;
using LocalizationCore.Painting;
using LocalizationCore.Localization.Filtering;

namespace LocalizationCore
{
    public class ModelPainter
    {
        const int PIXEL_OFFSET = 10;
        const double PERSON_RADIUS = 0.3;
        const int WINDOW_THICKNESS = 5;

        //public Building Building { private set; get; }
        public Floor Floor { private set; get; }

        public Painter Painter { private set; get; }

        Observations lastSelected = null;

        Color wallColor;
        Color doorColor;
        Color personColor;
        //Color eraceColor;      

        public double PixelsInMeter { private set; get; }

        Model model;

        public ModelPainter(Model model, Floor floor, Canvas canvas, double pixelsInMeter = 0)
        {
            this.Floor = floor;
            this.Painter = new Painter(canvas);
            this.model = model;
            personColor = Colors.Lime;
            //eraceColor = Colors.Maroon;
            wallColor = Colors.Black;
            doorColor = Colors.Brown;
            if (pixelsInMeter != 0)
                this.PixelsInMeter = pixelsInMeter;
            else
                this.PixelsInMeter = calcPixelsInMeter(canvas);
        }

        #region Interface               

        public void Clear()
        {
            Painter.Clear();
        }

        public Coord ToCoord(System.Windows.Point point)
        {
            return new Coord((point.X - PIXEL_OFFSET) / PixelsInMeter, (point.Y - PIXEL_OFFSET) / PixelsInMeter);            
        }

        public double ToScreen(double real)
        {
            return (real * PixelsInMeter);
        }

        public System.Windows.Point ToPoint(Coord coord)
        {
            var res = new System.Windows.Point();
            res.X = (int)(coord.X * PixelsInMeter) + PIXEL_OFFSET;
            res.Y = (int)(coord.Y * PixelsInMeter) + PIXEL_OFFSET;
            return res;
        }

        public void DrawCoord(Coord coord)
        {
            this.Painter.DrawCircle(1, ToPoint(coord), personColor, 1);
            //Canvas.DrawEllipse(_personPen, new Rectangle(p.X - 1, p.Y - 1, 3, 3));
        }

        public void DrawLine(Coord oldCoord, Coord newCoord, Color color)
        {
            Painter.DrawLine(ToPoint(oldCoord), ToPoint(newCoord), color, 1);
        }

        public void DrawFilledRect(Coord coord1, Coord coord2, Color color)
        {
            Painter.DrawRectangle(ToPoint(coord1), ToPoint(coord2), color, 1, true);
        }

        /// <summary>
        /// Draw locations grid.
        /// Depending of default parameters shows signal strength map and selected location.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="device">To show signal strength map (if not null)</param>
        /// <param name="selected">To show selected location</param>
        public void DrawObservations(RadioMap map, Device device = null, Observations selected = null, bool redraw = true)
        {
            if (redraw)
            {
                foreach (var loc in map)
                    DrawObservations(loc, device);
            }
                        
            if (selected != null)
            {
                Draw(selected.LocationLink, Colors.Violet, Colors.Green);                
                if (lastSelected != null)
                    DrawObservations(lastSelected, device);
                lastSelected = selected;
            }
        }

        private void DrawObservations(Observations loc, Device device)
        {
            if (device != null)
                RssLocationDraw(loc, device);
            else
                GeneralLocationDraw(loc);
        }

        private void GeneralLocationDraw(Observations loc)
        {
            Color color;
            if (loc.Fingerprints.Count() > 10)
                color = Colors.Green;
            else if (loc.Fingerprints.Count() > 0)
                color = Colors.LightGreen;
            else
                color = Colors.White;

            Draw(loc.LocationLink, color, Colors.Gray);
        }

        private void RssLocationDraw(Observations loc, Device device)
        {
            double val = -loc.GetValue(device);
            Color color = val == 0 ? Colors.White : LocalizationCore.Painter.GetGradient(val, 30, 100);

            Draw(loc.LocationLink, color, Colors.Gray);
        }

        public void Draw(Location loc, Color fillColor, Color borderColor)
        {
            Painter.DrawPolygone(toPointList(loc.Polygone.Vertexes), fillColor, borderColor);
        }

        public void DrawFloor()
        {
            //Clear();
            //DrawRooms();            
            DrawWindows();
            DrawWalls();
            
            DrawBeacons();            
        }

        private void DrawWindows()
        {
            foreach (var window in Floor.Windows)
                DrawWindow(window);
        }

        private void DrawWindow(RoomWindow window)
        {
            Painter.DrawLine(ToPoint(window.Start), ToPoint(window.End), Colors.Cyan, WINDOW_THICKNESS);
        }

        private void DrawBeacons()
        {
            foreach (Device d in Floor.Devices)
                if (d is WifiAccessPoint)
                    drawAccessPoint((AccessPoint)d, Colors.Blue);
                else
                    drawAccessPoint((AccessPoint)d, Colors.Red);
        }

        private void DrawWalls()
        {
            foreach (Wall w in Floor.Walls)
                drawWall(w);
        }

        private void DrawRooms()
        {
            foreach (Room r in Floor.Rooms)
                drawRoom(r);
        }

        public void DrawParticles(List<Particle> list)
        {
            foreach (var p in list)
                DrawParticle(p);
        }

        private void DrawParticle(Particle p)
        {
            if (p.Weight < 0)
                p.Weight = 0;
            var radius = (int)(p.Weight * 20) + 2;
            if (radius < 0)
                radius = 1;
            Painter.DrawCircle(radius, ToPoint(p.Coord), Colors.Black, 1);
        }

        public void DrawPerson(Coord coord, SVector dir, Color color)
        {
            double radius = 0.5;
            int thickness = 3;
            Painter.DrawCircle(ToScreen(radius), ToPoint(coord), color, thickness);
            Painter.DrawLine(ToPoint(coord), ToPoint(coord + dir.Normalize(radius)), color, thickness);
        }

        public void DrawEstPerson(Coord coord, SVector dir, Color color)
        {
            double radius = 0.2;
            int thickness = 5;
            Painter.DrawCircle(ToScreen(radius), ToPoint(coord), color, thickness);            
        }

        public void DrawRoute(List<Coord> route, Color color)
        {
            for (int i = 1; i < route.Count; i++)
                Painter.DrawLine(ToPoint(route[i - 1]), ToPoint(route[i]), color, 1);
        }

        public void DrawPdf(Pdf pdf, int limit)
        {
            if (pdf == null) return;

            foreach (var p in pdf.GetLocations())
                Draw(p.Key, Painter.GetGradient(p.Value * 100, 0, limit), Colors.White);
        }

        #endregion

        #region Private

        private double calcPixelsInMeter(Canvas canvas)
        {
            var vert = (canvas.Height - PIXEL_OFFSET * 2) / Floor.Height;
            var hor = (canvas.Width - PIXEL_OFFSET * 2) / Floor.Width;
            return vert > hor ? hor : vert;
        }

        private void drawAccessPoint(AccessPoint accessPoint, Color color)
        {
            Painter.DrawCircle(5, ToPoint(accessPoint.Coordinate), color, 4);
            drawCross(accessPoint.Coordinate, color, 1);
            drawPlus(accessPoint.Coordinate, color, 1);
        }

        private void drawPlus(Coord coord, Color color, int thickness)
        {
            double d = 0.2;
            Painter.DrawLine(ToPoint(new Coord(coord.X - d, coord.Y)), ToPoint(new Coord(coord.X + d, coord.Y)), color, thickness);
            Painter.DrawLine(ToPoint(new Coord(coord.X, coord.Y - d)), ToPoint(new Coord(coord.X, coord.Y + d)), color, thickness);
        }

        private void drawCross(Coord coord, Color color, int thickness)
        {
            double d = 0.2;
            Painter.DrawLine(ToPoint(new Coord(coord.X - d, coord.Y - d)),
                             ToPoint(new Coord(coord.X + d, coord.Y + d)), color, thickness);
            Painter.DrawLine(ToPoint(new Coord(coord.X - d, coord.Y + d)),
                             ToPoint(new Coord(coord.X + d, coord.Y - d)), color, thickness);
        }

        private void drawPeople(IEnumerable<Person> people)
        {
            foreach (Person p in people)
            {                
                //if (_history.Keys.Contains(p.Name))
                //    drawPerson(_history[p.Name], _erasePen);
                drawPerson(p.Coordinate);
                //_history[p.Name] = p.State.Coordinate.clone();
            }
        }

        private void drawPerson(Coord coord)
        {
            this.Painter.DrawCircle(PERSON_RADIUS * PixelsInMeter, ToPoint(coord), personColor, 1);

            this.Painter.DrawLine(ToPoint(new Coord(coord.X, coord.Y + PERSON_RADIUS)),
                                  ToPoint(new Coord(coord.X, coord.Y - PERSON_RADIUS)), personColor, 1);
            this.Painter.DrawLine(ToPoint(new Coord(coord.X - PERSON_RADIUS, coord.Y)),
                                  ToPoint(new Coord(coord.X + PERSON_RADIUS, coord.Y)), personColor, 1);
        }

        private void drawWall(Wall wall)
        {
            this.Painter.DrawLine(ToPoint(wall.Begin), ToPoint(wall.End), wallColor, 1);
            if (wall.Door != null)
                drawDoor(wall);
        }

        private void drawDoor(Wall wall)
        {
            this.Painter.DrawLine(ToPoint(wall.Door.Begin), ToPoint(wall.Door.End), doorColor, 3);
        }

        private void drawRoom(Room room)
        {
            Painter.DrawRectangle(ToPoint(room.Area.TopLeft), ToPoint(room.Area.RightBottom), Colors.Black, 1, false);
            if (room.Type == RoomType.BedRoom)
                drawBed(((PrivateRoom)room).BedLocation);
        }

        private void drawBed(SRect rect)
        {
            this.Painter.DrawRectangle(ToPoint(rect.TopLeft), ToPoint(rect.RightBottom), wallColor, 1);

            // pillow
            Coord pillowCenter = new Coord(rect.Center.X, rect.Center.Y - rect.Height / 3);
            double pillowSize_2 = 0.2;
            SRect pillow = new SRect(new Coord(pillowCenter.X - pillowSize_2, pillowCenter.Y - pillowSize_2),
                                   new Coord(pillowCenter.X + pillowSize_2, pillowCenter.Y + pillowSize_2));
            this.Painter.DrawRectangle(ToPoint(rect.TopLeft), ToPoint(rect.RightBottom), wallColor, 1);
        }

        private List<Point> toPointList(List<Coord> list)
        {
            List<Point> res = new List<Point>();
            foreach (var coord in list)
                res.Add(ToPoint(coord));
            return res;
        }

        #endregion


        
    }
}
