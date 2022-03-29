using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMFSSim.RMFS.AGVs;

namespace RMFSSim.RMFS.AGVs
{
    public abstract class AGV //: IEquatable<AGV>
    {
        /// <summary>
        /// AGV States Enumeration.
        /// </summary>
        [Flags]
        public enum AGVStates
        {
            Idle = 0,                           // AGV is doing nothing.
            Moving,                             // AGV is moving.
            WaitingToMove,                      // AGV is waiting to move.
            Rotating,                           // AGV is rotating.            
            RotatingPod,                        // AGV is rotating pod.
            WaitingToRotatePod,                 //AGV is waiting to rotate pod.
            LiftingPod,                         // AGV is lifting up a pod.
            DroppingPod,                        // AGV is dropping off a pod.
            DockingChargeStation,               // AGV is docking with charging station.
            UnDockingChargeStation,             // AGV is undocking with charging staiton.
            Charging,                           // AGV is charging.
            ChargeFinished,                     // AGV has finished charging.
            WaitingUserResume,                  // AGV is waiting for operator to resume.

            Disconnected = 100,                 // AGV is disconnected.
            MovingBlocked,                      // AGV is blocked by obstacle or other AGVs while moving.
            PodRotatingBlocked,                 // AGV is blocked by obstacle or other AGVs while rotating pod.
            UndefinedError = 200,               // AGV encounters undefinded error.
        }
        //public AGVHandler Handler { get; }
        //public AGVTaskHandler TaskHandler { get; }
        //public int ID { get; private protected set; }
        //public string Name { get; private protected set; }
        //public abstract MapNode CurrentNode { get; private protected set; }
        //public Rack BoundRack { get; private protected set; }
        //public AGVStates State { get; private protected set; }

        //public AGVHeading Heading
        //{
        //    get
        //    {
        //        return _heading;
        //    }
        //    private protected set
        //    {
        //        _heading = value;
        //        _heading = (AGVHeading)((int)_heading % 360);
        //        if ((int)_heading > 180) _heading -= 180;
        //        else if ((int)_heading <= -180) _heading += 180;
        //    }
        //}

        //protected AGV(AGVHandler handler)
        //{
        //    this.Handler = handler;
        //    this.TaskHandler = new AGVTaskHandler(this);
        //}
        //public abstract void StartNewPath(List<MapNode> path, AGVHeading? initialHeading = null);
        //public abstract void AddNodeToPath(MapNode node);
        //public abstract void EndPath();

        //public abstract void PickUpRack(Rack rack);
        //public abstract void DropOffRack();

        //public abstract void RotateRack(Rack.RackHeading rackHeading);

        //public abstract void WaitToRotateRack();

        //public abstract void WaitForUserResume();
        //public abstract void UserResume();
        //public abstract void Disconnect();
        //private AGVHeading _heading;

        //public bool Equals(AGV other)
        //{
        //    if (ReferenceEquals(null, other)) return false;
        //    if (ReferenceEquals(this, other)) return true;
        //    return string.Equals(this.Name, other.Name);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    if (obj.GetType() != this.GetType()) return false;
        //    return Equals((AGV)obj);
        //}

        //public static bool operator ==(AGV left, AGV right)
        //{
        //    return Equals(left, right);
        //}

        //public static bool operator !=(AGV left, AGV right)
        //{
        //    return !Equals(left, right);
        //}

        //public override int GetHashCode()
        //{
        //    return (this.Name != null ? this.Name.GetHashCode() : 0);
        //}

        //public override string ToString()
        //{
        //    return this.Name;
        //}
    }
}
