using System;


namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            Console.Clear();
            Console.ForegroundColor = System.ConsoleColor.Green;
            Console.Write("Въведете име на книга или част от него: ");
            Console.ForegroundColor = System.ConsoleColor.Yellow;
            string input = Console.ReadLine();
            Console.ForegroundColor = System.ConsoleColor.White;
            var result = GetBookTitlesContaining(db, input);
            Console.WriteLine("****************************************");
            Console.WriteLine(result);
            Console.WriteLine("****************************************");
            Console.ForegroundColor = System.ConsoleColor.Black;
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.Copies < 4200)
                .ToList();


            context.Books.RemoveRange(books);

            context.SaveChanges();
            return books.Count;
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .Select(x => new
                {
                    CategoryName = x.Name,
                    Books = x.CategoryBooks.Select(b => new
                    {
                        b.Book.ReleaseDate.Value,
                        b.Book.Title
                    })
                    .OrderByDescending(x => x.Value)
                    .Take(3)
                    .ToList()
                })
                .OrderBy(x => x.CategoryName)
                .ToList();

            var sb = new StringBuilder();
            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.CategoryName}");
                foreach (var book in category.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.Value.Year})");
                }
            }
            return sb.ToString().TrimEnd();
        }


        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(x => new
                {

                    CategoryName = x.Name,
                    Profit = x.CategoryBooks.Sum(x => x.Book.Price * x.Book.Copies)
                })
                .OrderByDescending(x => x.Profit)
                .ThenBy(x => x.CategoryName)
                .ToList();

            var result = string.Join(Environment.NewLine, categories.Select(x => $"{x.CategoryName} ${x.Profit:f2}"));
            return result;
        }


        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    Sum = x.Books.Sum(c => c.Copies)
                })
                .OrderByDescending(x => x.Sum)
                .ToList();

            var sb = new StringBuilder();
            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName} - {author.Sum}");
            }
            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                .Where(x => x.Title.Length > lengthCheck)
                .ToList();
            var result = books.Count;
            return result;
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(x => new
                {
                    BookTitle = x.Title,
                    BId = x.BookId,
                    AuthorFN = x.Author.FirstName,
                    AuthorLN = x.Author.LastName
                })
                .OrderBy(x => x.BId)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => $"{x.BookTitle} ({x.AuthorFN} {x.AuthorLN})"));
            return result;

        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(x => new
                {
                    x.Title
                })
                .OrderBy(x => x.Title)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => x.Title));
            return result;

        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Date < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .Select(x => new
                {
                    x.Title,
                    x.EditionType,
                    x.Price,
                    x.ReleaseDate.Value
                })
                .OrderByDescending(r => r.Value)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => $"{x.Title} - {x.EditionType} - ${x.Price:f2}"));
            return result;
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categoryList = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToLower())
                .ToArray();

            var allCategories = context.BooksCategories
                .Where(x => categoryList.Contains(x.Category.Name.ToLower()))
                .Select(x => x.Book.Title)
                .OrderBy(x => x)
                .ToList();

            var result = string.Join(Environment.NewLine, allCategories);
            return result;

        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                   .Where(x => x.ReleaseDate.Value.Year != year)
                   .Select(x => new
                   {
                       x.BookId,
                       x.Title
                   })
                   .OrderBy(x => x.BookId)
                   .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => x.Title));
            return result;
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.Price > 40)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                })
                .OrderByDescending(x => x.Price)
                .ToList();

            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }
            return sb.ToString().TrimEnd();

            // var result = string.Join(Environment.NewLine, books.Select(x => $"{x.Title} - ${x.Price:f2}"));
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            //EditionType editionType = Enum.Parse<EditionType>("Gold");
            var books = context.Books
                .Where(x => x.EditionType == EditionType.Gold
                && x.Copies < 5000)
                .Select(x => new
                {
                    x.BookId,
                    x.Title
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => x.Title));
            return result;
        }


        public static string GetGoldenBooks(BookShopContext context, string command)
        {
            var agerestr = Enum.Parse<AgeRestriction>(command, true);
            var books = context.Books
                .Where(x => x.AgeRestriction == agerestr)
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToList();

            var result = string.Join(Environment.NewLine, books);
            return result;
        }
    }



}
