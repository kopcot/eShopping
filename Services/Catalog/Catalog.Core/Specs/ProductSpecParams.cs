namespace Catalog.Core.Specs
{
    public class ProductSpecParams
    {
        #region Product
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? PriceOver { get; set; }
        public decimal? PriceUnder { get; set; }
        #endregion  Product
        #region ProductBrand
        public int? BrandId { get; set; }
        public string? ContainsBrandName { get; set; }
        public string? BrandName { get; set; }
        #endregion  ProductBrand
        #region ProductType
        public int? TypeId { get; set; }
        public string? TypeName { get; set; }
        public string? ContainsTypeName { get; set; }
        #endregion ProductType
        #region ImageFile
        public int? ImageId { get; set; }
        public string? ImageDirectory { get; set; }
        public string? ContainsImageDirectory { get; set; }
        #endregion ImageFile
        public SortingType? Sorting { get; set; } = SortingType.NoSorting;
        public enum SortingType : ushort
        { 
            NoSorting,
            DescendingById,
            AscendingById,
            DescendingByName,
            AscendingByName,
            DescendingByBrandId,
            AscendingByBrandId,
            DescendingByTypeId,
            AscendingByTypeId,
            DescendingByBrandName,
            AscendingByBrandName,
            DescendingByTypeName,
            AscendingByTypeName
        }
    }
}
