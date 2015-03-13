using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalizationCore.Primitives;
using LocalizationCore.SensorModel.Devices;

namespace LocalizationCore.BuildingModel
{
    public enum Side { South, North, West, East };

    public class FloorFactory
    {
        public Floor ResultFloor { private set; get; }

        public FloorFactory(FloorInstance inst, double width, double height)
        {
            ResultFloor = new Floor(inst);
            ResultFloor.Width = width;
            ResultFloor.Height = height;
        }

        #region Create Rooms
 

        public Room createHall(SRect area)
        {
            Room res = new Room("Hall", area);
            ResultFloor.addRoom(res);
            return res;
        }

        #endregion

        public Room CreateRoom(string name, SRect area, RoomType type)
        {
            Room res;
            if (type == RoomType.BedRoom)
                res = new PrivateRoom(name, area);
            else 
                res = new Room(name, area);

            res.Type = type;
            ResultFloor.addRoom(res);
            return res;
        }

        private Wall createWall(Coord begin, Coord end, bool door)
        {
            return new Wall(begin, end, door);
        }

        public void CreateWall(Coord coord1, Coord coord2)
        {
            ResultFloor.AddWall(createWall(coord1, coord2, false));
        }

        public void addDevice(Device device)
        {
            ResultFloor.Devices.Add(device);
        }

        public void CreateWindow(Coord coord1, Coord coord2)
        {
            ResultFloor.AddWindow(new RoomWindow(coord1, coord2));
        }
    }
}
