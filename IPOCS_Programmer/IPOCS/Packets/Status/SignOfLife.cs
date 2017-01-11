using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.IPOCS.Packets.Status
{
  public class SignOfLife : Packet
  {
    public static new byte RNID_PACKET { get { return 23; } }

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
