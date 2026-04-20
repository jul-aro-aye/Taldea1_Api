using System;
using System.IO;

namespace ErronkaApi.Logak
{
    public class LogAnder
    {
        public void RegistrarLog(string mensaje)
        {
            string directorioApp = AppDomain.CurrentDomain.BaseDirectory;
            string rutaArchivo = Path.Combine(directorioApp, "logTpv.txt");

            try
            {
                using (StreamWriter sw = new StreamWriter(rutaArchivo, true))
                {
                    string hora = DateTime.Now.ToString("HH:mm:ss");
                    sw.WriteLine($"[{hora}] Se ha ejecutado {mensaje}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error crítico al escribir el log: {ex.Message}");
            }
        }

    }
}