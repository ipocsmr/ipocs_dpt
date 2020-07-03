using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Makaretu.Dns;

namespace IPOCS_Programmer
{
  /// <summary>
  /// Interaction logic for NetworkScanner.xaml
  /// </summary>
  public partial class NetworkScanner : UserControl
  {
    private static TimeSpan TTL { get; } = TimeSpan.FromSeconds(10);
    private MulticastService mDNS { get; }
    private ServiceDiscovery sDiscovery { get; }

    private Timer Timer { get; } = new Timer();
    public ObservableCollection<string> ResponseItems { get; } = new ObservableCollection<string>();

    private string[] ServicesToFind { get; } = { "_ipocs._tcp.local", "_http._tcp.local" };

    public ObservableCollection<Service> Services { get; } = new ObservableCollection<Service>();
    public NetworkScanner()
    {
      InitializeComponent();

      mDNS = new MulticastService();
      sDiscovery = new ServiceDiscovery(mDNS);


      mDNS.NetworkInterfaceDiscovered += (s, e) =>
      {
        foreach (var nic in e.NetworkInterfaces)
        {
          AddLog($"NIC '{nic.Name}'");
        }
      };

      sDiscovery.ServiceDiscovered += (s, serviceName) =>
      {
        if (!ServicesToFind.Any(sN => serviceName.ToString().Contains(sN)))
          return;
        AddLog($"service '{serviceName}'");

        var service = Services.ToArray().FirstOrDefault(ss => ss.Name == serviceName);
        if (service == null)
        {
          service = new Service
          {
            LastSeen = DateTime.UtcNow,
            Name = serviceName.ToString()
          };
          Dispatcher.Invoke(() => {
            Services.Add(service);
          });
      }

        // Ask for the name of instances of the service.
        mDNS.SendQuery(serviceName, type: DnsType.PTR);
      };

      sDiscovery.ServiceInstanceDiscovered += (s, e) =>
      {

        var service = Services.FirstOrDefault(ss => e.ServiceInstanceName.ToString().EndsWith(ss.Name));
        if (service != null)
        {
          if (!service.Instances.Any(sI => sI.Name == e.ServiceInstanceName))
          {
            Dispatcher.Invoke(() =>
            {
              service.Instances.Add(new ServiceInstance
              {
                LastSeen = DateTime.UtcNow,
                Name = e.ServiceInstanceName.ToString()
              });
            });
          }
          AddLog($"service instance '{e.ServiceInstanceName}'");

          // Ask for the service instance details.
          mDNS.SendQuery(e.ServiceInstanceName, type: DnsType.SRV);
        }
      };

      mDNS.AnswerReceived += (s, e) =>
      {

        // Is this an answer to a service instance details?
        var servers = e.Message.Answers.OfType<SRVRecord>();
        foreach (var server in servers)
        {
          if (!ServicesToFind.Any(sN => server.Name.ToString().EndsWith(sN)))
            continue;
          //          server.Name

          AddLog($"host '{server.Target}' for '{server.Name}'");
          var serviceInstance = (from service in Services
                                 where service.Instances.Any(sI => sI.Name.EndsWith(server.Name.ToString()))
                                 select service.Instances.FirstOrDefault(sI => sI.Name.EndsWith(server.Name.ToString()))).FirstOrDefault();
          if (serviceInstance == null)
            continue;

          if (!serviceInstance.Hosts.Any(sih => sih.Name == server.Target))
          {
            Dispatcher.Invoke(() =>
            {
              serviceInstance.Hosts.Add(new ServiceInstanceHost
              {
                LastSeen = DateTime.UtcNow,
                Name = server.Target.ToString()
              });
            });
          }
          // Ask for the host IP addresses.
          mDNS.SendQuery(server.Target, type: DnsType.A);
          //mDNS.SendQuery(server.Target, type: DnsType.AAAA);
        }

        // Is this an answer to host addresses?
        var addresses = e.Message.Answers.OfType<AddressRecord>();
        foreach (var address in addresses)
        {
          var serviceInstanceHost = (from service in Services
                                     where service.Instances.Any(sI => sI.Hosts.Any(sih => sih.Name == address.Name))
                                     select service.Instances.Select(sI => sI.Hosts.First(sih => sih.Name == address.Name)).First()).FirstOrDefault();

          if (serviceInstanceHost == null)
            continue;
          AddLog($"host '{address.Name}' at {address.Address}");
          if (serviceInstanceHost.Adresses.Any(a => a.Equals(address.Address)))
            continue;
          Dispatcher.Invoke(() =>
          {
            serviceInstanceHost.Adresses.Add(address.Address);
          });
        }
      };

      mDNS.Start();
      Timer.Interval = 5000;
      Timer.AutoReset = true;
      Timer.Elapsed += Timer_Elapsed;
      Timer.Start();

    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      foreach (var service in Services.Where(s => DateTime.UtcNow - s.LastSeen > TTL).ToList())
      {
        Dispatcher.Invoke(() =>
        {
          Services.Remove(service);
        });
      }
      foreach (var service in Services)
      {
        foreach (var serviceInstance in service.Instances.Where(si => DateTime.UtcNow - si.LastSeen > TTL).ToList())
        {
          Dispatcher.Invoke(() =>
          {
            service.Instances.Remove(serviceInstance);
          });
        }

        foreach (var serviceInstance in service.Instances)
        {
          foreach (var serviceInstanceHost in serviceInstance.Hosts.Where(si => DateTime.UtcNow - si.LastSeen > TTL).ToList())
          {
            Dispatcher.Invoke(() =>
            {
              serviceInstance.Hosts.Remove(serviceInstanceHost);
            });
          }

          foreach (var serviceInstanceHost in serviceInstance.Hosts)
          {
          }
        }
      }
      Dispatcher.Invoke(() =>
      {
        ResponseItems.Clear();
      });
      sDiscovery.QueryAllServices();
    }

    private void AddLog(string msg)
    {
      Dispatcher.Invoke(() =>
      {
        ResponseItems.Add(msg);
      });
    }

    ~NetworkScanner()
    {
      sDiscovery.Dispose();
      mDNS.Stop();
    }
  }

  public class Service
  {
    public DateTime LastSeen { get; set; }
    public string Name { get; set; }
    public ObservableCollection<ServiceInstance> Instances { get; set; } = new ObservableCollection<ServiceInstance>();
  }

  public class ServiceInstance
  {
    public DateTime LastSeen { get; set; }
    public string Name { get; set; }
    public ObservableCollection<ServiceInstanceHost> Hosts { get; set; } = new ObservableCollection<ServiceInstanceHost>();
  }

  public class ServiceInstanceHost
  {
    public DateTime LastSeen { get; set; }
    public string Name { get; set; }
    public ObservableCollection<IPAddress> Adresses { get; set; } = new ObservableCollection<IPAddress>();
  }

}
