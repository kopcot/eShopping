namespace Basket.Core.Specs
{
    public class ShoppingCartItemSpecParams
    {
        public decimal? PriceOver { get; set; }
        public decimal? PriceUnder { get; set; }

        public string? ProductName { get; set; }
        public string? ContainsProductName { get; set; }
        public string? ImageDirectory { get; set; }
        public string? ContainsImageDirectory { get; set; }

        public SortingType? Sorting { get; set; } = SortingType.NoSorting;
        public enum SortingType : ushort
        {
            NoSorting,
            DescendingById,
            AscendingById,
            DescendingByProductName,
            AscendingByProductName,
            DescendingByQuantity,
            AscendingByQuantity,
            DescendingPrice,
            AscendingPrice,
            DescendingTotalPrice,
            AscendingTotalPrice,
            DescendingImageFile,
            AscendingImageFile,
        }
    }
}
