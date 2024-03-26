using Catalog.Core.Entities;
using Catalog.Core.Specs;

namespace Catalog.Infrastructure.Extensions
{
    internal static class CatalogSpecFilter
    {
        public static IQueryable<Product> FilterProducts(this IQueryable<Product> products, ProductSpecParams? catalogSpecParam)
        {

            if (catalogSpecParam is null)
                return products;
            if (catalogSpecParam.ProductId is not null)
                //products = products.Where(p => p.Id == catalogSpecParam.ProductId);
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.Id == catalogSpecParam.ProductId);
            if (catalogSpecParam.BrandId is not null)
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.BrandID == catalogSpecParam.BrandId);
            if (catalogSpecParam.TypeId is not null)
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.TypeID == catalogSpecParam.TypeId);
            if (catalogSpecParam.ImageId is not null)
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.ImageFileID == catalogSpecParam.ImageId);
            if (!string.IsNullOrEmpty(catalogSpecParam.ProductName))
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.Name == catalogSpecParam.ProductName);
            if (!string.IsNullOrEmpty(catalogSpecParam.BrandName))
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.Brand.Name == catalogSpecParam.BrandName);
            if (!string.IsNullOrEmpty(catalogSpecParam.TypeName))
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.Type.Name == catalogSpecParam.TypeName);
            if (!string.IsNullOrEmpty(catalogSpecParam.ImageDirectory))
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.ImageFileDirectory == null ? false : singleProduct.ImageFileDirectory.Directory == catalogSpecParam.ImageDirectory);
            if (!string.IsNullOrEmpty(catalogSpecParam.ContainsBrandName))
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.Brand.Name.Contains(catalogSpecParam.ContainsBrandName));
            if (!string.IsNullOrEmpty(catalogSpecParam.ContainsTypeName))
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.Type.Name.Contains(catalogSpecParam.ContainsTypeName));
            if (!string.IsNullOrEmpty(catalogSpecParam.ContainsImageDirectory))
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.ImageFileDirectory == null ? false : singleProduct.ImageFileDirectory.Directory.Contains(catalogSpecParam.ContainsImageDirectory));
            if (catalogSpecParam.PriceUnder is not null)
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.Price <= catalogSpecParam.PriceUnder);
            if (catalogSpecParam.PriceOver is not null)
                products = Queryable.Where<Product>(products, singleProduct => singleProduct.Price >= catalogSpecParam.PriceOver);
            
            return products;
        }
        public static IQueryable<Product> SortProducts(this IQueryable<Product> products, ProductSpecParams.SortingType? sortingType)
        {
            if (sortingType is null)
                return products;
            switch (sortingType)
            {
                case ProductSpecParams.SortingType.DescendingById:
                    products = Queryable.OrderByDescending<Product, int>(products, product => product.Id);
                    break;
                case ProductSpecParams.SortingType.AscendingById:
                    products = Queryable.OrderBy<Product, int>(products, product => product.Id);
                    break;
                case ProductSpecParams.SortingType.DescendingByName:
                    products = Queryable.OrderByDescending<Product, string>(products, product => product.Name);
                    break;
                case ProductSpecParams.SortingType.AscendingByName:
                    products = Queryable.OrderBy<Product, string>(products, product => product.Name);
                    break;
                case ProductSpecParams.SortingType.DescendingByBrandId:
                    products = Queryable.OrderByDescending<Product, int>(products, product => product.BrandID);
                    break;
                case ProductSpecParams.SortingType.AscendingByBrandId:
                    products = Queryable.OrderBy<Product, int>(products, product => product.BrandID);
                    break;
                case ProductSpecParams.SortingType.DescendingByTypeId:
                    products = Queryable.OrderByDescending<Product, int>(products, product => product.TypeID);
                    break;
                case ProductSpecParams.SortingType.AscendingByTypeId:
                    products = Queryable.OrderBy<Product, int>(products, product => product.TypeID);
                    break;
                case ProductSpecParams.SortingType.DescendingByBrandName:
                    products = Queryable.OrderByDescending<Product, string>(products, product => product.Brand.Name);
                    break;
                case ProductSpecParams.SortingType.AscendingByBrandName:
                    products = Queryable.OrderBy<Product, string>(products, product => product.Brand.Name);
                    break;
                case ProductSpecParams.SortingType.DescendingByTypeName:
                    products = Queryable.OrderByDescending<Product, string>(products, product => product.Type.Name);
                    break;
                case ProductSpecParams.SortingType.AscendingByTypeName:
                    products = Queryable.OrderBy<Product, string>(products, product => product.Type.Name);
                    break;
                default: 
                    return products;
            }
            return products; // 
        }
    }
}
