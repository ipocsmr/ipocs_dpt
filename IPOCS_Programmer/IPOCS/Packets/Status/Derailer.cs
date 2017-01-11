using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.IPOCS.Packets.Status
{
  public enum RQ_DERAILER_STATE
  {
    PASSABLE = 1,
    NON_PASSABLE = 2,
    MOVING = 3,
    OUT_OF_CONTROL = 4
  };
  
  public class Derailer : Packet
  {
    public static new byte RNID_PACKET { get { return 18; } }
    public RQ_DERAILER_STATE RQ_DERAILER_STATE;
    public RQ_RELEASE_STATE RQ_RELEASE_STATE;
    public short RT_OPERATION;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_DERAILER_STATE = (RQ_DERAILER_STATE)buffer[0];
      this.RQ_RELEASE_STATE = (RQ_RELEASE_STATE)buffer[1];
      this.RT_OPERATION = (short)((buffer[2] << 8) + buffer[3]);
      return 4;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)this.RQ_DERAILER_STATE);
      buffer.Add((byte)this.RQ_RELEASE_STATE);
      buffer.Add((byte)(this.RT_OPERATION >> 8));
      buffer.Add((byte)(this.RT_OPERATION & 0xFF));
      return 4;
    }

    protected override string stringify()
    {
      return $"{RQ_DERAILER_STATE.ToString()} -> {RQ_RELEASE_STATE.ToString()} -> {RT_OPERATION}";
    }
  }
}
