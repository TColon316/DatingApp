using System.Text.Json;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
        {
            var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));

            // Need another header because by default our client will not have access to this header. We specifically need to return a CORS
            //  header to allow the client to access this header.
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
        }
    }
}