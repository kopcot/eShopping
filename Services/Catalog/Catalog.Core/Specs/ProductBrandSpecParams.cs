namespace Catalog.Core.Specs
{
    public class ProductBrandSpecParams
    {
        public int? BrandId { get; set; }
        public string? ContainsBrandName { get; set; }
        public string? BrandName { get; set; }
        public SortingType? Sorting { get; set; } = SortingType.NoSorting;
        public enum SortingType : ushort
        {
            NoSorting,
            DescendingById,
            AscendingById,
            DescendingByName,
            AscendingByName
        }
    }
}
