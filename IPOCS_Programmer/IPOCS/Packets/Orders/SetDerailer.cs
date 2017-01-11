using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.IPOCS.Packets.Orders
{
  public enum RQ_DERAILER_COMMAND
  {
    DIVERT_RIGHT = 1,
    DIVERT_LEFT = 2
  };

  public class SetDerailer : Packet
  {
    public static new byte RNID_PACKET { get { return 11; } }
    public RQ_DERAILER_COMMAND RQ_DERAILER_COMMAND;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_DERAILER_COMMAND = (RQ_DERAILER_COMMAND)buffer[0];
      return 1;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)this.RQ_DERAILER_COMMAND);
      return 1;
    }

    protected override string stringify()
    {
      return $"{RQ_DERAILER_COMMAND.ToString()}";
    }
  }
}
