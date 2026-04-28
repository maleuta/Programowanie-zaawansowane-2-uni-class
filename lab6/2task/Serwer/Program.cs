using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


/*
2. [2 punkty] Proszę napisać programy, które będą działały analogicznie jak programy 
z punktu 1 z tą różnicą, że długość wiadomości nie będzie ograniczona do 1024 bajtów. 
Proponowane rozwiązanie: załóżmy, że zarówno klient jak i serwer będzie wysyłał 
na początku 4-bajtową wiadomość, w której prześle rozmiar (jako zakodowany do bajtów int) 
kolejnej wiadomości, która będzie zawierać właściwe dane.
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
            Console.WriteLine("serwer uruchomiony. oczekiwanie na połączenie klienta");

            using TcpClient client = server.AcceptTcpClient();
            using NetworkStream stream = client.GetStream();

            // 1. odbiór nagłówka 
            byte[] lengthBuffer = new byte[4];
            ReadExactly(stream, lengthBuffer, 4);
            
            // konwersja 4 bajtów na typ int
            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

            // 2. odbiór właściwej wiadom o określ rozmiarze
            byte[] messageBuffer = new byte[messageLength];
            ReadExactly(stream, messageBuffer, messageLength);
            
            string receivedMsg = Encoding.UTF8.GetString(messageBuffer);
            Console.WriteLine(receivedMsg);

            // 3. przygotowanie odpowiedzi
            string responseString = "\n \n \n odczytalem: " + receivedMsg;
            byte[] responseData = Encoding.UTF8.GetBytes(responseString);

            // 4. wysłanie nagłówka (4 bajty z rozmiarem odpowiedzi)
            byte[] responseLengthBytes = BitConverter.GetBytes(responseData.Length);
            stream.Write(responseLengthBytes, 0, 4);

            // 5. wysłanie właściwej odpowiedzi
            stream.Write(responseData, 0, responseData.Length);
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

    static void ReadExactly(NetworkStream stream, byte[] buffer, int count)
    {
        int totalBytesRead = 0;
        while (totalBytesRead < count)
        {
            int bytesRead = stream.Read(buffer, totalBytesRead, count - totalBytesRead);
            if (bytesRead == 0) 
            {
                throw new Exception("połączenie zostało zerwane;)");
            }
            totalBytesRead += bytesRead;
        }
    }
}