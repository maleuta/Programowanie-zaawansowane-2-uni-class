using System;
using System.IO;
using System.Linq;
using System.Net;
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
class Server
{
    static void Main()
    {
        int port = 8080;
        TcpListener server = null;

        try
        {
            server = new TcpListener(IPAddress.Loopback, port);
            server.Start();
            Console.WriteLine("serwer uruchomiony. oczekiwanie na połączenie klienta...");

            using TcpClient client = server.AcceptTcpClient();
            using NetworkStream stream = client.GetStream();
            Console.WriteLine("klient połączony.");

            // pamięt ścieżki startowej w zmiennej my_dir
            string my_dir = Directory.GetCurrentDirectory();

            while (true)
            {
                // 1. odbiór nagłówka z rozmiarem
                byte[] lengthBuffer = new byte[4];
                ReadExactly(stream, lengthBuffer, 4);
                int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                // 2. odbiór właściwej wiadomości
                byte[] messageBuffer = new byte[messageLength];
                ReadExactly(stream, messageBuffer, messageLength);
                string receivedMsg = Encoding.UTF8.GetString(messageBuffer);
                
                Console.WriteLine($"otrzymano polecenie: {receivedMsg}");

                // obsługa polecenia !end
                if (receivedMsg == "!end")
                {
                    Console.WriteLine("otrzymano polecenie zakończenia. serwer off.");
                    break; 
                }

                string responseString = "";

                // obsługa poleceń katalogowych
                if (receivedMsg == "list")
                {
                    responseString = GetDirectoryContents(my_dir);
                }
                else if (receivedMsg.StartsWith("in "))
                {
                    string targetDir = receivedMsg.Substring(3).Trim();
                    string newPath = Path.GetFullPath(Path.Combine(my_dir, targetDir));

                    if (Directory.Exists(newPath))
                    {
                        my_dir = newPath;
                        responseString = GetDirectoryContents(my_dir);
                    }
                    else
                    {
                        responseString = "katalog nie istnieje";
                    }
                }
                else
                {
                    responseString = "nieznane polecenie";
                }

                // 3. wysłanie odpowiedzi
                byte[] responseData = Encoding.UTF8.GetBytes(responseString);
                byte[] responseLengthBytes = BitConverter.GetBytes(responseData.Length);
                
                stream.Write(responseLengthBytes, 0, 4);
                stream.Write(responseData, 0, responseData.Length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Błąd serwera: " + e.Message);
        }
        finally
        {
            server?.Stop();
        }
    }

    static string GetDirectoryContents(string path)
    {
        try
        {
            var items = Directory.GetFileSystemEntries(path);
            if (items.Length == 0) return "[katalog jest pusty]";
            
            // pobiera same nazwy plików/folderów z pełnych ścieżek
            var names = items.Select(Path.GetFileName);
            return string.Join(Environment.NewLine, names);
        }
        catch (Exception ex)
        {
            return "błąd " + ex.Message;
        }
    }

    // odczyt wymaganej liczby bajtów
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