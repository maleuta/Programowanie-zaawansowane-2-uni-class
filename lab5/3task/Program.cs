using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FileSearchLab
{
    class Program
    {
        // kolejka przechow ścieżki znalez plików
        static Queue<string> foundFilesQueue = new Queue<string>();
        
        // obiekt do synchron dostępu do kolejki
        static readonly object lockObj = new object();
        
        // flaga informująca, czy proces wyszuk dobiegł końca
        static volatile bool isSearchComplete = false;

        static void Main(string[] args)
        {
            // konfig wyszukiw 
            string startDirectory = Directory.GetCurrentDirectory(); 
            string searchPattern = "proj"; // szuk nadpis

            Console.WriteLine($"rozpoczynam wyszukiwa plików zawieraj '{searchPattern}'");
            Console.WriteLine($"katalog początk: {startDirectory}\n");

            // uruchom wyszuk w osobnym wątku
            Thread searchThread = new Thread(() => SearchThreadTask(startDirectory, searchPattern));
            searchThread.IsBackground = false; // założenie
            searchThread.Start();

            // pętla wątku gł - odbier i wypisywa danych (konsument)
            while (true)
            {
                string fileToPrint = null;

                lock (lockObj)
                {
                    // czekamy, dopóki kolejka jest pusta ORAZ wyszukiwanie wciąż trwa
                    while (foundFilesQueue.Count == 0 && !isSearchComplete)
                    {
                        Monitor.Wait(lockObj);
                    }

                    // if są dane w kolejce, pobieramy je do wypisania
                    if (foundFilesQueue.Count > 0)
                    {
                        fileToPrint = foundFilesQueue.Dequeue();
                    }
                    // jeśli kolejka jest pusta i wyszuk się zakończyło, przerywamy pętlę
                    else if (isSearchComplete)
                    {
                        break; 
                    }
                }


                // wypis odbywa się w WĄTKU GŁÓWNYM, poza sekcją kryt
                if (fileToPrint != null)
                {
                    Console.WriteLine($"[Wątek Główny] znaleziono: {fileToPrint}");
                }
            }

            // oczek na ostat i poprawne zamkn wątku szukaj
            searchThread.Join();
            Console.WriteLine("\n[Wątek Główny] wyszukiwanie zakończone.");
        }

        // metoda inic proces wyszuk dla wątku pobocznego
        static void SearchThreadTask(string directory, string pattern)
        {
            // wywołujemy rekurenc funkcję szukającą
            PerformSearch(directory, pattern);

            // po zakończeniu całej rekurencji informujemy wątek główny, że to koniec
            lock (lockObj)
            {
                isSearchComplete = true;
                Monitor.Pulse(lockObj); // wybudzenie wątku gł po raz ostatni
            }
        }

        // właściwa metoda rekurencyjna przeszukująca katalogi
        static void PerformSearch(string currentDir, string pattern)
        {
            try
            {
                // 1. przeszuk plików w akt katalogu
                string[] files = Directory.GetFiles(currentDir);
                foreach (string file in files)
                {
                    // pobier samą nazwę pliku, aby sprawdzić obecność podnapisu
                    string fileName = Path.GetFileName(file);
                    
                    if (fileName.Contains(pattern))
                    {
                        // if pasuje, dodajemy do kolejki (sekcja krytyczna)
                        lock (lockObj)
                        {
                            foundFilesQueue.Enqueue(file);
                            Monitor.Pulse(lockObj); // wybudzamy wątek główny, by wypisał plik
                        }
                    }
                }

                // 2 rekurenc wejście do podkatalogów
                string[] subDirs = Directory.GetDirectories(currentDir);
                foreach (string dir in subDirs)
                {
                    PerformSearch(dir, pattern);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // łapiemy i ignor wyjątek braku uprawnień (np. przy folderach systemowych)            
                }
            catch (Exception ex)
            {
                // bezp obsługa innych niespodziew błędów (np. ścieżka za długa)
                lock (lockObj)
                {
                    foundFilesQueue.Enqueue($"[BŁĄD] {currentDir}: {ex.Message}");
                    Monitor.Pulse(lockObj);
                }
            }
        }
    }
}