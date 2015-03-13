using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LocalizationCore.Primitives;
using LocalizationCore.Algorithms;
using LocalizationCore.SensorModel.Devices;
using LocalizationCore.SensorModel.Sensors;

namespace LocalizationCore.BuildingModel
{
    public class Building
    {
        static Building inst = null;
                
        List<AreaLocation> _locations = new List<AreaLocation>();
        
        private Building(FloorInstance instance)
        {
            if (instance == FloorInstance.Hospital)
                Floor = CreateHospitalFloor();
            else
                Floor = CreateOfficeMap();
        }

        public static void CreateBuilding(FloorInstance instance)
        {
            inst = new Building(instance);
        }

        public static Building Instance
        {
            get
            {
                if (inst == null)
                    throw new Exception("Call CreateBuilding first");
                return inst;
            }
        }

        private static Floor CreateOfficeMap()
        {            
            FloorFactory factory = new FloorFactory(FloorInstance.Office, 63, 29);

            // main contour
            factory.CreateWall(new Coord(0, 9.45), new Coord(9.45, 0));
            factory.CreateWall(new Coord(9.45, 0), new Coord(63, 0));
            factory.CreateWall(new Coord(0, 9.45), new Coord(15.4, 23.8));            
            factory.CreateWall(new Coord(15.4, 23.8), new Coord(17.8, 21.3));
            factory.CreateWall(new Coord(17.8, 21.3), new Coord(63, 21.3));
            factory.CreateWall(new Coord(63, 21.3), new Coord(63, 0));

            factory.CreateWall(new Coord(8.50, 17.40), new Coord(11.2, 14.5));
            factory.CreateWall(new Coord(11.2, 14.5), new Coord(17.8, 21.3));

            // coridor
            factory.CreateWall(new Coord(15.4, 9.2), new Coord(63, 9.2));
            factory.CreateWall(new Coord(15.4, 11.6), new Coord(63, 11.6));
            
            factory.CreateWall(new Coord(15.4, 11.6), new Coord(15.4, 18.9));

            // LEFT SIDE
            // buhgalteria
            factory.CreateWall(new Coord(15.4, 9.2), new Coord(15.4, 0));
            factory.CreateWall(new Coord(15.4, 4.7), new Coord(20.7, 4.7));
            factory.CreateWall(new Coord(20.7, 9.2), new Coord(20.7, 0));

            // helios
            factory.CreateWall(new Coord(27.7, 9.2), new Coord(27.7, 0));
            factory.CreateWall(new Coord(22.2, 9.2), new Coord(22.2, 5.7));
            factory.CreateWall(new Coord(22.2, 5.7), new Coord(27.7, 5.7));

            // engeneers
            factory.CreateWall(new Coord(30.5, 0), new Coord(30.5, 5.7));
            factory.CreateWall(new Coord(27.7, 5.7), new Coord(33.3, 5.7));
            factory.CreateWall(new Coord(33.3, 9.2), new Coord(33.3, 0));
            
            // servers
            factory.CreateWall(new Coord(36.4, 9.2), new Coord(36.4, 0));
            // our room
            factory.CreateWall(new Coord(44.8, 9.2), new Coord(44.8, 0));

            // director
            factory.CreateWall(new Coord(50.4, 9.2), new Coord(50.4, 0));
            factory.CreateWall(new Coord(44.8, 6.4), new Coord(50.4, 6.4));

            // team d
            factory.CreateWall(new Coord(56.4, 9.2), new Coord(56.4, 0));
            //factory.CreateWall(new Coord(62.2, 9.2), new Coord(62.2, 0));

            // RIGHT SIDE
            factory.CreateWall(new Coord(20.7, 11.6), new Coord(20.7, 21.3));
            factory.CreateWall(new Coord(15.4, 14.1), new Coord(20.7, 14.1));   // secret
            factory.CreateWall(new Coord(19.2, 14.1), new Coord(19.2, 11.6));   // phone

            factory.CreateWall(new Coord(33.3, 11.6), new Coord(33.3, 21.3));   // engeners
            factory.CreateWall(new Coord(39.3, 11.6), new Coord(39.3, 21.3));   // 10

            // fire corridor
            factory.CreateWall(new Coord(33.3, 14.4), new Coord(35, 14.4));
            factory.CreateWall(new Coord(35, 11.6), new Coord(35, 14.4));

            factory.CreateWall(new Coord(44.8, 11.6), new Coord(44.8, 21.3));   // natalia
            factory.CreateWall(new Coord(50.4, 11.6), new Coord(50.4, 21.3));   // Kitchen
            // adresat
            factory.CreateWall(new Coord(56.4, 21.3), new Coord(56.4, 15.3));
            factory.CreateWall(new Coord(55, 15.3), new Coord(56.4, 15.3));
            factory.CreateWall(new Coord(55, 11.6), new Coord(55, 15.3));

            factory.CreateWall(new Coord(59.2, 11.6), new Coord(59.2, 21.3));   // warehous

            var res = factory.ResultFloor;
            res.ThisFloor = FloorInstance.Office;
            return res;
        }

        private static Floor CreateHospitalFloor()
        {
            FloorFactory factory = new FloorFactory(FloorInstance.Hospital, 26, 10);

            // Outside walls 
            factory.CreateWall(new Coord(0, 0), new Coord(26, 0));
            factory.CreateWall(new Coord(0, 0), new Coord(0, 10));
            factory.CreateWall(new Coord(0, 10), new Coord(26, 10));
            factory.CreateWall(new Coord(26, 0), new Coord(26, 10));

            // 1
            factory.CreateWall(new Coord(3, 0), new Coord(3, 4));
            factory.CreateWall(new Coord(0, 4), new Coord(0.2, 4));
            factory.CreateWall(new Coord(1, 4), new Coord(3.6, 4));
            factory.CreateWindow(new Coord(0.5, 0), new Coord(2.5, 0));
            // WC ladies
            factory.CreateWall(new Coord(5, 0), new Coord(5, 4));
            factory.CreateWall(new Coord(4.4, 4), new Coord(5.6, 4));
            factory.CreateWindow(new Coord(3.5, 0), new Coord(4.5, 0));
            // WC gents
            factory.CreateWall(new Coord(7, 0), new Coord(7, 4));
            factory.CreateWall(new Coord(6.4, 4), new Coord(7, 4));
            factory.CreateWindow(new Coord(5.5, 0), new Coord(6.5, 0));
            // Kitchen
            factory.CreateWall(new Coord(10, 0), new Coord(10, 4));
            factory.CreateWall(new Coord(7.8, 4), new Coord(15, 4));
            factory.CreateWindow(new Coord(7.5, 0), new Coord(9.5, 0));
            // Dining room
            factory.CreateWall(new Coord(18, 0), new Coord(18, 4));
            factory.CreateWall(new Coord(16, 4), new Coord(24, 4));            
            factory.CreateWindow(new Coord(10.5, 0), new Coord(12.5, 0));
            factory.CreateWindow(new Coord(13.5, 0), new Coord(15.5, 0));
            factory.CreateWindow(new Coord(16.5, 0), new Coord(17.5, 0));
            // Game room
            factory.CreateWall(new Coord(25, 4), new Coord(26, 4));
            factory.CreateWindow(new Coord(18.5, 0), new Coord(20.5, 0));
            factory.CreateWindow(new Coord(21.5, 0), new Coord(23.5, 0));
            factory.CreateWindow(new Coord(24.5, 0), new Coord(25.5, 0));
            // 2
            factory.CreateWall(new Coord(3, 6), new Coord(3, 10));
            factory.CreateWall(new Coord(0, 6), new Coord(2, 6));
            factory.CreateWindow(new Coord(0.5, 10), new Coord(2.5, 10));
            // 3
            factory.CreateWall(new Coord(6, 6), new Coord(6, 10));
            factory.CreateWall(new Coord(2.8, 6), new Coord(5, 6));
            factory.CreateWindow(new Coord(3.5, 10), new Coord(5.5, 10));
            // 4
            factory.CreateWall(new Coord(9, 6), new Coord(9, 10));
            factory.CreateWall(new Coord(5.8, 6), new Coord(8, 6));
            factory.CreateWindow(new Coord(6.5, 10), new Coord(8.5, 10));
            // 5
            factory.CreateWall(new Coord(12, 6), new Coord(12, 10));
            factory.CreateWall(new Coord(8.8, 6), new Coord(11, 6));
            factory.CreateWall(new Coord(11.8, 6), new Coord(14, 6));
            factory.CreateWindow(new Coord(9.5, 10), new Coord(11.5, 10));
            // Elevators
            factory.CreateWall(new Coord(14, 6), new Coord(14, 6.5));
            factory.CreateWall(new Coord(14, 7.5), new Coord(14, 8.5));
            factory.CreateWall(new Coord(14, 9.5), new Coord(14, 10));
            factory.CreateWall(new Coord(12, 8), new Coord(14, 8));

            // hall
            factory.CreateWindow(new Coord(14.5, 10), new Coord(16.5, 10));
            factory.CreateWindow(new Coord(17.5, 10), new Coord(19.5, 10));
            factory.CreateWindow(new Coord(20.5, 10), new Coord(22.5, 10));
            factory.CreateWindow(new Coord(23.5, 10), new Coord(25.5, 10));
            
            // rooms
            factory.CreateRoom("1", new SRect(new Coord(0, 0), new Coord(3, 4)), RoomType.BedRoom);
            factory.CreateRoom("ladies", new SRect(new Coord(3, 0), new Coord(5, 4)), RoomType.Toilet);
            factory.CreateRoom("gentelmens", new SRect(new Coord(5, 0), new Coord(7, 4)), RoomType.Toilet);
            factory.CreateRoom("Kitchen", new SRect(new Coord(7, 0), new Coord(10, 4)), RoomType.Kithen);
            factory.CreateRoom("Dining room", new SRect(new Coord(10, 0), new Coord(18, 4)), RoomType.DinnerRoom);
            factory.CreateRoom("Game room", new SRect(new Coord(18, 0), new Coord(26, 4)), RoomType.Other);
            factory.CreateRoom("2", new SRect(new Coord(0, 6), new Coord(3, 10)), RoomType.BedRoom);
            factory.CreateRoom("3", new SRect(new Coord(3, 6), new Coord(6, 10)), RoomType.BedRoom);
            factory.CreateRoom("4", new SRect(new Coord(6, 6), new Coord(9, 10)), RoomType.BedRoom);
            factory.CreateRoom("5", new SRect(new Coord(9, 6), new Coord(12, 10)), RoomType.BedRoom);            
            factory.CreateRoom("Lift", new SRect(new Coord(12, 6), new Coord(15, 8)), RoomType.Other);
            factory.CreateRoom("Steps", new SRect(new Coord(12, 8), new Coord(15, 10)), RoomType.Other);
            factory.CreateRoom("Coridor", new SRect(new Coord(0, 4), new Coord(15, 6)), RoomType.Coridor);
            factory.createHall(new SRect(new Coord(15, 4), new Coord(26, 10)));

            // access points and other receivers
            factory.addDevice(new WifiAccessPoint("WLAN1", new Coord(16, 0)));
            factory.addDevice(new WifiAccessPoint("WLAN2", new Coord(26, 0)));
            factory.addDevice(new WifiAccessPoint("WLAN3", new Coord(14, 8)));
            factory.addDevice(new WifiAccessPoint("WLAN4", new Coord(26, 10)));
            factory.addDevice(new WifiAccessPoint("WLAN5", new Coord(7, 4)));

            factory.addDevice(new RfidReader("RFID1", new Coord(0, 0)));
            factory.addDevice(new RfidReader("RFID2", new Coord(5, 0)));
            factory.addDevice(new RfidReader("RFID3", new Coord(0, 10)));
            factory.addDevice(new RfidReader("RFID4", new Coord(5, 10)));

            var res = factory.ResultFloor;
            res.ThisFloor = FloorInstance.Hospital;
            return res;
        }
        
        public Floor Floor { get; set; }

        public Room getRoom(Coord coord)
        {
            return Floor.getRoom(coord);
        }

        public AreaLocation getLocation(Coord c)
        {
            Room room = Building.Instance.getRoom(c);
            SRect area = room.Area;
            AreaLocation loc = findLocation(room.Name);
            if (loc == null)
            {
                loc = new AreaLocation(area, room.Name);
                _locations.Add(loc);
            }
            return loc;
        }

        private AreaLocation findLocation(string name)
        {
            var loc = _locations.Where(l => l.Name == name);
            if (loc.Count() != 0)
                return loc.Single();
            else
                return null;
        }
    }
}
