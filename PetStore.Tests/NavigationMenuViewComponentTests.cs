﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetStore.Components;
using PetStore.Models;
using PetStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using Xunit;

namespace PetStore.Tests
{
    public class NavigationMenuViewComponentTests
    {
        private ArticleType[] GetArticleTypes()
        {
            return new ArticleType[] {
                new ArticleType() { ArticleTypeId = 1, Name = "P1", Description = "D1" },
                new ArticleType() { ArticleTypeId = 2, Name = "P2", Description = "D2" },
                new ArticleType() { ArticleTypeId = 3, Name = "P3", Description = "D3" },
                new ArticleType() { ArticleTypeId = 4, Name = "P4", Description = "D4" },
            };
        }

        [Fact]
        public void Can_Select_ArticleTypes()
        {
            var ArticleTypes = GetArticleTypes();
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.ArticleTypes)
                .Returns(ArticleTypes.AsQueryable());

            NavigationMenuViewComponent target = new NavigationMenuViewComponent(mock.Object);

            var componentResult = target.Invoke() as ViewViewComponentResult;
            var result = componentResult.ViewData.Model as IEnumerable<ArticleType>;

            Assert.True(Enumerable.SequenceEqual(ArticleTypes, result));
        }

        [Fact]
        public void Indicates_Selected_ArticleType()
        {
            long ArticleTypeToSelect = 1;
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.ArticleTypes)
                .Returns(GetArticleTypes().AsQueryable());

            NavigationMenuViewComponent target = new NavigationMenuViewComponent(mock.Object);

            target.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext
                {
                    RouteData = new Microsoft.AspNetCore.Routing.RouteData()
                }
            };

            target.RouteData.Values["ArticleType"] = ArticleTypeToSelect;

            var result = (target.Invoke() as ViewViewComponentResult).ViewData["SelectedArticleType"] as long?;

            Assert.Equal(ArticleTypeToSelect, result);
        }
    }
}
