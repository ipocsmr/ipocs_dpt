using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.IPOCS.Packets
{
  public class ConnectionRequest : Packet
  {
    public static new byte RNID_PACKET { get { return 1; } }

    public short RM_PROTOCOL_VERSION;
    public string RXID_SITE_DATA_VERSION;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RM_PROTOCOL_VERSION = (short)((buffer[0] << 8) + buffer[1]);
      byte ll = 0;
      for (int i = 2; i < buffer.Count && buffer[ll] != 0x00; i++)
      {
        ll++;
      }
      this.RXID_SITE_DATA_VERSION = Encoding.UTF8.GetString(buffer.ToArray(), 2, ll - 2);
      ll++;
      return (byte)(2 + ll);
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      var b = new List<byte>();
      b.Add((byte)(this.RM_PROTOCOL_VERSION >> 8));
      b.Add((byte)(this.RM_PROTOCOL_VERSION & 0xFF));
      b.AddRange(Encoding.UTF8.GetBytes(this.RXID_SITE_DATA_VERSION));
      b.Add(0x00);
      buffer.AddRange(b);
      return (byte)b.Count();
    }

    protected override string stringify()
    {
      return $"{RM_PROTOCOL_VERSION.ToString("X4")} -> Site Data: {RXID_SITE_DATA_VERSION}";
    }
  }
}
