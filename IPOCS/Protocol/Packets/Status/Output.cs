using System.Collections.Generic;

namespace IPOCS.Protocol.Packets.Status
{
  public enum RQ_OUTPUT_STATE
  {
    ON = 1,
    OFF = 2
  };
    
  public class Output : Packet
  {
    public static new byte RNID_PACKET { get { return 22; } }
    public RQ_OUTPUT_STATE RQ_OUTPUT_STATE;
    
    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_OUTPUT_STATE = (RQ_OUTPUT_STATE)buffer[0];
      return 1;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)this.RQ_OUTPUT_STATE);
      return 1;
    }

    protected override string stringify()
    {
      return $"{RQ_OUTPUT_STATE.ToString()}";
    }

  }
}
