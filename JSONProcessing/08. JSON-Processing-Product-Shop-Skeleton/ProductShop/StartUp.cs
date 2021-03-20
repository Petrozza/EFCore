using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DataTransferObjects;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            var productShopContext = new ProductShopContext();
            //productShopContext.Database.EnsureDeleted();
            //productShopContext.Database.EnsureCreated();

            //var usersJson = File.ReadAllText("../../../Datasets/users.json");
            //var productsJson = File.ReadAllText("../../../Datasets/products.json");
            //var categoriesJson = File.ReadAllText("../../../Datasets/categories.json");
            //var categoriesProductsJson = File.ReadAllText("../../../Datasets/categories-products.json");
            //ImportUsers(productShopContext, usersJson);
            //ImportProducts(productShopContext, productsJson);
            //ImportCategories(productShopContext, categoriesJson);
            //ImportCategoryProducts(productShopContext, categoriesProductsJson);
            //Console.WriteLine(result);

            File.WriteAllText("users-and-products.json", GetUsersWithProducts(productShopContext));
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Include(x => x.ProductsSold)
                .ToList()
                .Where(x => x.ProductsSold.Any(b => b.BuyerId != null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    age = x.Age,
                    soldProducts = new
                    {
                        count = x.ProductsSold.Where(y => y.BuyerId != null).Count(),
                        products = x.ProductsSold.Where(y => y.BuyerId != null)
                        .Select(c => new
                        {
                            name = c.Name,
                            price = c.Price
                        })
                    }
                })
                .OrderByDescending(x => x.soldProducts.products.Count())
                .ToList();

            var resultObjects = new
            {
                usersCount = users.Count(),
                users = users
            };

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var result = JsonConvert.SerializeObject(resultObjects, Formatting.Indented, settings);
            return result;
        }





        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categegories = context.Categories
                //.OrderByDescending(x => x.CategoryProducts.Count())
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count(),
                    averagePrice = x.CategoryProducts.Count == 0 ? 0.ToString("f2") : x.CategoryProducts.Average(p => p.Product.Price).ToString("f2"),
                    totalRevenue = x.CategoryProducts.Sum(p => p.Product.Price).ToString("f2")
                    //.ToList()
                })
                .OrderByDescending(x => x.productsCount)
                .ToList();

            var result = JsonConvert.SerializeObject(categegories, Formatting.Indented);
            return result;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(b => b.BuyerId != null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold.Where(b => b.BuyerId != null)
                    .Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        buyerFirstName = p.Buyer.FirstName,
                        buyerLastName = p.Buyer.LastName
                    })
                    .ToList()
                })
                .OrderBy(x => x.lastName)
                .ThenBy(x => x.firstName)
                .ToList();

            var result = JsonConvert.SerializeObject(users, Formatting.Indented);
            return result;

        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = x.Seller.FirstName + " " + x.Seller.LastName
                })
                .OrderBy(x => x.price)
                .ToList();

            var productsInRange = JsonConvert.SerializeObject(products, Formatting.Indented);
            return productsInRange;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();
            var dtoCAtegoriesProducts = JsonConvert.DeserializeObject<IEnumerable<CategoriesProductsInputModel>>(inputJson);
                var categoriesProducts = mapper.Map<IEnumerable<CategoryProduct>>(dtoCAtegoriesProducts);

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();
            return $"Successfully imported {categoriesProducts.Count()}";

        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();
            
            var dtoCAtegories = JsonConvert.DeserializeObject<IEnumerable<CategoriesInputModel>>(inputJson)
                .Where(x => x.Name != null);
            var categories = mapper.Map<IEnumerable<Category>>(dtoCAtegories);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();
            var dtoProducts = JsonConvert.DeserializeObject<IEnumerable<ProductInputModel>>(inputJson);
            var products = mapper.Map<IEnumerable<Product>>(dtoProducts);

            context.Products.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Count()}";
        }

        public static string ImportUsers(ProductShopContext context, string inputJson) 
        {
            InitializeAutomapper();
            //var users = JsonConvert.DeserializeObject<User[]>(inputJson);
            var dtoUsers = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);
            var users = mapper.Map<IEnumerable<User>>(dtoUsers);

            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Count()}";

        }

        private static void InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });
            mapper = config.CreateMapper();
        }

    }
}