// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Globalization;


// namespace Linq
// {
//     // firstly, I need to write down all classes that will be used in files (every field is a string)

//     public class Region
//     {
//         public string RegionID { get; set;}
//         public string RegionDescription {get; set;}

//     }

//     public class Territories
//     {
//         public string TerritoryID { get; set; }
//         public string TerritoryDescription { get; set; }
//         public string RegionID { get; set; }
//     }


//     public class EmployeeTerritories
//     {
//         public string EmployeeID { get; set; }
//         public string TerritoryID { get; set; }
//     }

//     public class Employees
//     {
//         public string EmployeeID { get; set; }
//         public string LastName { get; set; }
//         public string FirstName { get; set; }
//         public string Title { get; set; }
//         public string TitleOfCourtesy { get; set; }
//         public string BirthDate { get; set; }
//         public string HireDate { get; set; }
//         public string Address { get; set; }
//         public string City { get; set; }
//         public string Region { get; set; }
//         public string PostalCode { get; set; }
//         public string Country { get; set; }
//         public string HomePhone { get; set; }
//         public string Extension { get; set; }
//         public string Photo { get; set; }
//         public string Notes { get; set; }
//         public string ReportsTo { get; set; }
//         public string PhotoPath { get; set; }
//     }

//     public class Orders
//     {
//         public string OrderID { get; set; }
//         public string CustomerID { get; set; }
//         public string EmployeeID { get; set; }
//         public string OrderDate { get; set; }
//         public string RequiredDate { get; set; }
//         public string ShippedDate { get; set; }
//         public string ShipVia { get; set; }
//         public string Freight { get; set; }
//         public string ShipName { get; set; }
//         public string ShipAddress { get; set; }
//         public string ShipCity { get; set; }
//         public string ShipRegion { get; set; }
//         public string ShipPostalCode { get; set; }
//         public string ShipCountry { get; set; }
//     }

//     public class OrderDetail
//     {
//         public string OrderID { get; set; }
//         public string ProductID { get; set; }
//         public string UnitPrice { get; set; }
//         public string Quantity { get; set; }
//         public string Discount { get; set; }
//     }


//     // wczytujemy te dane
//     class wczytywacz<T>
//     {
//     public List<T> wczytajListe(String path, Func<String[], T> generuj)
//     {
//         if (!File.Exists(path))
//         {
//             Console.WriteLine($"Błąd: Nie znaleziono pliku {path}");
//             return new List<T>();
//         }
        
//     // skip(1) pomija nagłówki CSV
//         return File.ReadAllLines(path)
//                     .Skip(1)
//                     .Where(line => !string.IsNullOrWhiteSpace(line))
//                     .Select(line => line.Split(','))
//                     .Select(generuj)
//                     .ToList(); 
//     }
//     }

//     class mainProgram
//     {
//         static void Main(string[] args)
//         {
//             string basePath = @"D:\AGH\4sem\4sem_pz2\lab4\"; 

//             var loader = new Wczytywacz<object>();

//             var regions = new Wczytywacz<Region>().WczytajListe(basePath + "regions.csv", 
//                 x => new Region { RegionID = x[0], RegionDescription = x[1].Trim() });

//             var territories = new Wczytywacz<Territory>().WczytajListe(basePath + "territories.csv", 
//                 x => new Territory { TerritoryID = x[0], TerritoryDescription = x[1].Trim(), RegionID = x[2] });

//             var empTerritories = new Wczytywacz<EmployeeTerritory>().WczytajListe(basePath + "employee_territories.csv", 
//                 x => new EmployeeTerritory { EmployeeID = x[0], TerritoryID = x[1] });

//             var employees = new Wczytywacz<Employee>().WczytajListe(basePath + "employees.csv",
//                 x => new Employee {
//                     EmployeeID = x[0],
//                     LastName = x[1],
//                     FirstName = x[2],
//                     Title = x[3],
//                     TitleOfCourtesy = x[4],
//                     BirthDate = x[5],
//                     HireDate = x[6],
//                     Address = x[7],
//                     City = x[8],
//                     Region = x[9],
//                     PostalCode = x[10],
//                     Country = x[11],
//                     HomePhone = x[12],
//                     Extension = x[13],
//                     Photo = x[14],
//                     Notes = x[15],
//                     ReportsTo = x[16],
//                     PhotoPath = x[17]
//                 });

//             // 2. nazwiska pracownikow
//             Console.WriteLine("2. Nazwiska wszystkich pracowników:");
//             var names = employees.Select(e => e.LastName);
//             foreach (var n in names) Console.WriteLine(n);
        
//             Console.WriteLine("3. Nazwisko + region + terytorium");
//             var flatList = from e in employees
//                             join et in empTerritories on e.EmployeeID  equals et.EmployeeID
//                             join ter in territories on et.TerritoryID equals ter.TerritoryID
//                             join reg in regions on ter.RegionID equals reg.RegionID
//                             select new { e.LastName, reg.RegionDescription, ter.TerritoryDescription};

//             foreach (var item in flatList)
//             {
//                 Console.WriteLine($"{item.LastName} | {item.RegionDescriprion} | {item.TerritoryFescription}");
//             } 


//             // 4. REGIONY Z LISTĄ PRACOWNIKÓW (GroupJoin)
//             Console.WriteLine("\n4. Regiony i przypisani pracownicy:");
//             var regionGroups = regions.GroupJoin(
//                 territories.Join(empTerritories, t => t.TerritoryID, et => et.TerritoryID, (t, et) => new { t.RegionID, et.EmployeeID })
//                            .Join(employees, x => x.EmployeeID, e => e.EmployeeID, (x, e) => new { x.RegionID, e.LastName }),
//                 r => r.RegionID,
//                 x => x.RegionID,
//                 (r, emps) => new { 
//                     RegionName = r.RegionDescription, 
//                     EmpNames = emps.Select(e => e.LastName).Distinct() 
//                 }
//             );

//             foreach (var rg in regionGroups)
//             {
//                 Console.WriteLine($"Region: {rg.RegionName}");
//                 foreach (var ename in rg.EmpNames) Console.WriteLine($"  - {ename}");
//             }


//             // 5. LICZBA PRACOWNIKÓW W REGIONACH
//             Console.WriteLine("\n5. Statystyka pracowników w regionach:");
//             foreach (var rg in regionGroups)
//                 Console.WriteLine($"{rg.RegionName}: {rg.EmpNames.Count()}");


//             // 6. ZAMÓWIENIA (Orders & Details) 
//             Console.WriteLine("\n6. Statystyki zamówień pracowników:");
//             var orders = new Wczytywacz<Order>().WczytajListe(basePath + "orders.csv", 
//                 x => new Order { OrderID = x[0], EmployeeID = x[2] });

//             var details = new Wczytywacz<OrderDetail>().WczytajListe(basePath + "orders_details.csv", 
//                 x => new OrderDetail { OrderID = x[0], UnitPrice = x[2], Quantity = x[3], Discount = x[4] });

//             var orderValues = details.GroupBy(d => d.OrderID)
//                 .Select(g => new {
//                     OrderID = g.Key,
//                     Value = g.Sum(od => double.Parse(od.UnitPrice, CultureInfo.InvariantCulture) * double.Parse(od.Quantity, CultureInfo.InvariantCulture) * (1.0 - double.Parse(od.Discount, CultureInfo.InvariantCulture)))
//                 });

//             // Łączymy pracowników z ich zamówieniami
//             var empStats = employees.GroupJoin(
//                 orders.Join(orderValues, o => o.OrderID, ov => ov.OrderID, (o, ov) => new { o.EmployeeID, ov.Value }),
//                 e => e.EmployeeID,
//                 ov => ov.EmployeeID,
//                 (e, evals) => new {
//                     e.LastName,
//                     Count = evals.Count(),
//                     Avg = evals.Any() ? evals.Average(x => x.Value) : 0,
//                     Max = evals.Any() ? evals.Max(x => x.Value) : 0
//                 }
//             );

//             foreach (var s in empStats)
//                 Console.WriteLine($"{s.LastName.PadRight(10)} | Liczba: {s.Count} | Średnia: {s.Avg:F2} | Max: {s.Max:F2}");

//             Console.WriteLine("\nNaciśnij dowolny klawisz, aby zakończyć...");
//             Console.ReadKey();

        
//         }

        
//     }
// }