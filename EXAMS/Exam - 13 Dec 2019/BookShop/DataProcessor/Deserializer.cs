namespace BookShop.DataProcessor
{
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(ImportBooksDTO[]), new XmlRootAttribute("Books"));
            var stringRead = new StringReader(xmlString);
            var importedBooks = (ImportBooksDTO[])xmlSerializer.Deserialize(stringRead);

            foreach (var currBook in importedBooks)
            {
                if (!IsValid(currBook))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isPublishedDataValid = DateTime.TryParseExact(currBook.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,  out var date);
                if (!isPublishedDataValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var book = new Book
                {
                    Name = currBook.Name,
                    Genre = (Genre)currBook.Genre,
                    Price = currBook.Price,
                    Pages = currBook.Pages,
                    PublishedOn = date
                };
                context.Books.Add(book);
                sb.AppendLine($"Successfully imported book {currBook.Name} for {currBook.Price:f2}.");
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var importedAuthors = JsonConvert.DeserializeObject<InportAuthorsDTO[]>(jsonString);

            foreach (var currAuthor in importedAuthors)
            {
                if (!IsValid(currAuthor) || context.Authors.Any(x => x.Email == currAuthor.Email))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var author = new Author
                {
                    FirstName = currAuthor.FirstName,
                    LastName = currAuthor.LastName,
                    Email = currAuthor.Email,
                    Phone = currAuthor.Phone
                };

                foreach (var currAuthorBook in currAuthor.Books)
                {
                    if (!currAuthorBook.Id.HasValue)
                    {
                        continue;
                    }
                    var book = context.Books
                        .FirstOrDefault(x => x.Id == currAuthorBook.Id);

                    if (book == null)
                    {
                        continue;
                    }

                    // filling mapping table
                    author.AuthorsBooks.Add(new AuthorBook
                    {
                        //Author = author,
                        Book = book
                    });
                }

                if (author.AuthorsBooks.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                sb.AppendLine($"Successfully imported author - {author.FirstName + ' ' + author.LastName} with {author.AuthorsBooks.Count} books.");
                context.Authors.Add(author);
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}