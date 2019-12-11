using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;

namespace PokemonWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string requestURL = "https://localhost:44330/api/pokemon";
        private string requestString;

        public MainWindow()
        {
            InitializeComponent();
            requestString = makeRequest(requestURL);
            int currentPokemon = 1;
            
            infoBanner.Text = "URL: " + requestURL;
            setPokemonData(currentPokemon);
        }

        private void setPokemonData(int id)
        {
            try
            {
                JArray jsonArray = JArray.Parse(requestString);
                foreach (JObject json in jsonArray)
                {
                    if(json["number"].ToString() == id.ToString())
                    {
                        pokemonID.Content = json["number"];
                        pokemonName.Content = json["pokemon"];
                        if(json["type2"].ToString() == "none")
                            pokemonType.Content = json["type1"];
                        else
                            pokemonType.Content = json["type1"] + "; " + json["type2"];
                        pokemonLives.Content = json["hp"];
                        pokemonDamage.Content = json["attack"];
                        return;
                    }
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        private string makeRequest(string url)
        {
            string code = null;
            string responseString = null;
            WebRequest request = WebRequest.Create(url);
            
            try
            {
                WebResponse response = request.GetResponse();
                
                using (Stream st = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(st);
                    responseString = reader.ReadToEnd();
                }
                
                responseCode.Content = "Response Status: OK";
                imageConnectionStatus.Source = new BitmapImage(new Uri("icons/connection/connected_16x16.png", UriKind.Relative));
                code = null;
            }
            catch (WebException e)
            {
                code = e.Message.ToString();
                responseCode.Content = "Response Status: ERROR";
                imageConnectionStatus.Source = new BitmapImage(new Uri("icons/connection/disconnected_16x16.png", UriKind.Relative));
            }
            responseCode.ToolTip = code;
            return responseString;
        }

        private void previousDataSet_Click(object sender, RoutedEventArgs e)
        {
            setPokemonData(Int32.Parse(pokemonID.Content.ToString()) - 1);
        }

        private void nextDataSet_Click(object sender, RoutedEventArgs e)
        {
            setPokemonData(Int32.Parse(pokemonID.Content.ToString()) + 1);
        }
    }
}
