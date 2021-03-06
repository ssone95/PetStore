﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading.Tasks;
using PetStore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace PetStore.Repository.DbSeed
{
    public static class SeedData
    {
        public static void EnsureDataExists(IApplicationBuilder appBuilder)
        {
            DatabaseContext _context = appBuilder
                .ApplicationServices
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<DatabaseContext>();

            if (_context.Database.GetPendingMigrations().Any())
                _context.Database.Migrate();


            var anyArticles = _context.Articles.Any();

            if (!anyArticles)
            {
                var ArticleTypes = GetMainArticleTypes();
                _context.ArticleTypes.AddRange(ArticleTypes);

                _context.SaveChanges();

                var ArticlesWithArticleTypes = GetArticles();

                var Articles = ArticlesWithArticleTypes.Select(x => x.Key)
                    .ToList();

                _context.Articles.AddRange(Articles);


                foreach (var Article in ArticlesWithArticleTypes.Keys)
                {
                    var ArticleArticleType = ArticlesWithArticleTypes
                        .Where(x => x.Key == Article)
                        .FirstOrDefault().Value ?? "";

                    Article.ArticleTypeId = _context.ArticleTypes.FirstOrDefault(x => x.Name == ArticleArticleType).ArticleTypeId;
                }

                _context.SaveChanges();
            }
        }

        static List<string> GetTableNames(this DatabaseContext context)
        {

            return new List<string>()
            {
                "Articles", "ArticleTypes", "ArticleArticleTypes"
            };
        }

        static Dictionary<Article, string> GetArticles()
        {
            return new Dictionary<Article, string>()
            {
                {
                    new Article()
                    {
                        Name = "Article 1",
                        Description = "Article Description 1",
                        Price = GetRandomPrice()
                    }, GetRandomArticleType()
                },
                {
                    new Article()
                    {
                        Name = "Article 2",
                        Description = "Article Description 2",
                        Price = GetRandomPrice()
                    }, GetRandomArticleType()
                },
                {
                    new Article()
                    {
                        Name = "Article 3",
                        Description = "Article Description 3",
                        Price = GetRandomPrice()
                    }, GetRandomArticleType()
                },
                {
                    new Article()
                    {
                        Name = "Article 4",
                        Description = "Article Description 4",
                        Price = GetRandomPrice()
                    }, GetRandomArticleType()
                },
                {
                    new Article()
                    {
                        Name = "Article 5",
                        Description = "Article Description 5",
                        Price = GetRandomPrice()
                    }, GetRandomArticleType()
                }
            };
        }

        private static string GetRandomArticleType()
        {
            var random = new Random();
            var ArticleTypes = GetMainArticleTypes();
            var ArticleTypesCount = ArticleTypes.Count;
            int numRandomArticleType = random.Next(1, ArticleTypesCount);

            return ArticleTypes[numRandomArticleType].Name;
        }

        private static decimal GetRandomPrice()
        {
            var random = new Random();
            var basePrice = (decimal)(random.Next(5, 75) * 1.0);
            var decimalPrice = (decimal)(random.NextDouble() * 99);
            return basePrice + decimalPrice;
        }

        static List<ArticleType> GetMainArticleTypes()
        {
            return new List<ArticleType>()
            {
                new ArticleType() { Name = "Food", Description = "Granules in liquid and solid form, canned food, supplements, vitamins, ..." },
                new ArticleType() { Name = "Collars", Description = "Collars, leashes, chains and similar equipment" },
                new ArticleType() { Name = "Accessories", Description = "Various shampoos, baths and preparations for the care of your pets" },
                new ArticleType() { Name = "Treats", Description = "Treats for your pets" },
                new ArticleType() { Name = "Medications", Description = "Medications for your pets" },
            };
        }
    }
}
