using System.Diagnostics;

namespace EpiSolve
{
    /// <summary>
    /// The main entry point for the EpiSolve application.
    /// Responsible for loading configuration and starting the Evolutionary Algorithm.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            AppConfig config = ConfigLoader.LoadConfig();

            if (config == null)
            {
                Console.WriteLine("Nepodařilo se načíst konfiguraci, program nemůže pokračovat.");
                return;
            }

            EA ea = new EA(config);


            Stopwatch sw = new Stopwatch();
            
            sw.Start();
            ea.FindBestStrategy();
            sw.Stop();

            Console.WriteLine($"Elapsed Time: {sw.Elapsed.TotalSeconds} s\n");
        }
    }
}
