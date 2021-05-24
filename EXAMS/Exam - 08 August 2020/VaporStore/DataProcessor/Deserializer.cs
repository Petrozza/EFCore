namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			var sb = new StringBuilder();
			var impoertedGames = JsonConvert.DeserializeObject<GamesImportModel[]>(jsonString);

            foreach (var importedGame in impoertedGames)
            {
                if (!IsValid(importedGame) || importedGame.Tags.Count() == 0)
                {
					sb.AppendLine("Invalid Data");
					continue;
                }

				var genre = context.Genres.FirstOrDefault(x => x.Name == importedGame.Genre)
					?? new Genre { Name = importedGame.Genre };
				var developer = context.Developers.FirstOrDefault(x => x.Name == importedGame.Developer)
					?? new Developer { Name = importedGame.Developer };

				var game = new Game
				{
					Name = importedGame.Name,
					Price = importedGame.Price,
					ReleaseDate = importedGame.ReleaseDate.Value,
					Genre = genre,
					Developer = developer
				};

                foreach (var importTag in importedGame.Tags)
                {
					var tag = context.Tags.FirstOrDefault(x => x.Name == importTag)
						?? new Tag { Name = importTag };
					game.GameTags.Add(new GameTag { Tag = tag }); // filling mapping table!!!
                }

				context.Games.Add(game);
				context.SaveChanges();
				sb.AppendLine($"Added {importedGame.Name} ({importedGame.Genre}) with {importedGame.Tags.Count()} tags");
            }
			return sb.ToString().TrimEnd();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			var sb = new StringBuilder();
			var importUsers = JsonConvert.DeserializeObject<UsersImportModel[]>(jsonString);
            foreach (var importUser in importUsers)
            {
                if (!IsValid(importUser) || !importUser.Cards.All(IsValid))
                {
					sb.AppendLine("Invalid Data");
					continue;
                }

				var user = new User
				{
					FullName = importUser.FullName,
					Username = importUser.Username,
					Email = importUser.Email,
					Age = importUser.Age,
					Cards = importUser.Cards.Select(x => new Card
					{
						Number = x.Number,
						Cvc = x.CVC,
						Type = x.Type.Value
					})
					.ToArray()
				};
				context.Users.Add(user);
				sb.AppendLine($"Imported {importUser.Username} with {importUser.Cards.Count()} cards");
            }
			context.SaveChanges();
			return sb.ToString().TrimEnd();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			var sb = new StringBuilder();
			var xmlSerializer = new XmlSerializer(typeof(PurchasesImportModel[]), new XmlRootAttribute("Purchases"));
			var textReader = new StringReader(xmlString);
			var importedPurchases = (PurchasesImportModel[])xmlSerializer.Deserialize(textReader);
            
			foreach (var currPurchase in importedPurchases)
            {
                if (!IsValid(currPurchase))
                {
					sb.AppendLine("Invalid Data");
					continue;
                }

				bool parsedDate = DateTime.TryParseExact(
					currPurchase.Date, 
					"dd/MM/yyyy HH:mm", 
					CultureInfo.InvariantCulture, 
					DateTimeStyles.None, 
					out var neddedDate);
                if (!parsedDate)
                {
					sb.AppendLine("Invalid Data");
					continue;
                }
				
				var purchase = new Purchase
				{
					Type = currPurchase.Type.Value,
					Date = neddedDate,
					ProductKey = currPurchase.Key,
					Card = context.Cards.FirstOrDefault(x => x.Number == currPurchase.Card),
					Game = context.Games.FirstOrDefault(x => x.Name == currPurchase.GameName)
				};
				context.Purchases.Add(purchase);

				//var username = context.Users
				//	.Where(x => x.Id == purchase.Card.UserId)
				//	.Select(x => x.Username)
				//	.FirstOrDefault();
				sb.AppendLine($"Imported {currPurchase.GameName} for {purchase.Card.User.Username}");

				//context.Purchases.Add(purchase);

            }
			context.SaveChanges();
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