using System.Collections.Generic;

namespace IPOCS.Protocol.Packets.Status
{
  public enum RQ_CONTROLLER_STATE
  {
    UNAVAILABLE = 1,
    RESTARTING = 2,
    OPERATIONAL = 3
  };

  public class Controller : Packet
  {
    public static new byte RNID_PACKET { get { return 15; } }
    public RQ_CONTROLLER_STATE RQ_CONTROLLER_STATE;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_CONTROLLER_STATE = (RQ_CONTROLLER_STATE)buffer[0];
      return 1;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)this.RQ_CONTROLLER_STATE);
      return 1;
    }

    protected override string stringify()
    {
      return $"{RQ_CONTROLLER_STATE.ToString()}";
    }
  }
}
