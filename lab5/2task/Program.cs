using System;
using System.IO;
using System.Threading;

namespace DirectoryMonitorLab
{
    class Program
    {
        // flaga sterująca działaniem wątku monitorującego
        static volatile bool isRunning = true;
        
        // obiekt chron dostęp do konsoli 
        static readonly object consoleLock = new object();

        static void Main(string[] args)
        {
            // domyślnie monitor bieżący katalog uruchom progr
            string pathToMonitor = Directory.GetCurrentDirectory();

            Console.WriteLine($"rozpoczęto monitorowanie katalogu: {pathToMonitor}");
            Console.WriteLine("Wciśnij 'q' lub 'Q', aby zakończyć\n");

            // tworz i uruchamiamy osobny wątek do monitor
            Thread monitorThread = new Thread(() => MonitorTask(pathToMonitor));
            monitorThread.IsBackground = false; // założenie laboratorium
            monitorThread.Start();

            // wątek główny: pętla oczekująca na wciśnięcie klawisza 'q'
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                    {
                        lock (consoleLock)
                        {
                            Console.WriteLine("\n[Main] rozpoczęto zamykanie programu");
                        }
                        // zmiana flagi spowoduje wyjście z pętli w wątku monitorującym
                        isRunning = false;
                        break;
                    }
                }
                Thread.Sleep(50); // odciążenie procesora
            }

            // oczekiwanie na poprawne zakończ wątku monitor
            monitorThread.Join();

            lock (consoleLock)
            {
                Console.WriteLine("program zakończył działanie.");
            }
        }

        // metoda !w osobnym wątku!
        static void MonitorTask(string path)
        {
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = path;
                
                //nie monitorujemy podkatalogów
                watcher.IncludeSubdirectories = false; 

                // podpięcie metod pod zdarzenia utworz i usun pliku
                watcher.Created += OnFileCreated;
                watcher.Deleted += OnFileDeleted;

                // uruch nasłuch zmian w systemie plików
                watcher.EnableRaisingEvents = true;

                // pętla utrzym ten wątek przy życiu do momentu przerwania
                while (isRunning)
                {
                    Thread.Sleep(200); 
                }

                // zatrzymanie nasłuch przed bezpiecznym wyjściem z wątku
                watcher.EnableRaisingEvents = false;
            }
        }

        // metoda, plik zostanie dodany
        private static void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            lock (consoleLock)
            {
                Console.WriteLine($"dodano plik [{e.Name}]");
            }
        }

        //metoda, plik zostanie usunięty
        private static void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            lock (consoleLock)
            {
                Console.WriteLine($"usunięto plik [{e.Name}]");
            }
        }
    }
}