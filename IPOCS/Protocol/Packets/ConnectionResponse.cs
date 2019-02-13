using System.Collections.Generic;

namespace IPOCS.Protocol.Packets
{
  public class ConnectionResponse : Packet
  {
    public static new byte RNID_PACKET { get { return 2; } }

    public short RM_PROTOCOL_VERSION;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RM_PROTOCOL_VERSION = (short)((buffer[0] << 8) + buffer[1]);
      return 2;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)(this.RM_PROTOCOL_VERSION >> 8));
      buffer.Add((byte)(this.RM_PROTOCOL_VERSION & 0xFF));
      return 2;
    }

    protected override string stringify()
    {
      return $"{RM_PROTOCOL_VERSION.ToString("X4")}";
    }
  }
}
