using Catalog.Core.Entities;
using Catalog.Core.Specs;

namespace Catalog.Infrastructure.Extensions
{
    internal static class CatalogSpecFilter 
    {
        public static IQueryable<Product> Filter(this IQueryable<Product> items, ProductSpecParams? specParam)
        {
            if (specParam is null)
                return items;
            if (specParam.ProductId is not null)
                items = Queryable.Where<Product>(items, product => product.Id == specParam.ProductId);
            if (specParam.BrandId is not null)
                items = Queryable.Where<Product>(items, product => product.BrandID == specParam.BrandId);
            if (specParam.TypeId is not null)
                items = Queryable.Where<Product>(items, product => product.TypeID == specParam.TypeId);
            if (specParam.ImageId is not null)
                items = Queryable.Where<Product>(items, product => product.ImageFileID == specParam.ImageId);
            if (!string.IsNullOrEmpty(specParam.ProductName))
                items = Queryable.Where<Product>(items, product => product.Name == specParam.ProductName);
            if (!string.IsNullOrEmpty(specParam.BrandName))
                items = Queryable.Where<Product>(items, product => product.Brand.Name == specParam.BrandName);
            if (!string.IsNullOrEmpty(specParam.TypeName))
                items = Queryable.Where<Product>(items, product => product.Type.Name == specParam.TypeName);
            if (!string.IsNullOrEmpty(specParam.ImageDirectory))
                items = Queryable.Where<Product>(items, product => product.ImageFileDirectory == null ? false : product.ImageFileDirectory.Directory == specParam.ImageDirectory);
            if (!string.IsNullOrEmpty(specParam.ContainsBrandName))
                items = Queryable.Where<Product>(items, product => product.Brand.Name.Contains(specParam.ContainsBrandName));
            if (!string.IsNullOrEmpty(specParam.ContainsTypeName))
                items = Queryable.Where<Product>(items, product => product.Type.Name.Contains(specParam.ContainsTypeName));
            if (!string.IsNullOrEmpty(specParam.ContainsImageDirectory))
                items = Queryable.Where<Product>(items, product => product.ImageFileDirectory == null ? false : product.ImageFileDirectory.Directory.Contains(specParam.ContainsImageDirectory));
            if (specParam.PriceUnder is not null)
                items = Queryable.Where<Product>(items, product => product.Price <= specParam.PriceUnder);
            if (specParam.PriceOver is not null)
                items = Queryable.Where<Product>(items, product => product.Price >= specParam.PriceOver);
            
            return items;
        }

        public static IQueryable<ProductBrand> Filter(this IQueryable<ProductBrand> items, ProductBrandSpecParams? specParam)
        {
            if (specParam is null)
                return items;

            return items;
        }
        public static IQueryable<ProductType> Filter(this IQueryable<ProductType> items, ProductTypeSpecParams? specParam)
        {
            if (specParam is null)
                return items;
            
            return items;
        }
        public static IQueryable<ImageFileDirectory> Filter(this IQueryable<ImageFileDirectory> items, ImageFileDirectorySpecParams? specParam)
        {
            if (specParam is null)
                return items;

            return items;
        }
        public static IQueryable<Product> Sort(this IQueryable<Product> items, ProductSpecParams.SortingType? sortingType)
        {
            if (sortingType is null)
                return items;
            switch (sortingType)
            {
                case ProductSpecParams.SortingType.DescendingById:
                    items = Queryable.OrderByDescending<Product, int>(items, product => product.Id);
                    break;
                case ProductSpecParams.SortingType.AscendingById:
                    items = Queryable.OrderBy<Product, int>(items, product => product.Id);
                    break;
                case ProductSpecParams.SortingType.DescendingByName:
                    items = Queryable.OrderByDescending<Product, string>(items, product => product.Name);
                    break;
                case ProductSpecParams.SortingType.AscendingByName:
                    items = Queryable.OrderBy<Product, string>(items, product => product.Name);
                    break;
                case ProductSpecParams.SortingType.DescendingByBrandId:
                    items = Queryable.OrderByDescending<Product, int>(items, product => product.BrandID);
                    break;
                case ProductSpecParams.SortingType.AscendingByBrandId:
                    items = Queryable.OrderBy<Product, int>(items, product => product.BrandID);
                    break;
                case ProductSpecParams.SortingType.DescendingByTypeId:
                    items = Queryable.OrderByDescending<Product, int>(items, product => product.TypeID);
                    break;
                case ProductSpecParams.SortingType.AscendingByTypeId:
                    items = Queryable.OrderBy<Product, int>(items, product => product.TypeID);
                    break;
                case ProductSpecParams.SortingType.DescendingByBrandName:
                    items = Queryable.OrderByDescending<Product, string>(items, product => product.Brand.Name);
                    break;
                case ProductSpecParams.SortingType.AscendingByBrandName:
                    items = Queryable.OrderBy<Product, string>(items, product => product.Brand.Name);
                    break;
                case ProductSpecParams.SortingType.DescendingByTypeName:
                    items = Queryable.OrderByDescending<Product, string>(items, product => product.Type.Name);
                    break;
                case ProductSpecParams.SortingType.AscendingByTypeName:
                    items = Queryable.OrderBy<Product, string>(items, product => product.Type.Name);
                    break;
                default: 
                    return items;
            }
            return items; // 
        }
        public static IQueryable<ProductBrand> Sort(this IQueryable<ProductBrand> items, ProductBrandSpecParams.SortingType? sortingType)
        {
            if (sortingType is null)
                return items;
            switch (sortingType)
            {
                case ProductBrandSpecParams.SortingType.DescendingById:
                    items = Queryable.OrderByDescending<ProductBrand, int>(items, product => product.Id);
                    break;
                case ProductBrandSpecParams.SortingType.AscendingById:
                    items = Queryable.OrderBy<ProductBrand, int>(items, product => product.Id);
                    break;
                case ProductBrandSpecParams.SortingType.DescendingByName:
                    items = Queryable.OrderByDescending<ProductBrand, string>(items, product => product.Name);
                    break;
                case ProductBrandSpecParams.SortingType.AscendingByName:
                    items = Queryable.OrderBy<ProductBrand, string>(items, product => product.Name);
                    break;
                default:
                    return items;
            }

            return items;
        }
        public static IQueryable<ProductType> Sort(this IQueryable<ProductType> items, ProductTypeSpecParams.SortingType? sortingType)
        {
            if (sortingType is null)
                return items;
            switch (sortingType)
            {
                case ProductTypeSpecParams.SortingType.DescendingById:
                    items = Queryable.OrderByDescending<ProductType, int>(items, product => product.Id);
                    break;
                case ProductTypeSpecParams.SortingType.AscendingById:
                    items = Queryable.OrderBy<ProductType, int>(items, product => product.Id);
                    break;
                case ProductTypeSpecParams.SortingType.DescendingByName:
                    items = Queryable.OrderByDescending<ProductType, string>(items, product => product.Name);
                    break;
                case ProductTypeSpecParams.SortingType.AscendingByName:
                    items = Queryable.OrderBy<ProductType, string>(items, product => product.Name);
                    break;
                default:
                    return items;
            }

            return items;
        }
        public static IQueryable<ImageFileDirectory> Sort(this IQueryable<ImageFileDirectory> items, ImageFileDirectorySpecParams.SortingType? sortingType)
        {
            if (sortingType is null)
                return items;
            switch (sortingType)
            {
                case ImageFileDirectorySpecParams.SortingType.DescendingById:
                    items = Queryable.OrderByDescending<ImageFileDirectory, int>(items, product => product.Id);
                    break;
                case ImageFileDirectorySpecParams.SortingType.AscendingById:
                    items = Queryable.OrderBy<ImageFileDirectory, int>(items, product => product.Id);
                    break;
                case ImageFileDirectorySpecParams.SortingType.DescendingByDirectory:
                    items = Queryable.OrderByDescending<ImageFileDirectory, string>(items, product => product.Directory);
                    break;
                case ImageFileDirectorySpecParams.SortingType.AscendingByDirectory:
                    items = Queryable.OrderBy<ImageFileDirectory, string>(items, product => product.Directory);
                    break;
                default:
                    return items;
            }

            return items;
        }
    }
}
