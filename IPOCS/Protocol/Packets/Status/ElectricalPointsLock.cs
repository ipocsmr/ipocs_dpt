using System.Collections.Generic;

namespace IPOCS.Protocol.Packets.Status
{
  public enum RQ_POINTS_LOCK_STATE
  {
    LOCKED_RIGHT = 1,
    LOCKED_LEFT = 2,
    UNLOCKED = 3,
    OUT_OF_CONTROL = 4
  };
    
  public class PointsLock : Packet
  {
    public static new byte RNID_PACKET { get { return 21; } }
    public RQ_POINTS_LOCK_STATE RQ_POINTS_LOCK_STATE;
    
    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_POINTS_LOCK_STATE = (RQ_POINTS_LOCK_STATE)buffer[0];
      return 1;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)this.RQ_POINTS_LOCK_STATE);
      return 1;
    }

    protected override string stringify()
    {
      return $"{RQ_POINTS_LOCK_STATE.ToString()}";
    }

  }
}
