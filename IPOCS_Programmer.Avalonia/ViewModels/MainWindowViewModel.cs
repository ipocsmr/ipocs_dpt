using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace IPOCS_Programmer.Avalonia.ViewModels
{
  public class MainWindowViewModel : ViewModelBase
  {
    public string Greeting => "Welcome to Avalonia!";

    public TextBlock TextBlock { get; } = new TextBlock();

    public ObservableCollection<Models.Client> Clients { get; } = new ObservableCollection<Models.Client>();

    public MainWindowViewModel()
    {
      Clients.Add(new Models.Client(new IPOCS.Client(new System.Net.Sockets.TcpClient())));
      this.TextBlock.Text = "TEsting";
    }
  }
}
