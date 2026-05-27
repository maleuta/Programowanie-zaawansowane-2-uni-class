using System;
using System.IO;
using System.Security.Cryptography;

namespace RSALaboratory
{
    class Program
    {
        const string PublicKeyFile = "public_key.xml";
        const string PrivateKeyFile = "private_key.xml";

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Błąd, nie podano typu polecenia.");
                PrintHelp();
                return;
            }

            try
            {
                // interpret 1 arg jako liczby
                int command = int.Parse(args[0]);

                switch (command)
                {
                    case 0:
                        GenerateKeys();
                        break;

                    case 1:
                        if (args.Length < 3) 
                            throw new ArgumentException("Dla polecenia '1' musisz podać plik wejściowy i wyjściowy.");
                        EncryptFile(args[1], args[2]);
                        break;

                    case 2:
                        if (args.Length < 3) 
                            throw new ArgumentException("Dla polecenia '2' musisz podać plik wejściowy i wyjściowy.");
                        DecryptFile(args[1], args[2]);
                        break;

                    default:
                        Console.WriteLine("Błąd: Nieznany typ polecenia. Akceptowane wartości to tylko 0, 1 lub 2.");
                        PrintHelp();
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Błąd: Typ polecenia musi być liczbą całkowitą (0, 1 lub 2).");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
                PrintHelp();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Błąd: Brak wymaganego pliku! Sprawdź ścieżkę: '{ex.FileName}'");
            }
            catch (CryptographicException)
            {
                Console.WriteLine("Błąd kryptograficzny! Możliwe przyczyny to:");
                Console.WriteLine(" - Próbujesz odszyfrować plik przy pomocy niewłaściwego klucza.");
                Console.WriteLine(" - Plik z danymi (lub plik klucza) jest uszkodzony.");
                Console.WriteLine(" - UWAGA: Próbujesz zaszyfrować plik, który jest zbyt duży. Sam algorytm RSA " +
                                  "służy do szyfrowania bardzo małych porcji danych (np. kluczy sesyjnych).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił niespodziewany błąd: {ex.Message}");
            }
        }

        static void GenerateKeys()
        {
            // instancja RSA z kluczem o dł 2048 bit
            using (RSA rsa = RSA.Create(2048))
            {
                string publicKey = rsa.ToXmlString(false); 
                string privateKey = rsa.ToXmlString(true); 

                File.WriteAllText(PublicKeyFile, publicKey);
                File.WriteAllText(PrivateKeyFile, privateKey);

                Console.WriteLine("Sukces! Wygenerowano nowe klucze RSA.");
                Console.WriteLine($"Klucz publiczny zapisano do: {PublicKeyFile}");
                Console.WriteLine($"Klucz prywatny zapisano do:  {PrivateKeyFile}");
            }
        }

        static void EncryptFile(string inputFile, string outputFile)
        {
            if (!File.Exists(PublicKeyFile))
            {
                throw new FileNotFoundException("Nie znaleziono pliku z kluczem publicznym. Uruchom najpierw program z poleceniem '0'.", PublicKeyFile);
            }
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException("Nie znaleziono pliku do zaszyfrowania.", inputFile);
            }

            string publicKeyXml = File.ReadAllText(PublicKeyFile);
            
            // wczyt całego pliku do pamięci 
            byte[] dataToEncrypt = File.ReadAllBytes(inputFile);

            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(publicKeyXml);
                
                // bezpieczn padding OAEP z SHA-256
                byte[] encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);
                
                File.WriteAllBytes(outputFile, encryptedData);
                Console.WriteLine($"Sukces! Zaszyfrowano plik '{inputFile}' i zapisano wynik do '{outputFile}'.");
            }
        }

        static void DecryptFile(string inputFile, string outputFile)
        {
            if (!File.Exists(PrivateKeyFile))
            {
                throw new FileNotFoundException("Nie znaleziono pliku z kluczem prywatnym. Uruchom najpierw program z poleceniem '0'.", PrivateKeyFile);
            }
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException("Nie znaleziono pliku do odszyfrowania.", inputFile);
            }

            string privateKeyXml = File.ReadAllText(PrivateKeyFile);
            byte[] dataToDecrypt = File.ReadAllBytes(inputFile);

            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(privateKeyXml);
                
                byte[] decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.OaepSHA256);
                
                File.WriteAllBytes(outputFile, decryptedData);
                Console.WriteLine($"Sukces! Odszyfrowano plik '{inputFile}' i odzyskane dane zapisano do '{outputFile}'.");
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("\nSposób użycia programu:");
            Console.WriteLine("  <nazwa_programu> 0           -> Generuje klucze RSA (public_key.xml i private_key.xml)");
            Console.WriteLine("  <nazwa_programu> 1 <a> <b>   -> Szyfruje plik <a> i zapisuje do <b> (używa klucza publicznego)");
            Console.WriteLine("  <nazwa_programu> 2 <a> <b>   -> Odszyfrowuje plik <a> i zapisuje do <b> (używa klucza prywatnego)");
        }
    }
}