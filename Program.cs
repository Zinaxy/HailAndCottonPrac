using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompanyXLogistics
{
    // Package class
    public class Package
    {
        public string SerialNumber { get; }
        public string QualityMark { get; }
        public double Mass { get; }
        public string PackageType { get; } // "loose" or "carton"
        public DateTime DateAdded { get; }

        public Package(string serialNumber, string qualityMark, double mass, string packageType)
        {
            SerialNumber = serialNumber;
            QualityMark = qualityMark;
            Mass = mass;
            PackageType = packageType;
            DateAdded = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Package(SerialNumber={SerialNumber}, QualityMark={QualityMark}, Mass={Mass}, Type={PackageType})";
        }

        // Convert package to CSV format for file storage
        public string ToCsv()
        {
            return $"{SerialNumber},{QualityMark},{Mass},{PackageType},{DateAdded}";
        }

        // Create a package from CSV format
        public static Package FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            return new Package(values[0], values[1], double.Parse(values[2]), values[3]);
        }
    }

    // Pallet class
    public class Pallet
    {
        public string SerialNumber { get; }
        public int Capacity { get; }

        public Pallet(string serialNumber, int capacity)
        {
            SerialNumber = serialNumber;
            Capacity = capacity;
        }

        public override string ToString()
        {
            return $"Pallet(SerialNumber={SerialNumber}, Capacity={Capacity})";
        }
    }

    // WarehouseManager class
    public class WarehouseManager
    {
        private List<Package> _packages = new List<Package>();
        private const string FilePath = "packages.csv";

        public WarehouseManager()
        {
            LoadPackagesFromFile();
        }

        // Load packages from file when the program starts
        private void LoadPackagesFromFile()
        {
            if (File.Exists(FilePath))
            {
                var lines = File.ReadAllLines(FilePath);
                foreach (var line in lines)
                {
                    _packages.Add(Package.FromCsv(line));
                }
            }
        }

        // Save packages to file
        private void SavePackagesToFile()
        {
            var lines = _packages.Select(p => p.ToCsv()).ToArray();
            File.WriteAllLines(FilePath, lines);
        }

        public void LoadPackage(Package package, string warehouseName, string rackSerial, int lineNumber, Pallet pallet = null)
        {
            // Add package to the list (simulating storage)
            _packages.Add(package);
            Console.WriteLine($"Package loaded into Warehouse: {warehouseName}, Rack: {rackSerial}, Line: {lineNumber}");
            if (pallet != null)
            {
                Console.WriteLine($"Package placed on Pallet: {pallet.SerialNumber}");
            }

            // Save packages to file after adding a new package
            SavePackagesToFile();
        }

        public Package? SearchPackage(string serialNumber)
        {
            return _packages.Find(p => p.SerialNumber == serialNumber);
        }

        // List all packages
        public void ListPackages()
        {
            if (_packages.Count == 0)
            {
                Console.WriteLine("No packages found.");
                return;
            }

            Console.WriteLine("List of Packages:");
            foreach (var package in _packages)
            {
                Console.WriteLine(package);
            }
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            WarehouseManager manager = new WarehouseManager();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n--- Logistics Management System ---");
                Console.WriteLine("1. Add Package");
                Console.WriteLine("2. Search Package");
                Console.WriteLine("3. List All Packages");
                Console.WriteLine("4. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter Package Serial Number: ");
                        var serialNumber = Console.ReadLine()!; 
                        Console.Write("Enter Quality Mark: ");
                        var qualityMark = Console.ReadLine()!; 
                        Console.Write("Enter Mass (kg): ");
                        double mass = double.Parse(Console.ReadLine()!); 
                        Console.Write("Enter Package Type (loose/carton): ");
                        var packageType = Console.ReadLine()!; 

                        var package = new Package(serialNumber, qualityMark, mass, packageType);

                        Console.Write("Enter Warehouse Name: ");
                        var warehouseName = Console.ReadLine()!; 
                        Console.Write("Enter Rack Serial Number: ");
                        var rackSerial = Console.ReadLine()!; 
                        Console.Write("Enter Line Number: ");
                        int lineNumber = int.Parse(Console.ReadLine()!); 

                        Pallet? pallet = null;
                        if (packageType == "loose")
                        {
                            Console.Write("Enter Pallet Serial Number: ");
                            var palletSerial = Console.ReadLine()!; 
                            Console.Write("Enter Pallet Capacity: ");
                            var palletCapacity = int.Parse(Console.ReadLine()!); 
                            pallet = new Pallet(palletSerial, palletCapacity);
                        }

                        try
                        {
                            manager.LoadPackage(package, warehouseName, rackSerial, lineNumber, pallet);
                            Console.WriteLine("Package added successfully!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case "2":
                        Console.Write("Enter Package Serial Number to search: ");
                        var searchSerial = Console.ReadLine()!; 
                        var foundPackage = manager.SearchPackage(searchSerial);
                        if (foundPackage != null)
                        {
                            Console.WriteLine("Package found: " + foundPackage);
                        }
                        else
                        {
                            Console.WriteLine("Package not found.");
                        }
                        break;

                    case "3":
                        manager.ListPackages();
                        break;

                    case "4":
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }
    }
}