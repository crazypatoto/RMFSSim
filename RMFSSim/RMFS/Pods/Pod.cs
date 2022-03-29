using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMFSSim.RMFS.Maps;

namespace RMFSSim.RMFS.Pods
{
    public class Pod
    {
        private float _heading = 0;
        public int ID { get; }
        public string Name { get; }
        public MapNode HomeNode { get; private set; }
        public MapNode CurrentNode { get; private set; }

        public float Heading
        {
            get
            {
                return _heading;
            }
            private set
            {
                _heading = value;
                _heading %= 360;
                if (_heading > 180) _heading -= 360;
                else if (_heading <= 180) _heading += 360;
            }
        }

        public Pod(int id, MapNode node, float heading = 0.0f, string name = null, MapNode homeNode = null)
        {
            this.ID = id;
            this.CurrentNode = node;
            this.Heading = heading;
            this.Name = name ?? $"Pod{id:D4}";
            this.HomeNode = homeNode ?? node;
        }
    }
}
