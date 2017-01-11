using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.IPOCS.Packets.Orders
{
  public enum RQ_RELEASE_COMMAND
  {
    LOCAL_CONTROL = 1,
    CENTRAL_CONTROL = 2
  }

  public class LocalRelease : Packet
  {
    public static new byte RNID_PACKET { get { return 9; } }
    public RQ_RELEASE_COMMAND RQ_RELEASE_COMMAND;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_RELEASE_COMMAND = (RQ_RELEASE_COMMAND)buffer[0];
      return 2;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)((int)this.RQ_RELEASE_COMMAND));
      return 2;
    }

    protected override string stringify()
    {
      return $"{RQ_RELEASE_COMMAND.ToString()}";
    }
  }
}
