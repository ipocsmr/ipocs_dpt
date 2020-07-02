using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using IPOCS_Programmer.Avalonia.ViewModels;
using IPOCS_Programmer.Avalonia.Views;
using System.Collections.ObjectModel;

namespace IPOCS_Programmer.Avalonia
{
  public class App : Application
  {
    public static ObservableCollection<ObjectTypes.Concentrator> Concentrators { get; } = new ObservableCollection<ObjectTypes.Concentrator>();

    public override void Initialize()
    {
      AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
      if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
      {
        desktop.MainWindow = new MainWindow
        {
          DataContext = new MainWindowViewModel(),
        };
      }

      base.OnFrameworkInitializationCompleted();
    }
  }
}
