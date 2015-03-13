using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogProvider;
using LocalizationCore.BuildingModel;
using LocalizationCore.Primitives;

namespace LocalizationCore.PersonModel
{
    /// <summary>
    /// Responsible for making routes from one location (coord)
    /// to another
    /// </summary>
    public class RouteManager
    {
        const string DEBUG = "RouteManager";

        Queue<Coord> route = null;

        public RouteManager()
        {
        }
        
        private void addFinishPoint(Room finishRoom)
        {
            Coord theOnlyPoint = finishRoom.Area.Center;
            route.Enqueue(theOnlyPoint);
        }

        public Coord getNextTarget()
        {
            return route.Peek();
        }

        public bool isFinished()
        {
            return route.Count() == 0;
        }

        public void deletePoint()
        {
            route.Dequeue();
        }

        public void CreateDefaultRoute()
        {
            route = new Queue<Coord>();
                        
            //route.Enqueue(new Coord(1, 4));
            route.Enqueue(new Coord(1, 5));
            route.Enqueue(new Coord(24.5, 5));
            route.Enqueue(new Coord(24.5, 1));
            route.Enqueue(new Coord(20, 1));
        }

        public void IntelectualRoutes()
        {
            route = new Queue<Coord>();

            //route.Enqueue(new Coord(1, 4));
            route.Enqueue(new Coord(1, 5));
            route.Enqueue(new Coord(24.5, 5));
            route.Enqueue(new Coord(24.5, 1));
            route.Enqueue(new Coord(20, 1));

            // to WC
            route.Enqueue(new Coord(24.5, 3));
            route.Enqueue(new Coord(24.5, 5));
            route.Enqueue(new Coord(4, 5));
            route.Enqueue(new Coord(4, 2));

            // to home
            route.Enqueue(new Coord(4, 5));
            route.Enqueue(new Coord(1, 5));
            route.Enqueue(new Coord(1, 1));

            // to neighbour
            route.Enqueue(new Coord(1, 5));
            route.Enqueue(new Coord(5.5, 5));
            route.Enqueue(new Coord(5.5, 9));

            // to coridor
            route.Enqueue(new Coord(5.5, 5));
            route.Enqueue(new Coord(25, 5));
            route.Enqueue(new Coord(25, 9));
            route.Enqueue(new Coord(20, 9));
            route.Enqueue(new Coord(20, 5));

            // to home
            route.Enqueue(new Coord(1, 5));
            route.Enqueue(new Coord(1, 1));
        }
    }
}
