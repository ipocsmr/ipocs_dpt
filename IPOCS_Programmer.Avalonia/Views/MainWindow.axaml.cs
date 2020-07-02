using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Diagnostics;

namespace IPOCS_Programmer.Avalonia.Views
{
  public class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
      System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      string version = fvi.FileVersion;
      this.Title = this.Title + " - " + version;
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
