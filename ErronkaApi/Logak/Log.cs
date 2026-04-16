using System;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace ErronkaApi.Logak
{
    public class Log
    {
        private readonly string logKarpeta;
        private static readonly object LogLock = new object();

        public Log(IHostEnvironment hostEnvironment)
        {
            logKarpeta = Path.Combine(hostEnvironment.ContentRootPath, "TPV_Logs");

            if (!Directory.Exists(logKarpeta))
                Directory.CreateDirectory(logKarpeta);
        }

        public void GordeLog(string erabiltzailea, string ekintza)
        {
            try
            {
                string eguna = DateTime.Now.ToString("yyyy-MM-dd");
                string fitxategia = Path.Combine(logKarpeta, $"TPV_Log_{eguna}.log");
                string ilara = $"{DateTime.Now:HH:mm:ss} | {erabiltzailea} | {ekintza}";

                lock (LogLock)
                {
                    File.AppendAllText(fitxategia, ilara + Environment.NewLine);
                }
            }
            catch
            {
                // Loga gordetzeko arazoak izan badira, ez dugu ezer egingo
            }
        }
    }
}
