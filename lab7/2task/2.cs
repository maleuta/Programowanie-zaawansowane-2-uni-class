using System;
using System.IO;
using System.Security.Cryptography;

namespace HashLaboratory
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("błąd!!!! nieprawidłowa liczba parametrów.");
                Console.WriteLine("sposób użycia: <nazwa_programu> <plik_wejściowy_a> <plik_hash_b> <algorytm_c>");
                Console.WriteLine("dostępne algorytmy: MD5, SHA256, SHA512");
                return;
            }

            string inputFile = args[0];
            string hashFile = args[1];
            string algorithmName = args[2].ToUpper();

            try
            {
                // czy (a) istnieje
                if (!File.Exists(inputFile))
                {
                    throw new FileNotFoundException($"Brak pliku wejściowego: '{inputFile}'");
                }

                // wczyt całego pliku (a) do pamięci RAM 
                byte[] fileData = File.ReadAllBytes(inputFile);
                
                // oblicz hash dla wczyt danych
                string computedHash = ComputeFileHash(fileData, algorithmName);

                if (!File.Exists(hashFile))
                {
                    // przypadek 1: plik (b) nie istnieje - zapisujemy hash
                    File.WriteAllText(hashFile, computedHash);
                    Console.WriteLine($"Sukces! Policzyłem hash algorytmem {algorithmName} i zapisałem go w pliku '{hashFile}'.");
                }
                else
                {

                    // opcja 2: plik (b) istnieje - weryfikujemy zgodność
                    // Trim() - zignor niewidoczne znaki nowej linii czy spacje na końcu pliku
                    string storedHash = File.ReadAllText(hashFile).Trim(); 


                    if (string.Equals(computedHash, storedHash, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("wynik weryfikacji: GIT");
                        Console.WriteLine($"plik jest nienaruszony. hash to: {computedHash}");
                    }
                    else
                    {
                        Console.WriteLine("wynik weryfikacji: NIE GIT :-(");
                        Console.WriteLine($"obliczony hash: {computedHash}");
                        Console.WriteLine($"hash w pliku:   {storedHash}");
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"błąd argumentu: {ex.Message}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Błąd: Brak uprawnień do odczytu lub zapisu któregoś z plików.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił niespodziewany błąd: {ex.Message}");
            }
        }

        // gener hash i zamieniająca go na tekst (hex)
        static string ComputeFileHash(byte[] data, string algorithmName)
        {
            HashAlgorithm hashAlgorithm = null;

            // odpowiedn algorytm
            switch (algorithmName)
            {
                case "MD5":
                    hashAlgorithm = MD5.Create();
                    break;
                case "SHA256":
                    hashAlgorithm = SHA256.Create();
                    break;
                case "SHA512":
                    hashAlgorithm = SHA512.Create();
                    break;
                default:
                    throw new ArgumentException($"nieobsługiwany algorytm: '{algorithmName}'. Akceptowane to tylko MD5, SHA256 lub SHA512.");
            }

            using (hashAlgorithm)
            {
                // oblicz skrót
                byte[] hashBytes = hashAlgorithm.ComputeHash(data);
                
                // form szesnastkowy "A1-B2-C3..." -> "a1b2c3..."
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}