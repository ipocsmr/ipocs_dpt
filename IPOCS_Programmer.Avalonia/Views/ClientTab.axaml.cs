using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IPOCS_Programmer.Avalonia.Views
{
  public class ClientTab : UserControl
  {
    /*
    public static readonly DirectProperty<ClientTab, Models.Client> ClientProperty =
      AvaloniaProperty.RegisterDirect<ClientTab, Models.Client>(
        nameof(Client), o => o.Client, (o, v) => o.Client = v);

    private Models.Client _client = null;
    public Models.Client Client
    {
      get { return _client; }
      set { SetAndRaise(ClientProperty, ref _client, value); }
    }
    */

    public ClientTab()
    {
      //Bind(DataContextProperty, this.GetObservable(ClientProperty));
      this.InitializeComponent();
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
