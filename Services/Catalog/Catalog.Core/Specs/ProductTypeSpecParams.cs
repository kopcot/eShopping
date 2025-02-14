namespace Catalog.Core.Specs
{
    public class ProductTypeSpecParams
    {
        public int? TypeId { get; set; }
        public string? TypeName { get; set; }
        public string? ContainsTypeName { get; set; }
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
