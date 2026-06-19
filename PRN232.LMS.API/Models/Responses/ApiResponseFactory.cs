using System.Dynamic;

namespace PRN232.LMS.API.Models.Responses;

public static class ApiResponseFactory
{
    public static ApiResponse<List<ExpandoObject>> CreatePagedList(
        List<ExpandoObject> items,
        int page,
        int pageSize,
        int totalItems,
        int totalPages)
    {
        return new ApiResponse<List<ExpandoObject>>
        {
            Success = true,
            Message = "Request processed successfully",
            Data = items,
            Errors = null,
            Pagination = new PaginationMetadata
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            }
        };
    }
}
