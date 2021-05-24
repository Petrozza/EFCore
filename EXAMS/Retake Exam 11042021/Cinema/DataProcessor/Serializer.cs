namespace Cinema.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using System.Linq;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                .AsEnumerable()
                .Where(m => m.Rating >= rating && m.Projections.Count(x => x.Tickets.Any()) > 0)
                .Select(m => new
                {
                    MovieName = m.Title,
                    Rating = $"{m.Rating:f2}",
                    TotalIncomes = $"{m.Projections.Sum(x => x.Tickets.Sum(x => x.Price)):f2}",
                    Customers = m.Projections.SelectMany(x => x.Tickets).Select(t => new
                    {
                        FirstName = t.Customer.FirstName,
                        LastName = t.Customer.LastName,
                        Balance = $"{t.Customer.Balance:f2}",
                    })
                    .OrderByDescending(c => c.Balance)
                    .ThenBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .ToList()
                })
                .OrderByDescending(m => double.Parse(m.Rating))
                .ThenByDescending(m => decimal.Parse(m.TotalIncomes))
                .Take(10)
                .ToList();

            var moviesAsJsonString = JsonConvert.SerializeObject(movies, Formatting.Indented);

            return moviesAsJsonString;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            return "NOT YET";
        }
    }
}