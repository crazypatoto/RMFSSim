using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMFSSim.RMFS.Maps
{
    public class Location : IEquatable<Location>
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Location(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public Location(Location location)
        {
            this.X = location.X;
            this.Y = location.Y;
        }

        public override string ToString()
        {
            return $"({this.X},{this.Y})";
        }

        public bool Equals(Location other)
        {
            if(other.X == this.X && other.Y == this.Y)
            {
                return true;
            }
            return false;
        }
    }
}
