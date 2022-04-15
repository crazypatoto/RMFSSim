using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMFSSim.RMFS.Maps
{
    public class Map
    {
        /// <summary>
        /// File path of the map.
        /// </summary>
        public string FilePath { get; private set; }
        /// <summary>
        /// Serial Number of the map.
        /// </summary>
        public string SerialNumber { get; }

        /// <summary>
        /// Width of the map.
        /// </summary>
        public int Width { get; }
        /// <summary>
        /// Length of the map.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// All nodes of the map. [length, width]
        /// </summary>
        public MapNode[,] AllNodes { get; }
        /// <summary>
        /// All edges of the map. (Toltal count = (2 * length - 1) * width)
        /// </summary>
        public MapEdge[,] AllEdges { get; }      

        public Map(int width, int length, string sn = null)
        {
            this.FilePath = null;
            this.Width = width;
            this.Length = length;
            if (sn == null)
            {
                this.SerialNumber = Guid.NewGuid().ToString();
            }
            else
            {
                this.SerialNumber = sn;
            }

            this.AllNodes = new MapNode[length, width];
            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    this.AllNodes[y, x] = new MapNode(x, y);
                }
            }

            this.AllEdges = new MapEdge[2 * length - 1, width];
            for (int y = 0; y < 2 * length - 1; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    this.AllEdges[y, x] = new MapEdge();
                }
            }
        }
        public Map(string path)
        {
            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
            {
                this.FilePath = path;
                this.SerialNumber = binaryReader.ReadString();
                this.Width = binaryReader.ReadInt32();
                this.Length = binaryReader.ReadInt32();
                this.AllNodes = new MapNode[this.Length, this.Width];
                for (int y = 0; y < this.Length; y++)
                {
                    for (int x = 0; x < this.Width; x++)
                    {
                        this.AllNodes[y, x] = new MapNode(binaryReader.ReadBytes(MapNode.RawBytesLength));
                    }
                }

                this.AllEdges = new MapEdge[2 * this.Length - 1, this.Width];
                for (int y = 0; y < 2 * this.Length - 1; y++)
                {
                    for (int x = 0; x < this.Width; x++)
                    {
                        this.AllEdges[y, x] = new MapEdge(binaryReader.ReadBytes(MapEdge.RawBytesLength));
                    }
                }
            }
        }

        public void Save()
        {
            if (this.FilePath != null)
            {
                SaveToFile(this.FilePath);
            }
        }
        public void SaveToFile(string path)
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                this.FilePath = path;
                binaryWriter.Write(this.SerialNumber);
                binaryWriter.Write(this.Width);
                binaryWriter.Write(this.Length);
                for (int y = 0; y < this.Length; y++)
                {
                    for (int x = 0; x < this.Width; x++)
                    {
                        binaryWriter.Write(this.AllNodes[y, x].ToBytes());
                    }
                }

                for (int y = 0; y < 2 * this.Length - 1; y++)
                {
                    for (int x = 0; x < this.Width; x++)
                    {
                        binaryWriter.Write(this.AllEdges[y, x].ToBytes());
                    }
                }
            }
        }

        public MapEdge GetEdgeByNodes(MapNode n1, MapNode n2)
        {
            return this.AllEdges[n1.Location.Y + n2.Location.Y, (n1.Location.X + n2.Location.X) / 2];
        }

        public Tuple<int, int> CoordinatesOfEdge(MapEdge edge)
        {
            int w = this.AllEdges.GetLength(0); // width
            int l = this.AllEdges.GetLength(1); // lenght

            for (int y = 0; y < w; ++y)
            {
                for (int x = 0; x < l; ++x)
                {
                    if (this.AllEdges[y, x].Equals(edge))
                        return Tuple.Create(x, y);
                }
            }

            return Tuple.Create(-1, -1);
        }

        public List<MapNode> GetNeighborNodes(MapNode node)
        {
            List<MapNode> neighborNodes = new List<MapNode>();
            if (node.Location.X - 1 >= 0) neighborNodes.Add(this.AllNodes[node.Location.Y, node.Location.X - 1]);
            if (node.Location.Y - 1 >= 0) neighborNodes.Add(this.AllNodes[node.Location.Y - 1, node.Location.X]);
            if (node.Location.X + 1 < this.Width) neighborNodes.Add(this.AllNodes[node.Location.Y, node.Location.X + 1]);
            if (node.Location.Y + 1 < this.Length) neighborNodes.Add(this.AllNodes[node.Location.Y + 1, node.Location.X]);
            return neighborNodes;
        }
    }
}
