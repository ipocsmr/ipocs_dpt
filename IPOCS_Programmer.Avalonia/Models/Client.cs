using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPOCS_Programmer.Avalonia.Models
{
  public class Client
  {
    public Client(IPOCS.Client client)
    {
      IPOCSClient = client;
    }

    public IPOCS.Client IPOCSClient { get; }
    public string Identity => IPOCSClient.Name;

    public List<ObjectTypes.BasicObject> Objects { get { return App.Concentrators.FirstOrDefault((c) => c.UnitID == this.IPOCSClient.UnitID).Objects; } }

  }
}
