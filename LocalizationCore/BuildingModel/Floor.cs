using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;
using LocalizationCore.PersonModel;
using LocalizationCore.SensorModel.Devices;

namespace LocalizationCore.BuildingModel
{
    public enum FloorInstance { Hospital = 1, Office = 2}

    public class Floor
    {
        public List<Room> Rooms { private set; get; }
        public List<Wall> Walls { private set; get; }
        public List<RoomWindow> Windows { private set; get; }

        public List<Device> Devices { private set; get; }

        public int Number { private set; get; }
        public double Width { get; set; }
        public double Height { get; set; }

        public FloorInstance ThisFloor { get; set; }

        public Floor(FloorInstance inst)
        {
            Rooms = new List<Room>();
            Walls = new List<Wall>();
            Devices = new List<Device>();
            Windows = new List<RoomWindow>();
        }

        public FloorKnowledge GetFloorKnowledge()
        {
            FloorKnowledge floor = new FloorKnowledge();
            floor.MealRoom = Building.Instance.Floor.getMealRoom();
            floor.ToiletRoom = Building.Instance.Floor.getToilet();
            floor.EntertainmentRoom = Building.Instance.Floor.getRoom("Game room");
            return floor;
        }

        public void addRoom(Room room)
        {
            Rooms.Add(room);
        }

        public void AddWall(Wall wall)
        {
            Walls.Add(wall);
        }

        public Room getRoom(Coord coord)
        {
            Room res = null;
            foreach (Room r in Rooms)
                if (r.Area.Contains(coord))
                {
                    res = r;
                    break;
                }
            if (res == null)
                throw new Exception("Cannot find room by point " + coord);
            return res;
        }

        public Room findRoom(Coord coord)
        {
            try
            {
                return getRoom(coord);
            }
            catch
            {
                return null;
            }
        }

        public Room getRoom(string name)
        {
            return Rooms.Where(r => r.Name == name).Single();
        }

        public Room getMealRoom()
        {
            return Rooms.Where(r => r.Type == RoomType.DinnerRoom).Single();
        }

        public Room getToilet()
        {
            return Rooms.Where(r => r.Type == RoomType.Toilet).First();
        }

        public void AddWindow(RoomWindow window)
        {
            Windows.Add(window);
        }

        public Wall IntersectWall(Coord a, Coord b)
        {            
            var walls = IntersectedWalls(a, b);

            if (walls.Count() == 0)
                return null;

            Wall nearest = walls.First();
            double distance = double.MaxValue;
            if (walls.Count() > 1)
                foreach (var w in walls)
                {
                    if ((Coord.Distance(a, w.A) + Coord.Distance(a, w.B) < distance))
                    {
                        nearest = w;
                        distance = Coord.Distance(a, w.A) + Coord.Distance(a, w.B);
                    }
                }

            return nearest;
        }

        public IEnumerable<Wall> IntersectedWalls(Coord a, Coord b)
        {
            List<Wall> res = new List<Wall>();

            foreach (var w in Walls)
                if (SVector.IsIntersection(a, b, w.A, w.B))
                    res.Add(w);

            return res;
        }
    }
}
