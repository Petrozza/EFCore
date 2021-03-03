using CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeFirst
{
    public class Program
    {
        static void Main(string[] args)
        {
            var db = new ApplicationDbContext();
            //db.Database.EnsureCreated();
            //AddCategoryNewsAndComents(db);

            //PrintNewsAndCategories(db);

            var neededComment = db.Comments.FirstOrDefault(x => x.Author == "Macho");
            neededComment.Author = "Vancho";

            db.SaveChanges();
        }

        private static void PrintNewsAndCategories(ApplicationDbContext db)
        {
            var news = db.News.Select(x => new
            {
                Name = x.Title,
                CAtegoryName = x.Category.Title,
            });

            foreach (var singleNews in news)
            {
                Console.WriteLine($"{singleNews.Name}->{singleNews.CAtegoryName}");
            }
        }

        private static void AddCategoryNewsAndComents(ApplicationDbContext db)
        {
            db.Categories.Add(new Category
            {
                Title = "Music",
                News = new List<News>
                {
                    new News
                    {
                        Title = "Metallica recorded new album",
                        Content = "Aired on 31.12.2021",
                        Comments = new List<Comments>
                        {
                            new Comments { Author = "Acho", Content = "Yepp" },
                            new Comments { Author = "Vasko", Content = "Finally"}
                        }
                    }
                }
            });
        }
    }
}
