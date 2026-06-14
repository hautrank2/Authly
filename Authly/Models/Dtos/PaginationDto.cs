namespace Authly.Models.Dtos
{
    public class PaginationDto<T>
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage => (PageSize ?? 0) > 0
            ? (int)Math.Ceiling((double)TotalCount / (PageSize ?? 1))
            : 0;
        public List<T> Items { get; set; } = [];
    }
}
