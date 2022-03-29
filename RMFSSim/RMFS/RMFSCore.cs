using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMFSSim.RMFS.Maps;


namespace RMFSSim.RMFS
{
    public class RMFSCore
    {
        public Map CurrentMap { get; }

        public RMFSCore(int mapWidth, int mapLength)
        {
            this.CurrentMap = new Map(mapWidth, mapLength);
        }

        public RMFSCore(string mapFilePath)
        {
            this.CurrentMap = new Map(mapFilePath);
        }
    }
}
