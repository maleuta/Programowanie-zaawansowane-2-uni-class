using System;
using System.Collections.Generic; 

class Program
{
    static void Main()
    {

    // właścicieli
    OsobaFizyczna osoba1 = new OsobaFizyczna("Jan", "Kowalski", "Adam", "12345678901", null);
    OsobaFizyczna osoba2 = new OsobaFizyczna("Anna", "Nowak", "Maria", "10987654321", null);

    // listy właścicieli
    List<PosiadaczRachunku> lista = new List<PosiadaczRachunku> { osoba1, osoba2 };

    // rachunek
    RachunekBankowy rachunek = new RachunekBankowy("123", 1000m, false, lista);

    rachunek = rachunek - osoba1;

    try
    {
        rachunek = rachunek - osoba2;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Wyjątek: " + ex.Message);
    }

        // posiadaczy rachunków
        // OsobaFizyczna jan = new OsobaFizyczna("Jan", "Kowalski", "Adam", "12345678901", "AB123456");
        // OsobaPrawna firma = new OsobaPrawna("Firma IT Sp. z o.o.", "Kraków");

        // Console.WriteLine("Stworzono posiadaczy:");
        // Console.WriteLine(jan.ToString());
        // Console.WriteLine(firma.ToString());

        // // przygotowanie list właścicieli
        // List<PosiadaczRachunku> wlascicieleJana = new List<PosiadaczRachunku> { jan };
        // List<PosiadaczRachunku> wlascicieleFirmy = new List<PosiadaczRachunku> { firma };

        // // tworzenie rachunków
        // // Jan zaczyna z 0 zł, debet dozwolony
        // RachunekBankowy kontoJana = new RachunekBankowy("1111-2222", 0m, true, wlascicieleJana);
        // // Firma zaczyna z 1000 zł, debet NIEDOZWOLONY
        // RachunekBankowy kontoFirmy = new RachunekBankowy("9999-8888", 1000m, false, wlascicieleFirmy);

        // Console.WriteLine($"Stan początkowy:");
        // Console.WriteLine($"Konto Jana: {kontoJana.StanRachunku} zł");
        // Console.WriteLine($"Konto Firmy: {kontoFirmy.StanRachunku} zł");
        // Console.WriteLine("-------------------------------------------------\n");

        // // testowanie transakcji

        // // wpłata gotówkowa (rachunek źródłowy = null)
        // RachunekBankowy.DokonajTransakcji(null, kontoJana, 500m, "Wpłata w oddziale");
        // Console.WriteLine($"Po wpłacie 500 zł na konto Jana -> Stan: {kontoJana.StanRachunku} zł");

        // // przelew z konta Jana na konto Firmy (300 zł)
        // RachunekBankowy.DokonajTransakcji(kontoJana, kontoFirmy, 300m, "Opłata za usługi");
        // Console.WriteLine($"Po przelewie 300 zł od Jana do Firmy:");
        // Console.WriteLine($"Konto Jana: {kontoJana.StanRachunku} zł");
        // Console.WriteLine($"Konto Firmy: {kontoFirmy.StanRachunku} zł");

        // // wypłata z bankomatu z konta Firmy (rachunek docelowy = null)
        // RachunekBankowy.DokonajTransakcji(kontoFirmy, null, 150m, "Wypłata z bankomatu");
        // Console.WriteLine($"Po wypłacie 150 zł z konta Firmy -> Stan: {kontoFirmy.StanRachunku} zł\n");

        // // testowanie wyjątków (brak debetu)
        // Console.WriteLine("Próba przelewu 2000 zł z konta Firmy na konto Jana (brak debetu)...");
        // try
        // {
        //     RachunekBankowy.DokonajTransakcji(kontoFirmy, kontoJana, 2000m, "Zbyt duży przelew");
        // }
        // catch (Exception ex)
        // {
        //     // przechwytujemy wyjątek i wyświetlamy jego wiadomość
        //     Console.WriteLine($"ZŁAPANO BŁĄD: {ex.Message}");
        // }
    }
}