namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2}!";

        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";

        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var impoertedMovies = JsonConvert
            .DeserializeObject<ImportMoviesDTO[]>(jsonString);

            foreach (var mov in impoertedMovies)
            {
                var existingMovies = context.Movies.Select(t => t.Title);
                if (context.Movies.Any(x => x.Title == mov.Title))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                if (!IsValid(mov)) //|| context.Movies.Where(x => x.Title == mov.Title) == null)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var movie = new Movie
                {
                    Title = mov.Title,
                    Genre = Enum.Parse<Genre>(mov.Genre),
                    Duration = TimeSpan.ParseExact(mov.Duration, "c", CultureInfo.InvariantCulture),
                    Director = mov.Director
                };

                context.Movies.Add(movie);
                context.SaveChanges();
                sb.AppendLine($"Successfully imported {movie.Title} with genre {movie.Genre} and rating {mov.Rating:f2}!");
            }
            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(ImportProjectionsDTO[]), new XmlRootAttribute("Projections"));
            var textReader = new StringReader(xmlString);

                var importedProjections = (ImportProjectionsDTO[])xmlSerializer.Deserialize(textReader);

            foreach (var proj in importedProjections)
            {
                var parseDate = DateTime.TryParseExact(proj.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);

                if (!IsValid(proj) || (!parseDate))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                List<int> validMovieIds = context.Movies.Select(m => m.Id).ToList();

                if (!validMovieIds.Contains(proj.MovieId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Projection projection = new Projection
                {
                    MovieId = proj.MovieId,
                    DateTime = date
                };

                context.Projections.Add(projection);
                context.SaveChanges();

                sb.AppendLine($"Successfully imported projection {projection.Movie.Title} on {projection.DateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}!");
            }

                return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerTicketsDTO[]), new XmlRootAttribute("Customers"));
            var textReader = new StringReader(xmlString);
            var importedCustomersTickets = (ImportCustomerTicketsDTO[])xmlSerializer.Deserialize(textReader);

            foreach (var cust in importedCustomersTickets)
            {
                if (!IsValid(cust))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var customer = new Customer
                {
                    FirstName = cust.FirstName,
                    LastName = cust.LastName,
                    Age = cust.Age,
                    Balance = cust.Balance
                };

                foreach (var tick in cust.Tickets)
                {
                    if (!IsValid(tick))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    var ticket = new Ticket
                    {
                        ProjectionId = tick.ProjectionId,
                        Price = tick.Price
                    };

                    customer.Tickets.Add(ticket);
                }
                context.Customers.Add(customer);
                context.SaveChanges();
                sb.AppendLine($"Successfully imported customer {customer.FirstName} {customer.LastName} with bought tickets: {customer.Tickets.Count}!");
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