using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.IPOCS.Packets
{
  public class ApplicationData : Packet
  {
    public static new byte RNID_PACKET { get { return 5; } }
    public short RNID_XUSER;
    public byte[] PAYLOAD;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RNID_XUSER = (short)((buffer[0] << 8) + buffer[1]);
      this.PAYLOAD = buffer.Skip(2).ToArray();
      return (byte)(2 + PAYLOAD.Length);
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      var b = new List<byte>();
      b.Add((byte)(this.RNID_XUSER >> 8));
      b.Add((byte)(this.RNID_XUSER & 0xFF));
      b.AddRange(this.PAYLOAD);
      buffer.AddRange(b);
      return (byte)b.Count();
    }

    protected override string stringify()
    {
      return $"{RNID_XUSER.ToString("0:X4")} -> {BitConverter.ToString(this.PAYLOAD)}";
    }
  }
}
