using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;

namespace LaboratoriumLinq
{
    // --- MODELE DANYCH (Zadanie 1) ---
    // Przyjmujemy zgodnie z poleceniem, że każde pole to String
    
    public class Region
    {
        public string RegionID { get; set; }
        public string RegionDescription { get; set; }
    }

    public class Territory
    {
        public string TerritoryID { get; set; }
        public string TerritoryDescription { get; set; }
        public string RegionID { get; set; }
    }

    public class EmployeeTerritory
    {
        public string EmployeeID { get; set; }
        public string TerritoryID { get; set; }
    }

    public class Employee
    {
        public string EmployeeID { get; set; }
        public string LastName { get; set; }
    }

    public class Order
    {
        public string OrderID { get; set; }
        public string EmployeeID { get; set; }
    }

    public class OrderDetail
    {
        public string OrderID { get; set; }
        public string UnitPrice { get; set; }
        public string Quantity { get; set; }
        public string Discount { get; set; }
    }

    // --- UNIWERSALNA KLASA WCZYTUJĄCA (Zadanie 1) ---
    class Wczytywacz<T>
    {
        public List<T> WczytajListe(string path, Func<string[], T> generuj)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"Błąd: Nie znaleziono pliku {path}");
                return new List<T>();
            }

            // Skip(1) pomija nagłówki CSV
            return File.ReadAllLines(path)
                       .Skip(1)
                       .Where(line => !string.IsNullOrWhiteSpace(line))
                       .Select(line => line.Split(','))
                       .Select(generuj)
                       .ToList();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // --- KONFIGURACJA ŚCIEŻKI ---
            // Zmień poniższą ścieżkę na folder, w którym masz pliki .csv
            string basePath = @"D:\AGH\4sem\4sem_pz2\lab4\"; 

            var loader = new Wczytywacz<object>(); // Pomocniczy obiekt do typów

            // --- 1. WCZYTYWANIE DANYCH ---
            var regions = new Wczytywacz<Region>().WczytajListe(basePath + "regions.csv", 
                x => new Region { RegionID = x[0], RegionDescription = x[1].Trim() });

            var territories = new Wczytywacz<Territory>().WczytajListe(basePath + "territories.csv", 
                x => new Territory { TerritoryID = x[0], TerritoryDescription = x[1].Trim(), RegionID = x[2] });

            var empTerritories = new Wczytywacz<EmployeeTerritory>().WczytajListe(basePath + "employee_territories.csv", 
                x => new EmployeeTerritory { EmployeeID = x[0], TerritoryID = x[1] });

            var employees = new Wczytywacz<Employee>().WczytajListe(basePath + "employees.csv", 
                x => new Employee { EmployeeID = x[0], LastName = x[1] });


            // --- 2. NAZWISKA WSZYSTKICH PRACOWNIKÓW ---
            Console.WriteLine("2. Nazwiska wszystkich pracowników:");
            var names = employees.Select(e => e.LastName);
            foreach (var n in names) Console.WriteLine(n);


            // --- 3. PRACOWNIK - REGION - TERYTORIUM (Płaska lista) ---
            Console.WriteLine("\n3. Nazwisko - Region - Terytorium:");
            var flatList = from e in employees
                           join et in empTerritories on e.EmployeeID equals et.EmployeeID
                           join t in territories on et.TerritoryID equals t.TerritoryID
                           join r in regions on t.RegionID equals r.RegionID
                           select new { e.LastName, r.RegionDescription, t.TerritoryDescription };

            foreach (var item in flatList)
                Console.WriteLine($"{item.LastName} | {item.RegionDescription} | {item.TerritoryDescription}");


            // --- 4. REGIONY Z LISTĄ PRACOWNIKÓW (GroupJoin) ---
            Console.WriteLine("\n4. Regiony i przypisani pracownicy:");
            var regionGroups = regions.GroupJoin(
                territories.Join(empTerritories, t => t.TerritoryID, et => et.TerritoryID, (t, et) => new { t.RegionID, et.EmployeeID })
                           .Join(employees, x => x.EmployeeID, e => e.EmployeeID, (x, e) => new { x.RegionID, e.LastName }),
                r => r.RegionID,
                x => x.RegionID,
                (r, emps) => new { 
                    RegionName = r.RegionDescription, 
                    EmpNames = emps.Select(e => e.LastName).Distinct() 
                }
            );

            foreach (var rg in regionGroups)
            {
                Console.WriteLine($"Region: {rg.RegionName}");
                foreach (var ename in rg.EmpNames) Console.WriteLine($"  - {ename}");
            }


            // --- 5. LICZBA PRACOWNIKÓW W REGIONACH ---
            Console.WriteLine("\n5. Statystyka pracowników w regionach:");
            foreach (var rg in regionGroups)
                Console.WriteLine($"{rg.RegionName}: {rg.EmpNames.Count()}");


            // --- 6. ZAMÓWIENIA (Orders & Details) ---
            Console.WriteLine("\n6. Statystyki zamówień pracowników:");
            var orders = new Wczytywacz<Order>().WczytajListe(basePath + "orders.csv", 
                x => new Order { OrderID = x[0], EmployeeID = x[2] });

            var details = new Wczytywacz<OrderDetail>().WczytajListe(basePath + "orders_details.csv", 
                x => new OrderDetail { OrderID = x[0], UnitPrice = x[2], Quantity = x[3], Discount = x[4] });

            // Obliczamy wartość każdego zamówienia (suma pozycji)
            var orderValues = details.GroupBy(d => d.OrderID)
                .Select(g => new {
                    OrderID = g.Key,
                    Value = g.Sum(od => double.Parse(od.UnitPrice, CultureInfo.InvariantCulture) * double.Parse(od.Quantity, CultureInfo.InvariantCulture) * (1.0 - double.Parse(od.Discount, CultureInfo.InvariantCulture)))
                });

            // Łączymy pracowników z ich zamówieniami
            var empStats = employees.GroupJoin(
                orders.Join(orderValues, o => o.OrderID, ov => ov.OrderID, (o, ov) => new { o.EmployeeID, ov.Value }),
                e => e.EmployeeID,
                ov => ov.EmployeeID,
                (e, evals) => new {
                    e.LastName,
                    Count = evals.Count(),
                    Avg = evals.Any() ? evals.Average(x => x.Value) : 0,
                    Max = evals.Any() ? evals.Max(x => x.Value) : 0
                }
            );

            foreach (var s in empStats)
                Console.WriteLine($"{s.LastName.PadRight(10)} | Liczba: {s.Count} | Średnia: {s.Avg:F2} | Max: {s.Max:F2}");

            Console.WriteLine("\nNaciśnij dowolny klawisz, aby zakończyć...");
            Console.ReadKey();
        }
    }
}