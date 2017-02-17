using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOCS_Programmer.ObjectTypes
{
  public class GenericInput : BasicObject
  {
    public override byte objectTypeId { get { return 1; } }

    public byte inputPin { get; set; }
    
    public override List<byte> Serialize()
    {
      var vector = new List<byte>();
      vector.Add(this.objectTypeId);
      byte[] toBytes = Encoding.ASCII.GetBytes(this.Name);
      vector.AddRange(toBytes);
      vector.Add(0);
      vector.Add(1); // Length;
      vector.Add(this.inputPin);

      return vector;
    }
  }
}
