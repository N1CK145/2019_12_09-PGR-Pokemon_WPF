using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PokemonWPF
{
    /// <summary>
    /// Interaktionslogik für SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        private MainWindow mainWindow;
        public SearchWindow(MainWindow w)
        {
            InitializeComponent();
            mainWindow = w;
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            List<String> rawCommands = txtSearchBox.Text.Split(';').ToList<String>();
            Dictionary<string, string> commands = new Dictionary<string, string>();

            foreach(string item in rawCommands)
            {
                string input = item.Replace(" ", "").ToLower();

                if(input.Length != 0)
                {
                    try
                    {
                        commands.Add(input.Split('=')[0].ToLower(), input.Split('=')[1].ToLower());
                    }
                    catch (Exception){}
                }
            }
            
            foreach(var cmd in commands)
            {
                switch (cmd.Key)
                {
                    case "number": case "id":
                        mainWindow.SetPokemonData(Int32.Parse(cmd.Value), mainWindow.RequestJsonArray);
                        break;
                    default:
                        MessageBox.Show("Konnte Befehl \"" + cmd.Key + "\" nicht finden!", "Unbekannter Befehl");
                        return;
                }
            }
        }
    }
}
