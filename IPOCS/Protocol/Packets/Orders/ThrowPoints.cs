using System.Collections.Generic;

namespace IPOCS.Protocol.Packets.Orders
{
  public enum RQ_POINTS_COMMAND
  {
    DIVERT_RIGHT = 1,
    DIVERT_LEFT = 2
  };

  public class ThrowPoints : Packet
  {
    public static new byte RNID_PACKET { get { return 10; } }
    public RQ_POINTS_COMMAND RQ_POINTS_COMMAND { get; set; }

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_POINTS_COMMAND = (RQ_POINTS_COMMAND)buffer[0];
      return 1;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)this.RQ_POINTS_COMMAND);
      return 1;
    }

    protected override string stringify()
    {
      return $"{RQ_POINTS_COMMAND.ToString()}";
    }
  }
}
