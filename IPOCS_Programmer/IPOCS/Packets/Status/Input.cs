using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.IPOCS.Packets.Status
{
  public enum RQ_INPUT_STATE
  {
    ON = 1,
    OFF = 2,
    UNDEFINED = 3
  };
    
  public class Input : Packet
  {
    public static new byte RNID_PACKET { get { return 20; } }
    public RQ_INPUT_STATE RQ_INPUT_STATE;
    
    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_INPUT_STATE = (RQ_INPUT_STATE)buffer[0];
      return 1;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)this.RQ_INPUT_STATE);
      return 1;
    }

    protected override string stringify()
    {
      return $"{RQ_INPUT_STATE.ToString()}";
    }

  }
}
