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
using System.Threading;
using System.Windows.Documents;
using System.Diagnostics;

namespace PokemonWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mainWindow;
        private Logger logger = new Logger(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.pokemon\logs\", "%date%.log");
        private string requestURL = "https://localhost:44330/api/pokemon";
        public JArray RequestJsonArray;
        private bool upToDate = false;

        public MainWindow()
        {
            mainWindow = this;
            try
            {
                InitializeComponent();
                logger.Log("---------------------------------------------------");
                logger.Log("Initialize Componets...");

                initData();
                infoBanner.Text = "RequestURL: " + requestURL;
            }
            catch (Exception e)
            { logger.Error(e.Message + "\n" + e.StackTrace); }
        }

        public void initData()
        {
            int defaultPkmn = 1;
            logger.Log("Getting json from \"" + requestURL + "\"");
            RequestJsonArray = makeRequest(requestURL);

            new Thread(() => DownloadImages(RequestJsonArray)).Start();

            if(!upToDate)
            {
                foreach(JObject jsonItem in RequestJsonArray)
                {
                    if (jsonItem.Value<int>("number") == defaultPkmn)
                    {
                        while (liveLogger.Content.ToString() == jsonItem["sprite"].ToString()) { Thread.Sleep(10); }
                        break;
                    }
                }
            }

            SetPokemonData(defaultPkmn, RequestJsonArray);
        }

        public void DownloadImages(JArray jsonArray)
        {
            string savePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +"/.pokemon/images/pokemon/";
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            if (!upToDate)
            {
                if (jsonArray == null)
                    return;

                foreach (JObject json in jsonArray)
                {
                    string link = json["sprite"].ToString();
                    string fileName = link.Split('/').Last<String>();
                    if (File.Exists(savePath + fileName))
                    {
                        logger.Log("File \"" + savePath + fileName + "\" already exists. Skipping download!");
                        continue;
                    }

                    logger.Log("Downloading image: \"" + json["sprite"] + "\"");
                    if (!WebDownloader.Download(link, savePath + fileName))
                        MessageBox.Show("ERROR:\n\nFileNotFound: " + link);
                }
            }
            logger.Log("Done!");
        }

        public void SetPokemonData(int id, JArray requestJsonArray)
        {
            if (requestJsonArray == null)
                return;

            foreach (JObject json in requestJsonArray)
            {
                if(json["number"].ToString() == id.ToString())
                {
                    logger.Log("Show pokemon " + json["number"].ToString() + " (" + json["pokemon"].ToString() + ")");

                    string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.pokemon/images/pokemon/";
                    string fileName = json["sprite"].ToString().Split('/').Last<String>();

                    pokemonImage.Source = new BitmapImage(new Uri(filePath + fileName, UriKind.Absolute));
                    pokemonImage.ToolTip = json["sprite"].ToString();
                    pokemonImageLink.NavigateUri = new Uri(json["sprite"].ToString(), UriKind.Absolute);

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

        private JArray makeRequest(string url)
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

            return responseString != null ? JArray.Parse(responseString) : null;
        }

        private void previousDataSet_Click(object sender, RoutedEventArgs e)
        {
            SetPokemonData(Int32.Parse(pokemonID.Content.ToString()) - 1, RequestJsonArray);
        }

        private void nextDataSet_Click(object sender, RoutedEventArgs e)
        {
            SetPokemonData(Int32.Parse(pokemonID.Content.ToString()) + 1, RequestJsonArray);
        }

        private void PokemonImageLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // Start Link in Browser
            Hyperlink hl = (Hyperlink)sender;
            string navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
        }

        private void SearchDataSet_Click(object sender, RoutedEventArgs e)
        {
            Window w = new SearchWindow(this);
            w.Show();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            initData();
        }
    }
}
