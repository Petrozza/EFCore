using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //var partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //var carsJson = File.ReadAllText("../../../Datasets/cars.json");
            //var customerJson = File.ReadAllText("../../../Datasets/customers.json");
            //var salesJson = File.ReadAllText("../../../Datasets/sales.json");

            //Console.WriteLine(ImportSuppliers(context, suppliersJson));
            //Console.WriteLine(ImportParts(context, partsJson));
            //Console.WriteLine(ImportCars(context, carsJson));
            //Console.WriteLine(ImportCustomers(context, customerJson));
            //Console.WriteLine(ImportSales(context, salesJson));

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(x => new SalesWithDiscountExport
                {
                    Car = new SaleCar
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    CustomerName = x.Customer.Name,
                    Discount = x.Discount.ToString("f2"),
                    Price = x.Car.PartCars.Sum(p => p.Part.Price).ToString("f2"),
                    
                    PriceWithDiscount = (x.Car.PartCars.Sum(p => p.Part.Price) -
                                      x.Car.PartCars.Sum(p => p.Part.Price) * x.Discount / 100m).ToString("f2")
                })
                .ToList();

            var result = JsonConvert.SerializeObject(sales, Formatting.Indented);
            return result;

        }
		
		public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(x => x.Sales.Any())
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count(),
                    spentMoney = c.Sales.Select(s => s.Car.PartCars.Sum(pc => pc.Part.Price)).Sum()
                })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToArray();

            var output = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return output;

        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(x => new
                {
                    car = new
                    {
                        Make = x.Make,
                        Model = x.Model,
                        TravelledDistance = x.TravelledDistance
                    },
                    parts = x.PartCars.Select(y => new
                    {
                        Name = y.Part.Name,
                        Price = y.Part.Price.ToString("f2")
                    })
                    .ToArray()
                })
                .ToArray();

            var result = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return result;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count()
                })
                //.OrderBy(x => x.Name)
                .ToArray();

            var result = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
            return result;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new 
                {
                    Id = x.Id,
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToArray();

            var result = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return result;
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(x => new 
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = x.IsYoungDriver
                })
                .ToArray();

            var result = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return result;
        }


        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliersDtos = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            //List<Supplier> suppliers = suppliersDtos.Select(x => new Supplier
            //{
            //    Name = x.Name,
            //    IsImporter = x.IsImporter
            //})
            //    .ToList();

            context.Suppliers.AddRange(suppliersDtos);
            context.SaveChanges();
             return $"Successfully imported {suppliersDtos.Count()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var suppliedIds = context.Suppliers
                .Select(x => x.Id)
                .ToArray();

            var parts = JsonConvert
                .DeserializeObject<Part[]>(inputJson)
                .Where(s => suppliedIds.Contains(s.SupplierId))
                .ToArray();
            

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsToImport = JsonConvert.DeserializeObject<CarInputModel[]>(inputJson);
            
            var ListOfCars = new List<Car>();
            var carParts = new List<PartCar>();

            foreach (var carDto in carsToImport)
            {
                
                var currentCar = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance
                };

                foreach (var part in carDto.PartsId.Distinct())
                {
                    var carPart = new PartCar()
                    {
                        PartId = part,
                            Car = currentCar
                    };
                    carParts.Add(carPart);
                }
                ListOfCars.Add(currentCar);
            }


            context.Cars.AddRange(ListOfCars);
            context.PartCars.AddRange(carParts);
            context.SaveChanges();

            return $"Successfully imported {ListOfCars.Count()}.";

        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }


        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }
    }
}