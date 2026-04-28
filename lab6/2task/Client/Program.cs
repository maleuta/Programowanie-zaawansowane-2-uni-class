using System;
using System.Net.Sockets;
using System.Text;


/*
2. [2 punkty] Proszę napisać programy, które będą działały analogicznie jak programy 
z punktu 1 z tą różnicą, że długość wiadomości nie będzie ograniczona do 1024 bajtów. 
Proponowane rozwiązanie: załóżmy, że zarówno klient jak i serwer będzie wysyłał 
na początku 4-bajtową wiadomość, w której prześle rozmiar (jako zakodowany do bajtów int) 
kolejnej wiadomości, która będzie zawierać właściwe dane.
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

            Console.WriteLine("wpisz wiadomość do wysłania:");
            string input = Console.ReadLine() ?? string.Empty;

            // 1. przygotowanie danych
            byte[] data = Encoding.UTF8.GetBytes(input);

            // 2. wysłanie nagłówka (4 bajty z rozmiarem wiadomości)
            byte[] lengthBytes = BitConverter.GetBytes(data.Length);
            stream.Write(lengthBytes, 0, 4);

            // 3. wysłanie właściwej wiadomości
            stream.Write(data, 0, data.Length);

            // 4. odbiór nagłówka od serwera (4 bajty)
            byte[] responseLengthBuffer = new byte[4];
            ReadExactly(stream, responseLengthBuffer, 4);
            
            // konw 4 bajtów na typ int
            int responseLength = BitConverter.ToInt32(responseLengthBuffer, 0);

            // 5. odbiór właściwej odpowiedzi o określ rozmiarze
            byte[] responseBuffer = new byte[responseLength];
            ReadExactly(stream, responseBuffer, responseLength);
            
            string responseMsg = Encoding.UTF8.GetString(responseBuffer);
            Console.WriteLine(responseMsg);
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
            if (bytesRead == 0) 
            {
                throw new Exception("Połączenie zostało zerwane przed odebraniem całych danych.");
            }
            totalBytesRead += bytesRead;
        }
    }
}