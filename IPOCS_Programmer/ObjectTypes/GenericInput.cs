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
  }
}
