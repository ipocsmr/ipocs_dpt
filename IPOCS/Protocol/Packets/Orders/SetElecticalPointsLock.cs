using System.Collections.Generic;

namespace IPOCS.Protocol.Packets.Orders
{
  public enum RQ_POINTS_LOCK_COMMAND
  {
    UNLOCK = 1,
    LOCK = 2
  };

  public class SetElecticalPointsLock : Packet
  {
    public static new byte RNID_PACKET { get { return 14; } }
    public RQ_POINTS_LOCK_COMMAND RQ_POINTS_LOCK_COMMAND;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_POINTS_LOCK_COMMAND = (RQ_POINTS_LOCK_COMMAND)buffer[0];
      return 1;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)this.RQ_POINTS_LOCK_COMMAND);
      return 1;
    }

    protected override string stringify()
    {
      return $"{RQ_POINTS_LOCK_COMMAND.ToString()}";
    }
  }
}
