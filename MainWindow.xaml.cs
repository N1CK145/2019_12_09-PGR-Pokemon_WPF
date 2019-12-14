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

namespace PokemonWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logger logger = new Logger(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.pokemon\logs\", "%date%.log");
        private string requestURL = "https://localhost:44330/api/pokemon";
        private JArray requestJsonArray;
        private bool upToDate = false;

        public MainWindow()
        {
            try
            {
                logger.Log("---------------------------------------------------");
                logger.Log("Initialize Componets...");
                InitializeComponent();

                initData();
                infoBanner.Text = "URL: " + requestURL;
            }
            catch (Exception e)
            { logger.Error(e.StackTrace); }
        }

        public void initData()
        {
            int defaultPkmn = 1;
            logger.Log("Getting json from \"" + requestURL + "\"");
            requestJsonArray = makeRequest(requestURL);

            new Thread(() => DownloadImages(requestJsonArray)).Start();

            bool finnish = false;
            while(!finnish || liveLogger.Content == null)
            {
                foreach(JObject jsonItem in requestJsonArray)
                {
                    if (jsonItem.Value<int>("number") == defaultPkmn)
                        while (liveLogger.Content.ToString() == jsonItem.Value<String>("sprite")) { Thread.Sleep(50); }
                }
                break;
            }

            setPokemonData(defaultPkmn, requestJsonArray);
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

                    logger.Log("Downloading image: \"" + json["sprite"] + "\"");
                    if (!WebDownloader.Download(link, savePath + fileName))
                        MessageBox.Show("ERROR:\n\nFileNotFound: " + link);
                    liveLogger.Dispatcher.Invoke(new Action(() => liveLogger.Content = "Downloading: " + link));
                }
            }
            // liveLogger.Dispatcher.Invoke(new Action(() => liveLogger.Content = "Done..."));
        }

        private void setPokemonData(int id, JArray requestJsonArray)
        {
            if (requestJsonArray == null)
                return;

            foreach (JObject json in requestJsonArray)
            {
                if(json["number"].ToString() == id.ToString())
                {
                    logger.Log("Show pokemon number " + json["number"].ToString());
                    string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.pokemon/images/pokemon/";
                    string fileName = json["sprite"].ToString().Split('/').Last<String>();

                    pokemonImage.Source = new BitmapImage(new Uri(filePath + fileName, UriKind.Absolute));
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
            setPokemonData(Int32.Parse(pokemonID.Content.ToString()) - 1, requestJsonArray);
        }

        private void nextDataSet_Click(object sender, RoutedEventArgs e)
        {
            setPokemonData(Int32.Parse(pokemonID.Content.ToString()) + 1, requestJsonArray);
        }
    }
}
