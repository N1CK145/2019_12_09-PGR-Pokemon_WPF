using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PokemonWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }
        private string makeRequest(string url)
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "Nothing");

            client.DownloadStringCompleted += (sender, e) =>
            {
                var serializer = new DataContractJsonSerializer(typeof(List<Contributor>));
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(e.Result)))
                {
                    var contributors = (List<Contributor>)serializer.ReadObject(ms);
                    contributors.ForEach(Console.WriteLine);
                }
            };

            client.DownloadStringAsync(new Uri(url));
        }
    }
}
