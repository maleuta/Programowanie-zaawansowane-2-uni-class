# Laboratorium 01: Pierwsze aplikacje konsolowe C# .NET Framework Core 7.0.
## Programowanie zaawansowane 2

- Maksymalna liczba punktów: 10

- Skala ocen za punkty:
    - 9-10 ~ bardzo dobry (5.0)
    - 8 ~ plus dobry (4.5)
    - 7 ~ dobry (4.0)
    - 6 ~ plus dostateczny (3.5)
    - 5 ~ dostateczny (3.0)
    - 0-4 ~ niedostateczny (2.0)

Celem laboratorium jest zapoznanie z operacjami wejścia/wyjścia języka C# i praktyki implementacji prostych algorytmów. 

Niektóre programy wymagają podania z linii poleceń pewnych parametrów. Dla uproszczenia przyjmijmy, że programy nie muszą obsługiwać wyjątków spowodowanych ewentualnymi błędami konwersji oraz, że użytkownicy podają odpowiednią liczbę parametrów.

1. W programie Visual Studio Code stwórz nową aplikację konsolową technologii .NET Framework 7.0 i uruchom go. Program ma pobierać z linii komend zestaw napisów oraz jako ostatni parametr liczbę powtórzeń. Program ma wypisać na ekran wszystkie napisy tyle razy, ile wynosiła wartość ostatniego parametru (3 punkt).

```cs

> dotnet new console --framework net7.0
> dotnet run
```

Zamiast net7.0 należy wpisać wersję .net zainstalowaną na komputerze.

```cs

dotnet --version

```

2. Napisz program, który będzie pobierał dane liczbowe klawiatury aż do momentu, kiedy użytkownik wpisze 0. Program ma sumować wpisane liczby a na końcu wyliczyć ich średnią. Wynik zapisz do pliku (2 punkty).

```cs

//Zapis linijki tekstu do pliku w trybie append
StreamWriter sw = new StreamWriter("NazwaPliku.txt", append:true);
sw.WriteLine("Jakiś napis");
sw.Close();

```

3. Napisz program, który w pliku tekstowym zawierającym liczby znajdzie liczbę o największej wartości. Program jako parametr (linii komend) ma pobierać nazwę pliku. Jako wynik do konsoli proszę wypisać tą liczbę oraz numery linijki, w których znaleziono liczbę, na przykład "555, linijka: 10" (2 punkty).

```cs

//czytanie z pliku tekstowego linijka po linijce aż do końca pliku
StreamReader sr = new StreamReader("NazwaPlikuTekstowego.txt");
while (!sr.EndOfStream)
{
    String napis = sr.ReadLine();
}
sr.Close();

```

4. Napisz program, który wypisze gamę dur rozpoczynając od jednego wybranego z dwunastu dźwięków. Są następujące dźwięki:
C, C#, D, D#, E, F, F#, G, G#, A, B, H

Po dźwięku H znowu następuje dźwięk C. Pomiędzy każdym dźwiękiem jest różnica pół tonu. Gama dur tworzona jest w następujący sposób: dźwięk podstawowy, a następnie dźwięki wyższe o: 2, 2, 1, 2, 2, 2, 1 ton. Czyli gama C-dur to: 

C D E F G A H C 

Gama C# dur to: 

C#, D#, F, F#, G#, B, C, C#

Gama kończy się zawsze tym samym dźwiękiem, od którego się zaczynała i ma 8 dźwięków. Program ma pobierać z klawiaturę nazwę dźwięku a na ekran wypisywać gamę (3 punkty).
