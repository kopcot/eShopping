namespace Shared.Core.Specs
{
    public class Pagination
    {
        public bool UsePagination { get; set; } = false;
        private int pageIndex = 0;
        public int PageIndex { get => pageIndex; set => pageIndex = Math.Max(value, 0); }
        private int pageSize = 1;
        public int PageSize { get => pageSize; set => pageSize = Math.Max(value, 1); }
    }
}
