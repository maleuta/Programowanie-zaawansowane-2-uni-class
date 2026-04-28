using System;
using System.Net;
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
            Console.WriteLine("Serwer uruchomiony. Oczekiwanie na połączenie klienta...");

            using TcpClient client = server.AcceptTcpClient();
            using NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            string receivedMsg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine(receivedMsg);

            string responseString = "odczytalem: " + receivedMsg;
            byte[] responseData = Encoding.UTF8.GetBytes(responseString);

            if (responseData.Length > 1024)
            {
                Array.Resize(ref responseData, 1024);
            }

            stream.Write(responseData, 0, responseData.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine("Błąd serwera: " + e.Message);
        }
        finally
        {
            // zatrzym serwera i zakończenie programu
            server?.Stop();
        }
    }
}