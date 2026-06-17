namespace ProductManagment_APIs.DTOs
{
    public class PagedRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 0;

        public string? SearchColumn { get; set; }
        public string? SearchValue { get; set; }
        public string? SortColumn { get; set; } = "CreatedAt";
        public string? SortDirection { get; set; } = "desc";

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
