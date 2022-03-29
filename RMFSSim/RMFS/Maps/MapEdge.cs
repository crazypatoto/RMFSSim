using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMFSSim.RMFS.Maps
{
    public class MapEdge
    {
        [Flags]
        public enum Directions : byte
        {
            None = 0b0000,
            Leaving = 0b0001,
            Entering = 0b0010,
            LeavingAndEntering = 0b0011
        }

        public const int RawBytesLength = 5;

        /// <summary>
        /// Direcion of the edge.
        /// </summary>
        public Directions Direction { get; set; }

        /// <summary>
        /// Speed limit when going through the edge. (m/s)
        /// </summary>
        public float SpeedLimit { get; set; }
        public MapEdge(Directions direction = Directions.LeavingAndEntering, float speedLimit = 0.0f)
        {
            this.Direction = direction;
            this.SpeedLimit = speedLimit;
        }

        public MapEdge(byte[] byteArray, int offset = 0)
        {
            this.Direction = (Directions)byteArray[offset + 0];
            this.SpeedLimit = BitConverter.ToSingle(byteArray, offset + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] byteArray = new byte[RawBytesLength];
            byteArray[0] = (byte)this.Direction;
            Array.Copy(BitConverter.GetBytes(this.SpeedLimit), 0, byteArray, 1, 4);
            return byteArray;
        }


        /// <summary>
        /// Reverse the direction of current edge;
        /// </summary>
        public void ReverseDirecion()
        {
            int direction = (int)this.Direction;
            direction = (direction ^ 0b0011) & 0b0011;
            this.Direction = (Directions)direction;
        }
    }
}
