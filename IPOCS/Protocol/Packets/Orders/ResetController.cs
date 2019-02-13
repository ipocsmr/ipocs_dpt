using System.Collections.Generic;

namespace IPOCS.Protocol.Packets.Orders
{
  public class ResetController : Packet
  {
    public static new byte RNID_PACKET { get { return 6; } }

    protected override byte parseSpecific(List<byte> buffer)
    {
      return 0;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      return 0;
    }

    protected override string stringify()
    {
      return string.Empty;
    }
  }
}
