namespace BookShop.DataProcessor
{
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors
                .Select(x => new
                {
                    AuthorName = x.FirstName + ' ' + x.LastName,
                    Books = x.AuthorsBooks
                            .OrderByDescending(b => b.Book.Price)
                            .Select(b => new
                            {
                                BookName = b.Book.Name,
                                BookPrice = b.Book.Price.ToString("f2")
                            })
                             .ToArray()

                })
                .ToArray()
                .OrderByDescending(x => x.Books.Length)
                .ThenBy(x => x.AuthorName);
                //.ToArray();

            var jsonOutput = JsonConvert.SerializeObject(authors, Formatting.Indented);
            return jsonOutput;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var booksXml = context.Books
                .Where(x => x.PublishedOn < date && x.Genre == Genre.Science)
                .ToArray()
                .OrderByDescending(x => x.Pages)
                .ThenByDescending(x => x.PublishedOn)
                .Take(10)
                .Select(b => new ExportOldestBooksDTO()
                {
                    Pages = b.Pages,
                    Name = b.Name,
                    Date = b.PublishedOn.ToString("d", CultureInfo.InvariantCulture)
                })
                .ToArray();

            XmlSerializer XmlSerializer = new XmlSerializer(typeof(ExportOldestBooksDTO[]), new XmlRootAttribute("Books"));

            StringWriter textWrite = new StringWriter();
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer.Serialize(textWrite, booksXml, ns);
            return textWrite.ToString().TrimEnd();
        }
    }
}