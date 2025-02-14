using Basket.Core.Entities;
using Basket.Core.Specs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Infrastructure.Extensions
{
    internal static class BasketSpecFilter
    {
        public static IQueryable<ShoppingCart> Filter(this IQueryable<ShoppingCart> items, ShoppingCartSpecParams? specParam)
        {
            if (specParam is null)
                return items;

            return items;
        }
        public static IQueryable<ShoppingCartItem> Filter(this IQueryable<ShoppingCartItem> items, ShoppingCartItemSpecParams? specParam)
        {
            if (specParam is null)
                return items;

            return items;
        }
        public static IQueryable<ShoppingCart> Sort(this IQueryable<ShoppingCart> items, ShoppingCartSpecParams.SortingType? sortingType)
        {
            if (sortingType is null)
                return items;
            switch (sortingType)
            {
                case ShoppingCartSpecParams.SortingType.DescendingById:
                    items = Queryable.OrderByDescending<ShoppingCart, int>(items, shoppingCart => shoppingCart.Id);
                    break;
                case ShoppingCartSpecParams.SortingType.AscendingById:
                    items = Queryable.OrderBy<ShoppingCart, int>(items, shoppingCart => shoppingCart.Id);
                    break;
                case ShoppingCartSpecParams.SortingType.DescendingByUserName:
                    items = Queryable.OrderByDescending<ShoppingCart, string>(items, shoppingCart => shoppingCart.UserName);
                    break;
                case ShoppingCartSpecParams.SortingType.AscendingByUserName:
                    items = Queryable.OrderBy<ShoppingCart, string>(items, shoppingCart => shoppingCart.UserName);
                    break;
                case ShoppingCartSpecParams.SortingType.DescendingByItemCount:
                    items = Queryable.OrderByDescending<ShoppingCart, int>(items, shoppingCart => shoppingCart.Items.Count);
                    break;
                case ShoppingCartSpecParams.SortingType.AscendingByItemCount:
                    items = Queryable.OrderBy<ShoppingCart, int>(items, shoppingCart => shoppingCart.Items.Count);
                    break;
                case ShoppingCartSpecParams.SortingType.DescendingTotalPrice:
                    items = Queryable.OrderByDescending<ShoppingCart, decimal>(items, shoppingCart => shoppingCart.Items.Sum(x => x.IsDeleted ? 0 : x.Price * x.Quantity));
                    break;
                case ShoppingCartSpecParams.SortingType.AscendingTotalPrice:
                    items = Queryable.OrderBy<ShoppingCart, decimal>(items, shoppingCart => shoppingCart.Items.Sum(x => x.IsDeleted ? 0 : x.Price * x.Quantity)); 
                    break;
                default:
                    return items;
            }
            return items; // 
        }
        public static IQueryable<ShoppingCartItem> Sort(this IQueryable<ShoppingCartItem> items, ShoppingCartItemSpecParams.SortingType? sortingType)
        {
            if (sortingType is null)
                return items;
            return items; 

        }
    }
}
