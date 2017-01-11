using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.IPOCS.Packets
{
  public enum RQ_ACK
  {
    ACCEPTED = 0,
    REJECTED_UNSPECIFIED = 1,
    UNKNOWN_RECEIVER_IDENTITY = 2,
    UNKNOWN_PACKET_IDENTIFIER = 3,
    INVALID_PACKET_IDENTIFIER = 4,
    UNKNOWN_STATE = 5,
    LOCALLY_RELEASED = 6
  };

  public class Acknowledgement : Packet
  {
    public static new byte RNID_PACKET { get { return 4; } }
    public RQ_ACK RQ_ACK;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_ACK = (RQ_ACK)((buffer[0] << 8) + buffer[1]);
      return 2;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)((int)this.RQ_ACK >> 8));
      buffer.Add((byte)((int)this.RQ_ACK & 0xFF));
      return 2;
    }

    protected override string stringify()
    {
      return $"{RQ_ACK.ToString()}";
    }
  }
}
