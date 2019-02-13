using System.Collections.Generic;

namespace IPOCS.Protocol.Packets
{
  public enum RQ_DISCONNECT
  {
    WRONG_SITE_DATA_VERSION = 1,
    WRONG_PROTOCOL_VERSION = 2,
    WRONG_SENDER_IDENTITY = 3,
    WRONG_RECEIVER_IDENTITY = 4,
    UNIT_CLOSING_DOWN = 5
  };

  public class Disconnect : Packet
  {
    public static new byte RNID_PACKET { get { return 3; } }

    public RQ_DISCONNECT RQ_DISCONNECT;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_DISCONNECT = (RQ_DISCONNECT)((buffer[0] << 8) + buffer[1]);
      return 2;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)((int)this.RQ_DISCONNECT >> 8));
      buffer.Add((byte)((int)this.RQ_DISCONNECT & 0xFF));
      return 2;
    }

    protected override string stringify()
    {
      return $"{RQ_DISCONNECT.ToString()}";
    }
  }
}
