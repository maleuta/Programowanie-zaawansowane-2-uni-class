using System;
using System.Net.Sockets;
using System.Text;

/*
3. [4 punkty] Napisz program o architekturze klient - serwer. Serwer ma oczekiwać na połączenie 
jednego klienta i ma "pamiętać" w zmiennej "my_dir" ścieżkę do swojego katalogu startowego. 
Klient będzie wysyłał na serwer wiadomości tekstowe wczytane z klawiatury a następnie odbierał 
i wypisywał wiadomości z serwera. Obsługiwane wiadomości:
    - "!end" - zakończ zarówno program serwera jak i klienta.

    - "list" - prześlij do klienta nazwy wszystkich katalogów i plików znajdujących się na ścieżce 
    zmiennej "my_dir" (bez rekurencyjnego wchodzenia do katalogów).

    - "in \[nazwa\]" - jeżeli "nazwa" jest podkatalogiem na ścieżce "my_dir" proszę zmodyfikować ścieżkę tak, 
    aby wskazywała na ten podkatalog i przesłać do klienta nazwy wszystkich katalogów i plików znajdujących się 
    na ścieżce zmiennej "my_dir" (bez rekurencyjnego wchodzenia do katalogów). Jeżeli "nazwa" nie jest podkatalogiem 
    proszę przesłać do klienta wiadomość "katalog nie istnieje". Jeżeli "nazwa" to ".." proszę spróbować wejść do katalogu 
    nadrzędnego i obsłużyć jego odczyt w analogiczny sposób jak dla każdego innego napisu.

    - Każdy inny przypadek - serwer ma przesłać do klienta wiadomość "nieznane polecenie".
    
    Nieznaną długość bajtowej wiadomości proszę obsłużyć analogicznie jak w zadaniu numer 2. 

*/

class Client
{
    static void Main()
    {
        int port = 8080;

        try
        {
            using TcpClient client = new TcpClient("127.0.0.1", port);
            using NetworkStream stream = client.GetStream();

            Console.WriteLine("połączono z serwerem. Dostępne polecenia: list, in [nazwa], !end");

            while (true)
            {
                Console.Write("\n> ");
                string input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input)) continue;

                // 1. wysłanie wiadom (nagłówek + dane)
                byte[] data = Encoding.UTF8.GetBytes(input);
                byte[] lengthBytes = BitConverter.GetBytes(data.Length);
                
                stream.Write(lengthBytes, 0, 4);
                stream.Write(data, 0, data.Length);

                // if wysłaliśmy polecenie zakończenia, od razu przerywamy pętlę klienta
                if (input == "!end")
                {
                    Console.WriteLine("kończenie pracy klienta");
                    break;
                }

                // 2. odbiór odpow od serwera
                byte[] responseLengthBuffer = new byte[4];
                ReadExactly(stream, responseLengthBuffer, 4);
                int responseLength = BitConverter.ToInt32(responseLengthBuffer, 0);

                byte[] responseBuffer = new byte[responseLength];
                ReadExactly(stream, responseBuffer, responseLength);
                
                string responseMsg = Encoding.UTF8.GetString(responseBuffer);
                Console.WriteLine("odpowiedź serwera:\n" + responseMsg);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Błąd klienta: " + e.Message);
        }
    }

    static void ReadExactly(NetworkStream stream, byte[] buffer, int count)
    {
        int totalBytesRead = 0;
        while (totalBytesRead < count)
        {
            int bytesRead = stream.Read(buffer, totalBytesRead, count - totalBytesRead);
            if (bytesRead == 0) throw new Exception("Połączenie przerwane.");
            totalBytesRead += bytesRead;
        }
    }
}