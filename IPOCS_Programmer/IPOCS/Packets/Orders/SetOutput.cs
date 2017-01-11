using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.IPOCS.Packets.Orders
{
  public enum RQ_OUTPUT_COMMAND
  {
    ON = 1,
    OFF = 2
  };

  public class SetOutput : Packet
  {
    public static new byte RNID_PACKET { get { return 13; } }
    public RQ_OUTPUT_COMMAND RQ_OUTPUT_COMMAND;
    public short RT_DURAION;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_OUTPUT_COMMAND = (RQ_OUTPUT_COMMAND)buffer[0];
      this.RT_DURAION = (short)((buffer[1] << 8) + buffer[2]);
      return 3;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)this.RQ_OUTPUT_COMMAND);
      buffer.Add((byte)(this.RT_DURAION >> 8));
      buffer.Add((byte)(this.RT_DURAION & 0xFF));
      return 3;
    }

    protected override string stringify()
    {
      return $"{RQ_OUTPUT_COMMAND.ToString()} -> {this.RT_DURAION.ToString()}";
    }
  }
}
