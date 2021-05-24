using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTO.Input;
using CarDealer.DTO.Output;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var suppliersXml = File.ReadAllText("./Datasets/suppliers.xml");
            var partsXml = File.ReadAllText("./Datasets/parts.xml");
            var carsXml = File.ReadAllText("./Datasets/cars.xml");
            var customersXml = File.ReadAllText("./Datasets/customers.xml");
            var salesXml = File.ReadAllText("./Datasets/sales.xml");
            System.Console.WriteLine(ImportSuppliers(context, suppliersXml));
            System.Console.WriteLine(ImportParts(context, partsXml));
            System.Console.WriteLine(ImportCars(context, carsXml));
            System.Console.WriteLine(ImportCustomers(context, customersXml));
            System.Console.WriteLine(ImportSales(context, salesXml));


            //Console.WriteLine(GetSalesWithAppliedDiscount(context));

        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            

            var sales = context.Sales
                .Select(x => new SalesWithDiscountModel()
                {
                    Car = new CarSaleModel()
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    
                    Discount = x.Discount,
                    CustomerName = x.Customer.Name,
                    Price = x.Car.PartCars.Sum(x => x.Part.Price),
                    PriceWithDiscount = x.Car.PartCars.Sum(x => x.Part.Price)  - x.Car.PartCars.Sum(x => x.Part.Price) * x.Discount / 100
                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SalesWithDiscountModel[]), new XmlRootAttribute("sales"));

            var textwriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(textwriter, sales, ns);

            var result = textwriter.ToString();
            //File.WriteAllText(@"../../../local-suppliers.xml", result);
            return result;
        }





        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(x => x.Sales.Any())
                .Select(x => new TotalSalesByCustomerModel
                {
                    FullName = x.Name,
                    BoughtCars = x.Sales.Count,
                    SpentMoney = x.Sales.Select(x => x.Car)
                    .SelectMany(y => y.PartCars)
                    .Sum(x => x.Part.Price)

                })
                .OrderByDescending(x => x.SpentMoney)
                .ToList();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TotalSalesByCustomerModel[]), new XmlRootAttribute("customers"));

            var textwriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(textwriter, customers, ns);

            var result = textwriter.ToString();
            //File.WriteAllText(@"../../../local-suppliers.xml", result);
            return result;

        }


        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(x => new CarsWithParts
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    Parts = x.PartCars.Select(p => new CarParts
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()
                })
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CarsWithParts[]), new XmlRootAttribute("cars"));

            var textwriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(textwriter, cars, ns);

            var result = textwriter.ToString();
            //File.WriteAllText(@"../../../local-suppliers.xml", result);
            return result;

        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            //StringBuilder sb = new StringBuilder();

            var suppliers = context
                .Suppliers
                .Where(x => !x.IsImporter)
                .Select(x => new LocalSuppliersModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(LocalSuppliersModel[]), new XmlRootAttribute("suppliers"));

            var textwriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textwriter, suppliers, ns);

            var result = textwriter.ToString();
            //File.WriteAllText(@"../../../local-suppliers.xml", result);
            return result;
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .Select(x => new CarsFromBmw
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CarsFromBmw[]), new XmlRootAttribute("cars"));

            var textwriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textwriter, cars, ns);

            var result = textwriter.ToString();
            File.WriteAllText(@"../../../bmw-cars.xml", result);
            return result;
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.TravelledDistance > 2000000)
                .Select(x => new CarOutputModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CarOutputModel[]), new XmlRootAttribute("cars"));

            var textwriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textwriter, cars, ns);

            var result = textwriter.ToString();
            return result;

        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            const string root = "Sales";
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SalesInputModel[]), new XmlRootAttribute(root));

            var textRead = new StringReader(inputXml);
            var salesDtos = (SalesInputModel[])xmlSerializer.Deserialize(textRead);

            var validCarsIds = context.Cars.Select(x => x.Id).ToList();

            var neededSales = salesDtos
                .Where(x => validCarsIds.Contains(x.CarId))
                .Select(x => new Sale
                {
                    CarId = x.CarId,
                    CustomerId = x.CustomerId,
                    Discount = x.Discount
                })
                .ToList();

            context.Sales.AddRange(neededSales);
            context.SaveChanges();

            return $"Successfully imported {neededSales.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            const string root = "Customers";
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CustomerInputModel[]), new XmlRootAttribute(root));
            var textRead = new StringReader(inputXml);
            var CustomersDtos = (CustomerInputModel[])xmlSerializer.Deserialize(textRead);

            List<Customer> customers = new List<Customer>();

            foreach (var custromerDto in CustomersDtos)
            {
                Customer customer = new Customer 
                {
                    Name = custromerDto.Name,
                    BirthDate = custromerDto.BirthDAte,
                    IsYoungDriver = custromerDto.IsYoungDriver
                };
                customers.Add(customer);
            }
            context.Customers.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Count()}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            const string root = "Cars";
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CarInputModel[]), new XmlRootAttribute(root));
            var textRead = new StringReader(inputXml);
            var carDtos = xmlSerializer.Deserialize(textRead) as CarInputModel[];

            var allParts = context.Parts.Select(x => x.Id).ToList();
            var cars = new List<Car>();

            foreach (var currentCar in carDtos)
            {
                var distinctedParts = currentCar.CarPartsInputModel.Select(x => x.Id).Distinct();
                var partCars = distinctedParts
                    .Intersect(allParts)
                    .Select(pc => new PartCar 
                    {
                        PartId = pc
                    })
                    .ToList();

                var car = new Car
                {
                    Make = currentCar.Make,
                    Model = currentCar.Model,
                    TravelledDistance = currentCar.TravelDistance,
                    PartCars = partCars
                };

                cars.Add(car);
            }
            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            const string root = "Parts";

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PartInputModel[]), new XmlRootAttribute(root));

            var textRead = new StringReader(inputXml);

            var partInputModels = xmlSerializer.Deserialize(textRead) as PartInputModel[];

            var neededSuppliersIds = context.Suppliers
                .Select(x => x.Id)
                .ToList();

            var parts = partInputModels
                .Where(x => neededSuppliersIds.Contains(x.SupplierId))
                .Select(x => new Part
                {
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SupplierId = x.SupplierId
                })
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();
               
            return $"Successfully imported {parts.Count}";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SupplierInputModel[]), new XmlRootAttribute("Suppliers"));
            var textRead = new StringReader(inputXml);
            var suppliersDTOs = (SupplierInputModel[])xmlSerializer.Deserialize(textRead);

            List<Supplier> suppliers = new List<Supplier>();

            foreach (var supplierDTO in suppliersDTOs)
            {
                Supplier supplier = new Supplier()
                {
                    Name = supplierDTO.Name,
                    IsImporter = supplierDTO.IsImporter
                };
                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}";
        }
    }
}