using System.Collections.Generic;

namespace IPOCS.Protocol.Packets.Orders
{
  public class SignOfLifeTimer : Packet
  {
    public static new byte RNID_PACKET { get { return 8; } }

    public short RT_INTERVAL;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RT_INTERVAL = (short)((buffer[0] << 8) + buffer[1]);
      return 2;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)(this.RT_INTERVAL >> 8));
      buffer.Add((byte)(this.RT_INTERVAL & 0xFF));
      return 2;
    }

    protected override string stringify()
    {
      return $"{RT_INTERVAL.ToString()}";
    }
  }
}
