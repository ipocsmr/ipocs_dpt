using System.Collections.Generic;

namespace IPOCS.Protocol.Packets.Orders
{
  public enum RQ_LEVEL_CROSSING_COMMAND
  {
    OPEN_NOW = 1,
    OPEN_AFTER_PASSAGE = 2,
    CLOSE = 3,
    ACTIVATE_REDUCED_AUTOMATION = 4,
    DEACTIVATE_REDUCED_AUTOMATION = 5
  };

  public class SetLevelCrossing : Packet
  {
    public static new byte RNID_PACKET { get { return 12; } }
    public RQ_LEVEL_CROSSING_COMMAND RQ_LEVEL_CROSSING_COMMAND;
    public short RT_DELAY;
    public byte RNID_TRACK;

    protected override byte parseSpecific(List<byte> buffer)
    {
      this.RQ_LEVEL_CROSSING_COMMAND = (RQ_LEVEL_CROSSING_COMMAND)buffer[0];
      this.RT_DELAY = (short)((buffer[1] << 8) + buffer[2]);
      this.RNID_TRACK = buffer[3];
      return 4;
    }

    protected override byte serializeSpecific(List<byte> buffer)
    {
      buffer.Add((byte)this.RQ_LEVEL_CROSSING_COMMAND);
      buffer.Add((byte)(this.RT_DELAY >> 8));
      buffer.Add((byte)(this.RT_DELAY & 0xFF));
      buffer.Add(this.RNID_TRACK);
      return 4;
    }

    protected override string stringify()
    {
      return $"{RQ_LEVEL_CROSSING_COMMAND.ToString()} -> Delay: {this.RT_DELAY} -> Track: {RNID_TRACK}";
    }
  }
}
