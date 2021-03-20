using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var usersXml = File.ReadAllText("../../../Datasets/users.xml");
            //var productsXml = File.ReadAllText("../../../Datasets/products.xml");
            //var categoriesXml = File.ReadAllText("../../../Datasets/categories.xml");
            //var categoriesProductsXml = File.ReadAllText("../../../Datasets/categories-products.xml");

            //Console.WriteLine(ImportUsers(context, usersXml));
            //Console.WriteLine(ImportProducts(context, productsXml));
            //Console.WriteLine(ImportCategories(context, categoriesXml));
            //Console.WriteLine(ImportCategoryProducts(context, categoriesProductsXml));

            Console.WriteLine(GetSoldProducts(context));
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(u => u.ProductsSold.Any(x => x.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .Select(x => new UsersSoldProductsModel()
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Where(p => p.Buyer != null)
                        .Select(p => new SoldProductsModel()
                        {
                            Name = p.Name,
                            Price = p.Price
                        }).ToArray()

                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UsersSoldProductsModel[]), new XmlRootAttribute("Users"));

            var textwriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(textwriter, users, ns);

            var result = textwriter.ToString();
            //File.WriteAllText(@"../../../users-sold-products.xml", result); NOT WORKING IN JUDGE
            return result;

        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new ProductsInRangeModel
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProductsInRangeModel[]), new XmlRootAttribute("Products"));

            var textwriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(textwriter, products, ns);

            var result = textwriter.ToString();
            File.WriteAllText(@"../../../products-in-range.xml", result);
            return result;

        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCategoriesProductsModel[]), new XmlRootAttribute("CategoryProducts"));
            var textRead = new StringReader(inputXml);
            var categoriesProductsDtos = (ImportCategoriesProductsModel[])xmlSerializer.Deserialize(textRead);
            List<CategoryProduct> categoriesProducts = new List<CategoryProduct>();

            foreach (var categoryProductDto in categoriesProductsDtos)
            {
                if(context.Categories.Any(c => c.Id == categoryProductDto.CategoryId)
                    && context.Products.Any(p => p.Id == categoryProductDto.ProductId))
                {
                    CategoryProduct categoryProduct = new CategoryProduct()
                    {
                        CategoryId = categoryProductDto.CategoryId,
                        ProductId = categoryProductDto.ProductId
                    };
                    categoriesProducts.Add(categoryProduct);
                }
                
            }

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count}";
        }


        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCategoriesModel[]), new XmlRootAttribute("Categories"));
            var textRead = new StringReader(inputXml);
            var categoriesDtos = (ImportCategoriesModel[])xmlSerializer.Deserialize(textRead);

            List<Category> categories = new List<Category>();

            foreach (var categoryDto in categoriesDtos.Where(x => x.Name != null))
            {
                Category category = new Category
                {
                    Name = categoryDto.Name
                };
                categories.Add(category);
            }
            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }


        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProductsModel[]), new XmlRootAttribute("Products"));
            var textRead = new StringReader(inputXml);
            var productsDtos = (ImportProductsModel[])xmlSerializer.Deserialize(textRead);

            List<Product> products = new List<Product>();

            foreach (var productDto in productsDtos)
            {
                Product product = new Product
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    SellerId = productDto.SellerId,
                    BuyerId = productDto.BuyerId
                };
                products.Add(product);
            }
            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";

        }


        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportUsersModel[]), new XmlRootAttribute("Users"));
            var textRead = new StringReader(inputXml);
            var usersDtos = (ImportUsersModel[])xmlSerializer.Deserialize(textRead);

            List<User> users = new List<User>();

            foreach (var userDto in usersDtos)
            {
                User user = new User()
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Age = userDto.Age
                };
                users.Add(user);
            }
            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
    }
}