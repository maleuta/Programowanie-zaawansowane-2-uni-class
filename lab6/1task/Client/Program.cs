using System;
using System.Net.Sockets;
using System.Text;

/*
"""
Celem laboratorium jest zapoznanie z programowaniem sieciowym z zastosowaniem gniazd TCP/
IP. Proszę wykonać następujące programy.

1. [4 punkty] Proszę napisać program serwera oraz program klienta. 
Serwer będzie oczekiwał na połączenie jednego klienta. Klient po połączeniu z serwerem 
ma wysłać na serwer wpisaną z klawiatury tekstową wiadomość. Jeżeli długość wiadomości 
przekroczy 1024 bajty proszę ograniczyć ją do 1024 bajtów. Serwer ma odebrać wiadomość 
i wypisać ją w postaci zdekodowanego napisu (String-a, a nie tablicy bajtów) do konsoli.
 Następnie ma wysłać do klienta wiadomość "odczytalem: " i przesłany przez klienta napis. 
 Jeżeli długość wiadomości przekroczy 1024 bajty proszę ograniczyć ją do 1024 bajtów. 
 Po przesłaniu program serwera ma zakończyć działanie. Klient ma odebrać wiadomość 
 od serwera, wypisać ją na ekran w postaci napisu (String) i zakończyć działanie.
""";
*/

class Client
{
    static void Main()
    {
        int port = 8080;

        try
        {
            // połączenie z serwerem
            using TcpClient client = new TcpClient("127.0.0.1", port);
            using NetworkStream stream = client.GetStream();

            // pobr wiadom z klaw 
            Console.WriteLine("Wpisz wiadomość do wysłania:");
            string input = Console.ReadLine() ?? string.Empty;

            // zamiana na bajty
            byte[] data = Encoding.UTF8.GetBytes(input);

            // ogranicz wiadomości do 1024 bajtów
            if (data.Length > 1024)
            {
                Array.Resize(ref data, 1024);
            }

            // wysłanie wiadom do serwera
            stream.Write(data, 0, data.Length);

            // odbiór odpow 
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            // zdekod i wyświetlenie odpowiedzi
            string responseMsg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine(responseMsg);
        }
        catch (Exception e)
        {
            Console.WriteLine("Błąd klienta: " + e.Message);
        }
    }
}