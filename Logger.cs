using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonWPF
{
    class Logger
    {
        string path = "";
        public Logger(string savePath, string saveFile)
        {
            if (!savePath.EndsWith("\\") && !savePath.EndsWith("/"))
                savePath += "/";
            path = savePath + saveFile.Replace("%date%", DateTime.Now.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            if (!File.Exists(path))
                File.Create(path);
        }

        public void Log(string logMessage)
        {
            String dateTime = DateTime.Now.ToString("H:mm:ss");
            String message = "[" + dateTime + "] INFO: " + logMessage;
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
            }
        }
        public void Error(string logMessage)
        {
            String dateTime = DateTime.Now.ToString("H:mm:ss");
            String message = "[" + dateTime + "] ERROR: " + logMessage;
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Flush();
            }
        }
    }
}
