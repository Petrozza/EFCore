namespace VaporStore.DataProcessor
{
	using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var genre = context.Genres
				.ToList()
				.Where(x => genreNames.Contains(x.Name))
				.Select(x => new
				{
					Id = x.Id,
					Genre = x.Name,
					Games = x.Games.Select(g => new
					{
						Id = g.Id,
						Title = g.Name,
						Developer = g.Developer.Name,
						Tags = string.Join(", ", g.GameTags.Select(t => t.Tag.Name)),
						Players = g.Purchases.Count()
					})
					.Where(x => x.Players > 0)
					.OrderByDescending(x => x.Players)
					.ThenBy(x => x.Id),
					TotalPlayers = x.Games.Sum(p => p.Purchases.Count)
				})
				.OrderByDescending(x => x.TotalPlayers)
				.ThenBy(x => x.Id);

			return JsonConvert.SerializeObject(genre, Formatting.Indented);

		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
			var users = context.Users.ToList()
				.Where(u => u.Cards.Any(p => p.Purchases.Any(t => t.Type.ToString() == storeType)))
				.Select(x => new UserExportModel
				{
					Username = x.Username,
					Purchases = x.Cards.SelectMany(c => c.Purchases)
						.Where(p => p.Type.ToString() == storeType)
						.Select(p => new PurchaseExportModel
						{
							Card = p.Card.Number,
							Cvc = p.Card.Cvc,
							Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
							Game = new GameExportModel
							{
								Title = p.Game.Name,
								Genre = p.Game.Genre.Name,
								Price = p.Game.Price
							}
						})
						.OrderBy(x => x.Date)
						.ToArray(),
					TotalSpent = x.Cards.Sum(c => c.Purchases.Where(t => t.Type.ToString() == storeType).Sum(p => p.Game.Price))
				})
				.OrderByDescending(x => x.TotalSpent)
				.ThenBy(x => x.Username)
				.ToArray();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserExportModel[]), new XmlRootAttribute("Users"));
			var textWriter = new StringWriter();
			var ns = new XmlSerializerNamespaces();
			ns.Add("", "");
			xmlSerializer.Serialize(textWriter, users, ns);
			return textWriter.ToString().TrimEnd();

		}
	}
}