using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMFSSim.RMFS.Maps
{
    public class MapNode
    {
        /// <summary>
        /// Enum of node types.
        /// </summary>
        public enum NodeTypes : byte
        {
            None = 0x00,
            PickStation,
            ReplenishmentStation,
            ChargeStation,
            Stroage,         
        }

        /// <summary>
        /// Length of bytes when converted to byte array.
        /// </summary>
        public const int RawBytesLength = 9;

        /// <summary>
        /// Location of node.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Type of node.
        /// </summary>
        public NodeTypes Type { get; set; }

        public MapNode(int x, int y, NodeTypes type = NodeTypes.None)
        {
            this.Location = new Location(x, y);
            this.Type = type;
        }

        public MapNode(byte[] byteArray, int offset = 0)
        {
            this.Type = (NodeTypes)byteArray[offset + 0];
            var location = new Location(BitConverter.ToInt32(byteArray, offset + 1), BitConverter.ToInt32(byteArray, offset + 5));
            this.Location = location;
        }

        public byte[] ToBytes()
        {
            byte[] byteArray = new byte[RawBytesLength];
            byteArray[0] = (byte)this.Type;
            Array.Copy(BitConverter.GetBytes(this.Location.X), 0, byteArray, 1, 4);
            Array.Copy(BitConverter.GetBytes(this.Location.Y), 0, byteArray, 5, 4);
            return byteArray;
        }

        public string Name
        {
            get { return $"{this.Type}_{this.Location.X.ToString("D3")}-{this.Location.Y.ToString("D3")}"; }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public bool IsNeighbourNode(MapNode node)
        {
            return Math.Abs(this.Location.X - node.Location.X) + Math.Abs(this.Location.Y - node.Location.Y) == 1;
        }
    }
}