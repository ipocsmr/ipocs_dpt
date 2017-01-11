using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.IPOCS.Packets.Status
{
  public enum RQ_POINTS_STATE
  {
    RIGHT = 1,
    LEFT = 2,
    MOVING = 3,
    OUT_OF_CONTROL = 4
  };

  public enum RQ_RELEASE_STATE
  {
    LOCAL = 1,
    CENTRAL = 2,
    UNKNOWN = 3
  }

  public class Points : Packet
  {
    public static new byte RNID_PACKET { get { return 17; } }
    public RQ_POINTS_STATE RQ_POINTS_STATE;
    public RQ_RELEASE_STATE RQ_RELEASE_STATE;
    public short RT_OPERATION;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_POINTS_STATE = (RQ_POINTS_STATE)buffer[0];
      this.RQ_RELEASE_STATE = (RQ_RELEASE_STATE)buffer[1];
      this.RT_OPERATION = (short)((buffer[2] << 8) + buffer[3]);
      return 4;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)this.RQ_POINTS_STATE);
      buffer.Add((byte)this.RQ_RELEASE_STATE);
      buffer.Add((byte)(this.RT_OPERATION >> 8));
      buffer.Add((byte)(this.RT_OPERATION & 0xFF));
      return 4;
    }

    protected override string stringify()
    {
      return $"{RQ_POINTS_STATE.ToString()} -> {RQ_RELEASE_STATE.ToString()} -> {RT_OPERATION}";
    }

  }
}
