using System;
using System.Collections.Generic;
using System.Threading;

namespace ThreadSyncLab
{
    class Program
    {
        static int n = 5; // liczba wątków
        static int startedThreadsCount = 0; // licznik wątków, które fakt zaczęły pracę
        
        // flaga informująca wątki, że mają zakończyć działanie
        static bool shouldExit = false;
        
        // obiekt do synchr (sekcja krytyczna)
        static readonly object lockObj = new object();

        static void Main(string[] args)
        {
            List<Thread> threads = new List<Thread>();

            Console.WriteLine($"[główny] uruchamiam {n} wątków...");

            for (int i = 0; i < n; i++)
            {
                int id = i;
                Thread t = new Thread(() => WorkerTask(id));
                t.IsBackground = false; // założenie
                threads.Add(t);
                t.Start();
            }

            // KROK 1: oczek aż WSZYSTKIE wątki fakt wystartują
            lock (lockObj)
            {
                while (startedThreadsCount < n)
                {
                    // wątek gł zasypia, dopóki jakiś wątek roboczy go nie powiadomi (Pulse)
                    Monitor.Wait(lockObj);
                }
            }

            Console.WriteLine("[Główny] SUKCES: Wszystkie wątki zaczęły się wykonywać");

            // KROK 2: inicjowanie zamknięcia
            Console.WriteLine("[główny] wysyłam sygnał zamkn do wszystkich wątków");
            lock (lockObj)
            {
                shouldExit = true;
                // budzimy wszystkie wątki, które czekają wewnątrz WorkerTask
                Monitor.PulseAll(lockObj);
            }

            // oczekiw na fiz zakończ każdego wątku
            foreach (var t in threads)
            {
                t.Join();
            }

            Console.WriteLine("[Główny] wszystkie wątki zostały zamknięte. Zamykam program.");
        }

        static void WorkerTask(int id)
        {
            // sygnal startu
            lock (lockObj)
            {
                startedThreadsCount++;
                // powiad wątek gł, że ten konkretny wątek już żyje i pracuje
                Monitor.Pulse(lockObj);
                
                // tu wątek "zatrzymuje się", aby nie skończyć pracy zbyt szybko
                // czeka na sygnał od głównego (shouldExit)
                while (!shouldExit)
                {
                    Monitor.Wait(lockObj);
                }
            }
            
            // tu wątek fakt kończy metodę
        }
    }
}