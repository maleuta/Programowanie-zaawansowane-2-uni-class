using System;
using System.Collections.Generic;
using System.Threading;

namespace ProducerConsumerLab
{
    // klasa reprezentująca generowane dane
    class DataItem
    {
        public int ProducerId { get; set; }
    }

    class Program
    {
        // współdzielona struktura danych
        static Queue<DataItem> sharedQueue = new Queue<DataItem>();
        
        // obiekt do synchronizacji (sekcja krytyczna)
        static readonly object lockObj = new object();
        
        // fflaga sterująca działaniem wątków
        static volatile bool isRunning = true;

        static void Main(string[] args)
        {
            int n = 3; // producent
            int m = 2; // konsument

            List<Thread> threads = new List<Thread>();

            Console.WriteLine("'q', aby zatrzymać\n");

            // uruch wątków producentów
            for (int i = 0; i < n; i++)
            {
                int id = i;
                // parametry: ID, min i maks czas oczekn(w milisek)
                Thread t = new Thread(() => ProducerTask(id, 500, 1500));
                t.IsBackground = false; // zalozenie
                t.Start();
                threads.Add(t);
            }

            // wątki konsumentów
            for (int i = 0; i < m; i++)
            {
                int id = i;
                // parametry: ID, min i maks czas oczek (w milisek)
                Thread t = new Thread(() => ConsumerTask(id, 800, 2000));
                t.IsBackground = false; 
                t.Start();
                threads.Add(t);
            }

            // wątek gł nasłuchuje wciśnięcia klawisza 'q'
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                    {
                        Console.WriteLine("\n[Main] rozpoczęto zamykanie programu...");
                        isRunning = false;

                        // wybudzamy wszystkie zablokowane wątki konsumentów,
                        // aby mogły sprawdzić flagę isRunning i zakończyć działanie.
                        lock (lockObj)
                        {
                            Monitor.PulseAll(lockObj);
                        }
                        break;
                    }
                }
                Thread.Sleep(50); 
            }

            // oczekiwanie na bezp zakończ wszystkich wątków
            foreach (var t in threads)
            {
                t.Join();
            }

            Console.WriteLine("\nprogram zakończył działanie poprawnie");
        }

        // metoda przez wątki PRODUCENTÓW
        static void ProducerTask(int id, int minDelay, int maxDelay)
        {
            // unique seed dla generatora liczb losowych per wątek
            Random rnd = new Random(id * 1000 + Environment.TickCount);

            while (isRunning)
            {
                // symulacja czasu gener danych
                Thread.Sleep(rnd.Next(minDelay, maxDelay));

                if (!isRunning) break;

                DataItem item = new DataItem { ProducerId = id };

                // dod elementu
                lock (lockObj)
                {
                    sharedQueue.Enqueue(item);
                    Console.WriteLine($"[Producent {id}] wygenerował i dodał dane.");
                    
                    // inform jednego z oczek konsumentów, że pojawiły się nowe dane
                    Monitor.Pulse(lockObj);
                }
            }
        }

        // metoda przez wątki KONSUMENTÓW
        static void ConsumerTask(int id, int minDelay, int maxDelay)
        {
            Random rnd = new Random(id * 2000 + Environment.TickCount);
            
            // słownik zapamięt ile danych pobrano od producentów
            Dictionary<int, int> consumedStats = new Dictionary<int, int>();

            while (isRunning)
            {
                DataItem item = null;

                // pobieranie elementu
                lock (lockObj)
                {
                    // używamy pętli while, aby zabezpieczyć się przed fałsz wybudz
                    while (sharedQueue.Count == 0 && isRunning)
                    {
                        Monitor.Wait(lockObj); // zwalnia blokadę i czeka na Pulse() od producenta
                    }

                    // if program jest wyłączany i kolejka jest pusta, przerywamy
                    if (!isRunning && sharedQueue.Count == 0)
                    {
                        break;
                    }

                    // w przec razie pobieramy element
                    if (sharedQueue.Count > 0)
                    {
                        item = sharedQueue.Dequeue();
                        Console.WriteLine($"[Konsument {id}] pobrał dane od producenta {item.ProducerId}.");
                    }
                }

                // aktual statystyk i symulacja "przetwarzania" poza sekcją krytyczną
                if (item != null)
                {
                    if (consumedStats.ContainsKey(item.ProducerId))
                        consumedStats[item.ProducerId]++;
                    else
                        consumedStats[item.ProducerId] = 1;

                    // symulacja czasu "konsumpcji" danych
                    Thread.Sleep(rnd.Next(minDelay, maxDelay));
                }
            }

            // print podczas zamykania wątku
            PrintSummary(id, consumedStats);
        }

        static void PrintSummary(int consumerId, Dictionary<int, int> stats)
        {
            // lock dla konsoli, żeby wypis podsumow różnych konsumentów
            // nie nałożyły się na siebie na ekranie.
            lock (Console.Out)
            {
                Console.WriteLine($"\nkonsument {consumerId} kończy działanie. podsumowanie...");
                if (stats.Count == 0)
                {
                    Console.WriteLine("  brak skonsumowanych danych.");
                }
                else
                {
                    foreach (var kvp in stats)
                    {
                        Console.WriteLine($"  producent {kvp.Key} - {kvp.Value}");
                    }
                }
            }
        }
    }
}