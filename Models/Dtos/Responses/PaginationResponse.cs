namespace cs_apiEcommerce.Models.Dtos.Responses;

public class PaginationResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public ICollection<T> Items { get; set; } = [];
}
