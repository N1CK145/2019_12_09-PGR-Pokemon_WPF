using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PokemonWPF
{
    class WebDownloader
    {
        internal static bool Download(string url, string savePath)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, savePath);
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
                return false;
            }
        }
    }
}
