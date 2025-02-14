namespace Basket.Core.Specs
{
    public class ShoppingCartSpecParams
    {
        public string? UserName { get; set; }
        public string? ContainsUserName { get; set; }
        public decimal? ItemCountOver { get; set; }
        public decimal? ItemCountUnder { get; set; }
        public decimal? TotalPriceOver { get; set; }
        public decimal? TotalPriceUnder { get; set; }
        public SortingType? Sorting { get; set; } = SortingType.NoSorting;
        public enum SortingType : ushort
        {
            NoSorting,
            DescendingById,
            AscendingById,
            DescendingByUserName,
            AscendingByUserName,
            DescendingByItemCount,
            AscendingByItemCount,
            DescendingTotalPrice,
            AscendingTotalPrice,
        }
    }
}
